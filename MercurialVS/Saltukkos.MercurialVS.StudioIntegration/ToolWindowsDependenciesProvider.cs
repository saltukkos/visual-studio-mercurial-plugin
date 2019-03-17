using System;
using JetBrains.Annotations;
using Saltukkos.Container.Meta;
using Saltukkos.MercurialVS.Architecture;
using Saltukkos.MercurialVS.SourceControl;
using Saltukkos.MercurialVS.StudioIntegration.SolutionFilesStatus;

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
            [NotNull] Func<SolutionFilesStatusViewModel> solutionFilesStatusViewModelFactoryFunc)
        {
            ConfigurationStorage = configurationStorage;
            SolutionFilesStatusViewModelFactoryFunc = solutionFilesStatusViewModelFactoryFunc;
            _instance = this;
        }

        [NotNull] public IConfigurationStorage ConfigurationStorage { get; }

        [NotNull] public Func<SolutionFilesStatusViewModel> SolutionFilesStatusViewModelFactoryFunc { get; }

        [NotNull]
        public static ToolWindowsDependenciesProvider GetInstance()
        {
            return _instance ?? throw new InvalidOperationException("Container was not init");
        }
    }
}