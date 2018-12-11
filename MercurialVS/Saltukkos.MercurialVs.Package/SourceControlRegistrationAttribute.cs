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

        public SourceControlRegistrationAttribute()
        {
            _packageId = Guid.Parse(Constants.PackageGuid);
            _sourceControlId = Guid.Parse(Constants.SourceControlGuid);
            _sourceControlServiceId = Guid.Parse(Constants.SourceControlServiceGuid);
        }

        public override void Register([NotNull] RegistrationContext context)
        {
            using (var providersKey = context.CreateKey("SourceControlProviders"))
            {
                using (var myProviderKey = providersKey.CreateSubkey(_sourceControlId.ToString("B")))
                {
                    myProviderKey.SetValue("", Constants.SourceControlProviderName);
                    myProviderKey.SetValue("Service", _sourceControlServiceId.ToString("B"));
                    using (var myProviderNameKey = myProviderKey.CreateSubkey("Name"))
                    {
                        //myProviderNameKey.SetValue("", "some key");
                        myProviderNameKey.SetValue("Package", _packageId.ToString("B"));
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