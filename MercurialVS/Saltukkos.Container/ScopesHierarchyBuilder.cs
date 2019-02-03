using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Saltukkos.Utils;

namespace Saltukkos.Container
{
    internal sealed class ScopesHierarchyBuilder
    {
        [NotNull]
        private readonly Dictionary<Type, List<Type>> _nestedScopes = new Dictionary<Type, List<Type>>();

        [NotNull]
        private readonly Dictionary<Type, Type> _baseScopes = new Dictionary<Type, Type>();

        public void AddScope([NotNull] Type scopeType, [NotNull] Type baseScopeType)
        {
            ThrowIf.Null(scopeType, nameof(scopeType));
            ThrowIf.Null(baseScopeType, nameof(baseScopeType));

            if (_baseScopes.ContainsKey(scopeType))
            {
                throw new InvalidOperationException("Scopes hierarchy must be a tree");
            }

            _baseScopes.Add(scopeType, baseScopeType);

            if (!_nestedScopes.TryGetValue(baseScopeType, out var nestedScopesList))
            {
                nestedScopesList = new List<Type>();
                _nestedScopes.Add(baseScopeType, nestedScopesList);
            }

            nestedScopesList.Add(scopeType);
        }

        [NotNull]
        public ScopesHierarchy Build()
        {
            return new ScopesHierarchy(_nestedScopes, _baseScopes);
        }
    }
}