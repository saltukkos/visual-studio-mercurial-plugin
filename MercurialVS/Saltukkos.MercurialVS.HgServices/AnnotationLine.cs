using JetBrains.Annotations;
using Saltukkos.Utils;

namespace Saltukkos.MercurialVS.HgServices
{
    public struct AnnotationLine
    {
        public AnnotationLine([NotNull] string line, int lineNumber, int revision)
        {
            ThrowIf.Null(line, nameof(line));
            Line = line;
            LineNumber = lineNumber;
            Revision = revision;
        }

        [NotNull]
        public string Line { get; }

        public int LineNumber { get; }
        
        public int Revision { get; }

        public override string ToString()
        {
            return Line;
        }
    }
}