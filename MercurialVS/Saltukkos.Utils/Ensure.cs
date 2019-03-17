using System.Diagnostics;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Saltukkos.Utils
{
    public static class Ensure
    {
        [ContractAnnotation("value : null => halt")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        [NotNull]
        public static T NotNull<T>([CanBeNull, NoEnumeration] T value)
        {
            if (value == null)
            {
                Debug.Fail("Value is null");
            }

            return value;
        }
    }
}