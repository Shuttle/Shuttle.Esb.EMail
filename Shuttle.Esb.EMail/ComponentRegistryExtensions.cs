using Shuttle.Core.Container;
using Shuttle.Core.Contract;

namespace Shuttle.Esb.EMail
{
    public static class ComponentRegistryExtensions
    {
        public static void RegisterEMail(this IComponentRegistry registry)
        {
            Guard.AgainstNull(registry, nameof(registry));

            registry.AttemptRegister<IEMailTracker, DefaultEMailTracker>();
            registry.AttemptRegister<IEMailAttachmentService, DefaultEMailAttachmentService>();
        }
    }
}