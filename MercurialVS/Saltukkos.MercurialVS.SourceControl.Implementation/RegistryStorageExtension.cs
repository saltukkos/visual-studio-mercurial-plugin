using JetBrains.Annotations;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.SourceControl.Implementation
{
    public static class RegistryStorageExtension
    {
        public static bool? LoadBoolValue([NotNull] this IRegistryStorage storage, [NotNull] string key)
        {
            ThrowIf.Null(storage, nameof(storage));
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
            ThrowIf.Null(storage, nameof(storage));
            ThrowIf.Null(key, nameof(key));
            storage.SaveValue(key, value ? "true" : "false");
        }
    }
}