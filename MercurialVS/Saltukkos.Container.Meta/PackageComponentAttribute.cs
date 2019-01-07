using System;
using JetBrains.Annotations;

namespace Saltukkos.Container.Meta
{
    [MeansImplicitUse]
    [BaseTypeRequired(typeof(IPackageComponent))]
    public sealed class PackageComponentAttribute : Attribute
    {

    }
}