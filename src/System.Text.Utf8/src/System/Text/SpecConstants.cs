namespace System.Text
{
    internal static class SpecConstants
    {
        public const uint HighSurrogateFirstCodePoint = 0xD800;
        public const uint HighSurrogateLastCodePoint = 0xDFFF;
        public const uint LowSurrogateFirstCodePoint = 0xDC00;
        public const uint LowSurrogateLastCodePoint = 0xDFFF;

        public const uint SurrogateRangeStart = HighSurrogateFirstCodePoint;
        public const uint SurrogateRangeEnd = LowSurrogateLastCodePoint;
    }
}
