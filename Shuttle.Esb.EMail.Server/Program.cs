using System;
using System.IO;
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
    class Program
    {
        static void Main(string[] args)
        {
            ServiceHost.Run<Host>();
        }
    }

    internal class Host : IServiceHost
    {
        private readonly ILog _log;
        private IServiceBus _bus;
        private IKernel _kernel;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();
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
                container.Register(typeof(IEMailClient), Type.GetType(configuration.EMailClientType),
                    Lifestyle.Singleton);
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

                    ThreadSleep.While((int)configuration.TrackerExpiryInterval.TotalMilliseconds, cancellationToken);
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
