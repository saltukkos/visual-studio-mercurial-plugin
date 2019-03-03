using JetBrains.Annotations;

namespace Saltukkos.MercurialVS.HgServices
{
    public struct ChangeSet
    {
        [NotNull]
        public string CommitMessage { get; set; }
        
        [NotNull]
        public string Branch { get; set; }
        
        [NotNull]
        public string Author { get; set; }
        
        [NotNull]
        public string RevisionHash { get; set; }
        
        public int RevisionNumber { get; set; }
    }
}