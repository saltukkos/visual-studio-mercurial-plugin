using System;
using JetBrains.Annotations;
using Saltukkos.Utils;

namespace Saltukkos.Container.Meta
{
    [BaseTypeRequired(typeof(ILifeTimeScope<>))]
    public class LifetimeScopeAttribute : Attribute
    {
        public LifetimeScopeAttribute([NotNull] Type baseScopeType)
        {
            BaseScopeType = baseScopeType;
            ThrowIf.Null(baseScopeType, nameof(baseScopeType));
        }

        [NotNull] 
        public Type BaseScopeType { get; }
    }
}