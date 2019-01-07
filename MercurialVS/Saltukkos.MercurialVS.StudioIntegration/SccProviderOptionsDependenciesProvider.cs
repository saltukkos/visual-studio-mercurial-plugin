using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.SourceControl;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    /// <summary>
    /// Visual Studio creates Tools Options Page only through
    /// paramerless constructor, so we need this hack
    /// </summary>
    [PackageComponent]
    public class SccProviderOptionsDependenciesProvider : IPackageComponent
    {
        [CanBeNull]
        public static SccProviderOptionsDependenciesProvider Instance { get; private set; }

        [NotNull]
        public IConfigurationStorage ConfigurationStorage { get; }

        public SccProviderOptionsDependenciesProvider([NotNull] IConfigurationStorage configurationStorage)
        {
            ConfigurationStorage = configurationStorage;
            Instance = this;
        }
    }
}