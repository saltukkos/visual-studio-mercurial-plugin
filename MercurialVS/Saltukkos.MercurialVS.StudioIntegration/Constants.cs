using JetBrains.Annotations;

namespace Saltukkos.MercurialVS.StudioIntegration
{
    public static class Constants
    {
        [NotNull]
        public const string PackageGuid = "FCA36CF1-C45C-4C36-A5D1-A74B0011B175";

        [NotNull]
        public const string SourceControlGuid = "0A91DAD3-1B0B-4026-8F5C-C606118D3693";

        [NotNull]
        public const string SourceControlServiceGuid = "5EB7635D-A0F9-4B8E-ACDF-841CC3790359";

        [NotNull]
        public const string SolutionFilesStatusToolWindowGuid = "0A504052-326E-4110-81F4-98CD2E9B5C48";

        [NotNull]
        public const string FileHistoryToolWindowGuid = "7CCD26C4-A969-4E9B-94E2-E3937282A12E";

        [NotNull]
        public const string OptionsPageGuid = "47AD10C4-93EA-4257-AFF9-7461D5BD5A3A";

        [NotNull]
        public const string CommandSetGuid = "3D262A98-C1C0-429D-A418-2A3DBC3425EA";

        [NotNull]
        public const string SourceControlCategoryName = "Source Control";
        
        [NotNull]
        public const string SourceControlProviderName = "MercurialVS";

        public const int ShowSolutionFileStatusToolWindowCommandId = 0x0100;

        public const int ShowCurrentFileLogCommandId = 0x0101;
    }
}