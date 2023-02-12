global using Segment = System.Collections.Generic.List<int>;

namespace PuzzleModeler;

internal static class SegmentExtension
{
    public static bool IsEmptySegment(this Segment segment)
    {
        return segment.All(i => i == 0);
    }
}