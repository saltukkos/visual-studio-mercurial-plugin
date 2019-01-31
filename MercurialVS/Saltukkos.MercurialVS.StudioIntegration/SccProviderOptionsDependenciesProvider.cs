using System;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell.Interop;
using Saltukkos.Container.Meta;
using Saltukkos.Container.Meta.LifetimeScopes;
using Saltukkos.MercurialVS.SourceControl;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    /// <summary>
    /// Visual Studio creates Tools Options Page only through
    /// paramerless constructor, so we need this hack
    /// </summary>
    [Component(typeof(PackageScope))]
    public class SccProviderOptionsDependenciesProvider
    {
        [CanBeNull]
        public static SccProviderOptionsDependenciesProvider Instance { get; private set; }

        [NotNull]
        public IDirectoryStateProvider DirectoryStateProvider { get; }

        [NotNull]
        public IConfigurationStorage ConfigurationStorage { get; }

        public IVsDifferenceService VsDifferenceService { get; }

        public SccProviderOptionsDependenciesProvider(
            [NotNull] IConfigurationStorage configurationStorage,
            [NotNull] IDirectoryStateProvider directoryStateProvider, 
            [NotNull] IVsDifferenceService vsDifferenceService)
        {
            DirectoryStateProvider = directoryStateProvider;
            VsDifferenceService = vsDifferenceService;
            ConfigurationStorage = configurationStorage;
            Instance = this;
        }        
    }
}