using System;
using Castle.Windsor;
using log4net;
using Shuttle.Core.Data;
using Shuttle.Core.Host;
using Shuttle.Core.Infrastructure;
using Shuttle.Core.Infrastructure.Castle;
using Shuttle.Core.Infrastructure.Log4Net;
using Shuttle.ESB.Castle;
using Shuttle.ESB.Core;
using Shuttle.ESB.Modules.ActiveTimeRange;
using Shuttle.ESB.SqlServer;

namespace Shuttle.EMail.Server
{
	public class ServiceBusHost : IHost, IDisposable
	{
		private readonly WindsorContainer container = new WindsorContainer();

		private IServiceBus bus;

		public void Dispose()
		{
			Log.Information("E-Mail Server Stopped.");

			bus.AttemptDispose();
		}

		public void Start()
		{
			Log.Assign(new Log4NetLog(LogManager.GetLogger(typeof(ServiceBusHost))));

			ConnectionStrings.Approve();

			Log.Information("E-Mail Server Started.");

			container.RegisterSingleton<IEMailGateway, EMailGateway>();
			container.RegisterSingleton<IEMailConfiguration, EMailConfiguration>();
			container.RegisterTransient<IMessageHandler>("Shuttle.EMail.Server");

			bus = ServiceBus
				.Create()
				.MessageHandlerFactory(new CastleMessageHandlerFactory(container))
				.SubscriptionManager(SubscriptionManager.Default())
				.AddModule(new ActiveTimeRangeModule())
				.Start();
		}
	}
}