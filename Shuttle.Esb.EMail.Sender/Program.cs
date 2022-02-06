using Ninject;
using Shuttle.Core.Ninject;
using Shuttle.Esb.EMail.Messages;

namespace Shuttle.Esb.EMail.Sender
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var container = new NinjectComponentContainer(new StandardKernel());

            container.RegisterServiceBus();

            using (var bus = container.Resolve<IServiceBus>().Start())
            {
                var message = new SendEMailCommand
                {
                    FromAddress = new SendEMailCommand.Address
                    {
                        EMailAddress = "me@ebenroux.co.za"
                    },
                    Subject = "Sending using Shuttle.Esb.EMail is super-easy!",
                    Body = "Hello World!"
                };
                
                message.ToAddresses.Add(new SendEMailCommand.Address
                {
                    EMailAddress = "me@ebenroux.co.za"
                });
                
                bus.Send(message);
            }
        }
    }
}