using System;
using JetBrains.Annotations;
using Saltukkos.Utils;

namespace Saltukkos.Container.Meta
{
    [MeansImplicitUse]
    public sealed class ComponentAttribute : Attribute
    {
        public ComponentAttribute([NotNull] Type scopeType)
        {
            ThrowIf.Null(scopeType, nameof(scopeType));
            ScopeType = scopeType;
        }

        [NotNull] 
        public Type ScopeType { get; }
    }
}