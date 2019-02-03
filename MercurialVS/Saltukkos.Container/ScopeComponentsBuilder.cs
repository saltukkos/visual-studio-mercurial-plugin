using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Saltukkos.Utils;

namespace Saltukkos.Container
{
    internal sealed class ScopeComponentsMapBuilder
    {
        [NotNull]
        private readonly Dictionary<Type, List<Type>> _components = new Dictionary<Type, List<Type>>();

        public void AddComponentToScope([NotNull] Type componentType, [NotNull] Type scopeType)
        {
            ThrowIf.Null(componentType, nameof(componentType));
            ThrowIf.Null(scopeType, nameof(scopeType));
            
            if (!_components.TryGetValue(scopeType, out var list))
            {
                list = new List<Type>();
                _components.Add(scopeType, list);
            }

            list.Add(componentType);
        }

        [NotNull]
        public ScopeComponentsMap Build()
        {
            return new ScopeComponentsMap(_components);
        }
    }
}