using JetBrains.Annotations;

namespace Saltukkos.MercurialVS.SourceControl
{
    public interface IConfigurationStorage
    {
        bool SomeFlag { get; set; }
        
        [NotNull]
        string SomeString { get; set; }
    }
}