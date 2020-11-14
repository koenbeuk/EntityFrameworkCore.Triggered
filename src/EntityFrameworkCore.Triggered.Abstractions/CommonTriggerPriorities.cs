namespace EntityFrameworkCore.Triggered
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE1006:Naming Styles", Justification = "Intended public constants")]
    public static class CommonTriggerPriority
    {
        public const int Earlier = -2;
        public const int Early = -1;
        public const int Normal = 0;
        public const int Late = 1;
        public const int Later = 2;
    }
}
