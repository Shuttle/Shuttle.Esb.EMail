using Shuttle.Core.Container;
using Shuttle.Core.Contract;

namespace Shuttle.Esb.EMail
{
    public class Bootstrap : IComponentRegistryBootstrap
    {
        private static bool _registryBootstrapCalled;

        public void Register(IComponentRegistry registry)
        {
            Guard.AgainstNull(registry, nameof(registry));

            if (_registryBootstrapCalled)
            {
                return;
            }

            registry.AttemptRegister<IEMailTracker, DefaultEMailTracker>();
            registry.AttemptRegister<IEMailAttachmentService, DefaultEMailAttachmentService>();

            _registryBootstrapCalled = true;
        }
    }
}