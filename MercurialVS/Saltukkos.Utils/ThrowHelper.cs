using System;
using JetBrains.Annotations;

namespace Saltukkos.Utils
{
    internal static class ThrowHelper
    {
        public static void ThrowNullReferenceException([NotNull] string variableName)
        {
            throw new NullReferenceException($"variable {variableName} is null");
        }
    }
}