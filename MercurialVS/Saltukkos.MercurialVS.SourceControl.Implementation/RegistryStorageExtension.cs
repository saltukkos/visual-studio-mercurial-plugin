using JetBrains.Annotations;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
    public static class RegistryStorageExtension
    {
        public static bool? LoadBoolValue([NotNull] this IRegistryStorage storage, [NotNull] string key)
        {
            var stringValue = storage.LoadValue(key);
            switch (stringValue)
            {
                case "true":
                    return true;
                case "false":
                    return false;
                default:
                    return null;
            }
        }

        public static void SaveBoolValue([NotNull] this IRegistryStorage storage, [NotNull] string key, bool value)
        {
            storage.SaveValue(key, value ? "true" : "false");
        }
    }
}