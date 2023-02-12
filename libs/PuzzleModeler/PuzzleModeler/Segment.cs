global using Segment = System.Collections.Generic.List<int>;

namespace PuzzleModeler;

internal static class SegmentExtension
{
    internal static bool IsEmptySegment(this Segment segment)
    {
        return segment.All(i => i == 0);
    }

    internal static int NumberMarkedInSegment(this Segment segment)
    {
        return segment.Count(i => i == 2);
    }
    
}