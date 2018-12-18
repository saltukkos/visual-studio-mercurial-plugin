using JetBrains.Annotations;
using Saltukkos.Container.Meta;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
    [Component]
    public sealed class ConfigurationStorage : IConfigurationStorage
    {
        [NotNull]
        private readonly IRegistryStorage _storage;

        public ConfigurationStorage([NotNull] IRegistryStorage storage)
        {
            _storage = storage;
        }

        public bool SomeFlag
        {
            get { return _storage.LoadBoolValue(nameof(SomeFlag)) ?? true; }
            set { _storage.SaveBoolValue(nameof(SomeFlag), value); }
        }

        public string SomeString
        {
            get { return _storage.LoadValue(nameof(SomeString)) ?? "Some default value"; }
            set { _storage.SaveValue(nameof(SomeString), value); }
        }
    }
}