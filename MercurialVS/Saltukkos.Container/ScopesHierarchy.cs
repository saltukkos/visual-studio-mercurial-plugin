using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Saltukkos.Utils;

namespace Saltukkos.Container
{
    internal sealed class ScopesHierarchy
    {
        [NotNull]
        private readonly IReadOnlyDictionary<Type, List<Type>> _nestedScopes;

        [NotNull]
        private readonly IReadOnlyDictionary<Type, Type> _baseScopes;

        public ScopesHierarchy(
            [NotNull] IReadOnlyDictionary<Type, List<Type>> nestedScopes,
            [NotNull] IReadOnlyDictionary<Type, Type> baseScopes)
        {
            ThrowIf.Null(baseScopes, nameof(baseScopes));
            _nestedScopes = nestedScopes;
            _baseScopes = baseScopes;
        }

        [NotNull]
        public Type GetBaseScope([NotNull] Type scope)
        {
            ThrowIf.Null(scope, nameof(scope));
            return Ensure.NotNull(_baseScopes[scope]);
        }

        [NotNull]
        public IReadOnlyList<Type> GetNestedScopes([NotNull] Type scope)
        {
            ThrowIf.Null(scope, nameof(scope));
            if (_nestedScopes.TryGetValue(scope, out var list))
            {
                return list;
            }

            return new Type[0];
        }
    }
}