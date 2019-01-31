using JetBrains.Annotations;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
    public interface IRegistryStorage
    {
        [CanBeNull]
        string LoadValue([NotNull] string key);

        void SaveValue([NotNull] string key, [NotNull] string value);
    }
}