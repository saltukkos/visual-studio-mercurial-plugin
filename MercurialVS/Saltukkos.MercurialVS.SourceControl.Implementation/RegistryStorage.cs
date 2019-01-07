using Microsoft.Win32;
using Saltukkos.Container.Meta;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
    [PackageComponent]
    internal sealed class RegistryStorage : IRegistryStorage
    {
        private const string StoragePath = @"SOFTWARE\Saltukkos\MercuraialVS\";

        public string LoadValue(string key)
        {
            using (var configKey = Registry.CurrentUser.OpenSubKey(StoragePath))
            {
                return configKey?.GetValue(key)?.ToString();
            }
        }

        public void SaveValue(string key, string value)
        {
            using (var configKey = Registry.CurrentUser.CreateSubKey(StoragePath))
            {
                configKey.SetValue(key, value);
            }
        }
    }
}