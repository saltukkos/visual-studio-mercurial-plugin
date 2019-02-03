using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Saltukkos.Utils;

namespace Saltukkos.Container
{
    internal sealed class ScopeComponentsMap
    {
        [NotNull]
        private readonly IReadOnlyDictionary<Type, List<Type>> _components;

        public ScopeComponentsMap([NotNull] IReadOnlyDictionary<Type, List<Type>> components)
        {
            ThrowIf.Null(components, nameof(components));
            _components = components;
        }

        [ContractAnnotation("=> true, scopeComponents: notnull; => false, scopeComponents: null")]
        public bool TryGetScopeComponents([NotNull] Type scope, out IReadOnlyList<Type> scopeComponents)
        {
            ThrowIf.Null(scope, nameof(scope));
            var success = _components.TryGetValue(scope, out var scopeComponentsList);
            scopeComponents = scopeComponentsList;
            return success;
        }
    }
}