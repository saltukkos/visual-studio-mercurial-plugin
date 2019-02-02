using Microsoft.Win32;
using Saltukkos.Container.Meta;
using Saltukkos.Container.Meta.LifetimeScopes;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
    [Component(typeof(PackageScope))]
    internal sealed class RegistryStorage : IRegistryStorage
    {
        private const string StoragePath = @"SOFTWARE\Saltukkos\MercuraialVS\";

        public string LoadValue(string key)
        {
            ThrowIf.Null(key, nameof(key));
            using (var configKey = Registry.CurrentUser.OpenSubKey(StoragePath))
            {
                return configKey?.GetValue(key)?.ToString();
            }
        }

        public void SaveValue(string key, string value)
        {
            ThrowIf.Null(value, nameof(value));
            ThrowIf.Null(key, nameof(key));
            using (var configKey = Registry.CurrentUser.CreateSubKey(StoragePath))
            {
                configKey.SetValue(key, value);
            }
        }
    }
}