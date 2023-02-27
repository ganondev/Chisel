global using Segment = System.Collections.Generic.List<int>;

namespace PuzzleModeler;

public static class SegmentExtension
{
    internal static bool IsEmptySegment(this Segment segment)
    {
        return segment.All(i => i == 0);
    }

    internal static int NumberMarkedInSegment(this Segment segment)
    {
        return segment.Count(i => i == 2);
    }

    public static bool FindSplitMarkings(this Segment segment, out int[] indices)
    {
        if (segment.NumberMarkedInSegment() < 2)
        {
            indices = new int[] { };
            return false;
        }

        var rawIndices = segment.Select((x, i) => x == 2 ? i : -1).Where(i => i > -1).ToArray();
        var prunedIndices = rawIndices.Where((x, i) =>
        {
            if (i == rawIndices.Length - 1) return true;
            return rawIndices[i + 1] - x != 1;
        }).ToArray();
        
        if (prunedIndices.Length == 1)
        {
            indices = new int[] { };
            return false;
        }

        indices = prunedIndices;
        return true;
    }
    
}