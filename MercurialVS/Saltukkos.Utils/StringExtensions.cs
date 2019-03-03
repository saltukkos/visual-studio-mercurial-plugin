using System;
using JetBrains.Annotations;

namespace Saltukkos.Utils
{
    public static class StringExtensions
    {
        public static bool Contains([NotNull] this string original, [NotNull] string subString, StringComparison comparison)
        {
            ThrowIf.Null(original, nameof(original));
            ThrowIf.Null(subString, nameof(subString));
            return original.IndexOf(subString, comparison) != -1;
        }
    }
}