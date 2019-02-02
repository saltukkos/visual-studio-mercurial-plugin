using System;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Shell.Interop;
using Saltukkos.Container.Meta;
using Saltukkos.Container.Meta.LifetimeScopes;
using Saltukkos.MercurialVS.SourceControl;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    /// <summary>
    /// Visual Studio creates Tools windows only through
    /// parameterless constructor, so we need this hack.
    /// Use only in windows, that implicitly created by VS
    /// </summary>
    [Component(typeof(PackageScope))]
    public class ToolWindowsDependenciesProvider
    {
        [CanBeNull]
        private static ToolWindowsDependenciesProvider _instance;

        public ToolWindowsDependenciesProvider(
            [NotNull] IConfigurationStorage configurationStorage,
            [NotNull] IDirectoryStateProvider directoryStateProvider,
            [NotNull] IFileHistoryProvider fileHistoryProvider,
            [NotNull] IVsDifferenceService vsDifferenceService)
        {
            FileHistoryProvider = fileHistoryProvider;
            DirectoryStateProvider = directoryStateProvider;
            VsDifferenceService = vsDifferenceService;
            ConfigurationStorage = configurationStorage;
            _instance = this;
        }

        [NotNull]
        public IFileHistoryProvider FileHistoryProvider { get; }

        [NotNull]
        public IDirectoryStateProvider DirectoryStateProvider { get; }

        [NotNull]
        public IConfigurationStorage ConfigurationStorage { get; }

        [NotNull]
        public IVsDifferenceService VsDifferenceService { get; }

        [NotNull]
        public static ToolWindowsDependenciesProvider GetInstance()
        {
            return _instance ?? throw new InvalidOperationException("Container was not init");
        }
    }
}