using System;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell;
using Saltukkos.MercurialVS.StudioIntegration;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.Package
{
    public class ProvideToolsOptionsPageVisibilityAttribute : RegistrationAttribute
    {
        private Guid SourceControlGuidId { get; } = Guid.Parse(Constants.SourceControlGuid);

        [NotNull]
        private string RegistryPath { get; } =
            $"ToolsOptionsPages\\{Constants.SourceControlCategoryName}\\{Constants.SourceControlProviderName}\\VisibilityCmdUIContexts";

        public override void Register([NotNull] RegistrationContext context)
        {
            ThrowIf.Null(context, nameof(context));
            using (var key = context.CreateKey(RegistryPath))
            {
                key.SetValue(SourceControlGuidId.ToString("B"), 1);
            }
        }

        public override void Unregister([NotNull] RegistrationContext context)
        {
            ThrowIf.Null(context, nameof(context));
            context.RemoveValue(RegistryPath, SourceControlGuidId.ToString("B"));
        }
    }
}