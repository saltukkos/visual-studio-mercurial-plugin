using System.Runtime.CompilerServices;
using JetBrains.Annotations;

namespace Saltukkos.Utils
{
    public static class ThrowIf
    {
        [ContractAnnotation("value : null => halt")]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Null<T>([CanBeNull, NoEnumeration] T value, [NotNull, InvokerParameterName] string variableName)
        {
            if (value == null)
            {
                ThrowHelper.ThrowNullReferenceException(variableName);
            }
        }
    }
}