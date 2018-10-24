using System;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell;

namespace StudioIntegrationPackage
{
    public class SourceControlRegistrationAttribute : RegistrationAttribute
    {
        private readonly Guid _packageId;
        private readonly Guid _sourceControlId;
        private readonly Guid _sourceControlServiceId;

        public SourceControlRegistrationAttribute(
            [NotNull] string packageId, 
            [NotNull] string sourceControlId, 
            [NotNull] string sourceControlServiceId)
        {
            _packageId = Guid.Parse(packageId);
            _sourceControlId = Guid.Parse(sourceControlId);
            _sourceControlServiceId = Guid.Parse(sourceControlServiceId);
        }

        public override void Register([NotNull] RegistrationContext context)
        {
            using (var sccProviders = context.CreateKey("SourceControlProviders"))
            {
                using (var sccProviderKey = sccProviders.CreateSubkey(_sourceControlId.ToString("B")))
                {
                    sccProviderKey.SetValue("", "Default name without resources");
                    sccProviderKey.SetValue("Service", _sourceControlServiceId.ToString("B"));
                    using (var sccProviderNameKey = sccProviderKey.CreateSubkey("Name"))
                    {
                        sccProviderNameKey.SetValue("", "some resource key");
                        sccProviderNameKey.SetValue("Package", _packageId.ToString("B"));
                    }
                }
            }
        }

        public override void Unregister([NotNull] RegistrationContext context)
        {
            context.RemoveKey("SourceControlProviders\\" + _sourceControlId.ToString("B"));
        }
    }
}