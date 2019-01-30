using System;
using System.Diagnostics;
using System.Linq;
using JetBrains.Annotations;

namespace Saltukkos.Container.Meta
{
    public sealed class ComponentAttribute : Attribute
    {
        public ComponentAttribute([NotNull] Scope scope)
        {
            var scopeGenericType = scope.GetType();
            var scopeType = scopeGenericType.GenericTypeArguments?.FirstOrDefault();
            Debug.Assert(scopeType != null, nameof(scopeType) + " != null");
            ScopeType = scopeType;
        }

        [NotNull] 
        public Type ScopeType { get; }
    }
}