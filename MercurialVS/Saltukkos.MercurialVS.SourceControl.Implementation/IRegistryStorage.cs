using JetBrains.Annotations;
using Saltukkos.Container.Meta;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
    public interface IRegistryStorage : IPackageComponent
    {
        [CanBeNull]
        string LoadValue([NotNull] string key);

        void SaveValue([NotNull] string key, [NotNull] string value);
    }
}