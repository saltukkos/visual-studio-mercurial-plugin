using System;
using JetBrains.Annotations;
using Saltukkos.Container.Meta.LifetimeScopes;

namespace Saltukkos.Container.Meta
{
    /// <summary>
    /// .Net Framework does not allow generic attributes,
    /// so this is the solution to restrict passed scope type
    /// </summary>
    public abstract class Scope
    {
        protected internal Scope([NotNull] Type scopeType)
        {
            ScopeType = scopeType;
        }

        [NotNull]
        public Type ScopeType { get; }
    }

    public class Scope<T> : Scope
        where T : PackageScope
    {
        public Scope() : base(typeof(T))
        {
        }
    }
}