using JetBrains.Annotations;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.HgServices
{
    public enum DiffLineType
    {
        ContextLine,
        MetaInfoLine,
        AddLine,
        RemoveLine
    }

    public struct DiffLine
    {
        public DiffLine([NotNull] string line, DiffLineType diffLineType)
        {
            ThrowIf.Null(line, nameof(line));
            Line = line;
            DiffLineType = diffLineType;
        }

        [NotNull]
        public string Line { get; }

        public DiffLineType DiffLineType { get; }
        
        public override string ToString()
        {
            return Line;
        }
    }
}