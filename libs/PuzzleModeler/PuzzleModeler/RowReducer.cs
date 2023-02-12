namespace PuzzleModeler;

public abstract class RowReducer
{
    private protected int Face { get; }
    private Segment[] Segments { get; }

    private protected int RowRemaining =>
        (from segment in Segments
            where !segment.IsEmptySegment()
            select segment.Count).Sum();

    private bool IsComplete =>
        Face == RowRemaining;

    internal RowReducer(int face, Segment[] segments)
    {
        Face = face;
        Segments = segments;
    }

    public bool RunReduction(out Segment[] result)
    {
        var segments = Segments;
        result = segments;
        return Reduce(in segments);
    }

    internal virtual bool Reduce(in Segment[] segments)
    {

        if (IsComplete)
        {
            for (var index = 0; index < segments.Length; index++)
            {
                if (!segments[index].IsEmptySegment())
                    segments[index] = 
                        (from i in segments[index] select i == 0 ? 0 : 2)
                        .ToList();
            }
            return true;
        }

        return false;

    }

}