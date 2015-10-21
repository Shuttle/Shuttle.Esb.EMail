using System;
using Castle.MicroKernel.Registration;
using Castle.Windsor;
using log4net;
using Shuttle.Core.Data;
using Shuttle.Core.Host;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.Infrastructure.Log4Net;
using Shuttle.ESB.Castle;
using Shuttle.ESB.Core;
using Shuttle.ESB.Modules;
using Shuttle.ESB.SqlServer;

namespace Shuttle.EMail.Server
{
	public class ServiceBusHost : IHost, IDisposable
	{
		private readonly WindsorContainer _container = new WindsorContainer();

		private IServiceBus _bus;

		public void Dispose()
		{
			Log.Information("E-Mail Server Stopped.");

			_bus.AttemptDispose();
		}

		public void Start()
		{
			Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof (ServiceBusHost))));

			new ConnectionStringService().Approve();

			Log.Information("E-Mail Server Started.");

			_container.Register(Component.For<IEMailConfiguration>().ImplementedBy<EMailConfiguration>());
			_container.Register(Component.For<IEMailService>().ImplementedBy<EMailService>());

			_container.Register(
				Classes
					.FromThisAssembly()
					.BasedOn(typeof (IMessageHandler<>))
					.WithServiceFirstInterface());

			_bus = ServiceBus
				.Create(c =>
				{
					c.MessageHandlerFactory(new CastleMessageHandlerFactory(_container));
					c.SubscriptionManager(SubscriptionManager.Default());
					c.AddModule(new ActiveTimeRangeModule());
				});
		}
	}
}