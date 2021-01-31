using System;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using log4net;
using Ninject;
using Shuttle.Core.Container;
using Shuttle.Core.Log4Net;
using Shuttle.Core.Logging;
using Shuttle.Core.Ninject;
using Shuttle.Core.ServiceHost;
using Shuttle.Core.Threading;
using ILog = Shuttle.Core.Logging.ILog;

namespace Shuttle.Esb.EMail.Server
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            ServiceHost.Run<Host>();
        }
    }

    internal class Host : IServiceHost
    {
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
        private readonly ILog _log;
        private IServiceBus _bus;
        private IKernel _kernel;
        private Task _task;

        public Host()
        {
            var logConfigurationFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "log4net.xml");

            if (File.Exists(logConfigurationFilePath))
            {
                Log.Assign(
                    new Log4NetLog(LogManager.GetLogger(typeof(Program)), new FileInfo(logConfigurationFilePath)));
            }

            _log = Log.For(this);

#if NETCOREAPP
            _log.Information("[framework] : .net core");
#else
            _log.Information("[framework] : .net");
#endif
        }

        public void Start()
        {
            _log.Information("[starting]");

            _kernel = new StandardKernel();

            var container = new NinjectComponentContainer(_kernel);
            var configuration = EMailSection.GetConfiguration();

            container.RegisterInstance(configuration);

            if (!string.IsNullOrWhiteSpace(configuration.EMailClientType))
            {
                var emailClientType = Type.GetType(configuration.EMailClientType,
                    assemblyName =>
                    {
                        Assembly assembly;

                        try
                        {
                            assembly = Assembly.LoadFrom(
                                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, $"{assemblyName.Name}.dll"));
                        }
                        catch (Exception ex)
                        {
                            throw new ApplicationException(string.Format(Resources.AssemblyLoadException, assemblyName.Name, configuration.EMailClientType, ex.Message));
                        }
                        
                        return assembly;
                    },
                    (assembly, typeName, ignore) => assembly == null ?
                        Type.GetType(typeName, false, ignore) :
                        assembly.GetType(typeName, false, ignore)
                );

                if (emailClientType == null)
                {
                    throw new ApplicationException(string.Format(Resources.EMailClientImplementTypeException, configuration.EMailClientType));
                }

                container.Register(typeof(IEMailClient), emailClientType, Lifestyle.Singleton);
            }
            else
            {
                container.Register<IEMailClient, DefaultEMailClient>();
            }

            ServiceBus.Register(container);

            _bus = ServiceBus.Create(container).Start();

            var cancellationToken = _cancellationTokenSource.Token;
            var tracker = container.Resolve<IEMailTracker>();

            _task = Task.Run(() =>
            {
                while (!cancellationToken.IsCancellationRequested)
                {
                    tracker.Expire(configuration.TrackerExpiryDuration);

                    ThreadSleep.While((int) configuration.TrackerExpiryInterval.TotalMilliseconds, cancellationToken);
                }
            });

            _log.Information("[started]");
        }

        public void Stop()
        {
            _cancellationTokenSource.Cancel();

            _task?.Wait();

            _bus?.Dispose();
            _kernel?.Dispose();
        }
    }
}