using JetBrains.Annotations;
using Saltukkos.Container.Meta;

namespace Saltukkos.MercurialVS.SourceControl
{
    public interface IConfigurationStorage : IPackageComponent
    {
        bool SomeFlag { get; set; }
        
        [NotNull]
        string SomeString { get; set; }
    }
}