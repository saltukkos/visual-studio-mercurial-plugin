using System;
using JetBrains.Annotations;

namespace Saltukkos.Container.Meta
{
    [MeansImplicitUse]
    public sealed class ComponentAttribute : Attribute
    {
        public ComponentAttribute([NotNull] Type scopeType)
        {
            ScopeType = scopeType;
        }

        [NotNull] 
        public Type ScopeType { get; }
    }
}