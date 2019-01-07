using System;
using JetBrains.Annotations;

namespace Saltukkos.Container.Meta
{
    [MeansImplicitUse]
    [BaseTypeRequired(typeof(ISourceControlComponent))]
    public sealed class SourceControlComponentAttribute : Attribute
    {

    }
}