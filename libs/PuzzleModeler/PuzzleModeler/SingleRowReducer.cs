using System.Collections;

namespace PuzzleModeler;

public class SingleRowReducer : RowReducer
{
    public SingleRowReducer(int face, Segment[] segments) : base(face, segments)
    {
    }

    internal override bool Reduce(in Segment[] segments)
    {

        if (base.Reduce(in segments))
        {
            return true;
        }
        
        if (Face == 0)
        {
            for (var index = 0; index < segments.Length; index++)
            {
                if (segments[index].IsEmptySegment())
                    continue;
                segments[index] = (from i in segments[index] select 0).ToList();
            }
            return true;
        }
        
        for (var i = 0; i < segments.Length; i++)
        {
            if (segments[i].IsEmptySegment())
                continue;
            if (ReduceSegment(ref segments[i]))
                return true;
        }
        return false;
    }

    private bool ReduceSegment(ref Segment segment)
    {
        IEnumerable<int> input = segment;
        if (ApplyReduction(ref input))
        {
            // if output and input are same no change happened
            if (!input.Equals(segment))
            {
                segment = input.ToList();
                return true;
            }
            return false;
        }
        return false;
    }

    private bool ApplyReduction(ref IEnumerable<int> segment)
    {

        if (Face > segment.Count())
        {
            segment = from i in segment select 0;
            return true;
        }

        if (Face > RowRemaining / 2)
        {
            var margins = RowRemaining - Face;

            var centerSize = RowRemaining - margins * 2;
            var centerSegment = segment
                .Skip(margins)
                .Take(centerSize);
            if (centerSegment.Any(i => i == 1))
            {
                centerSegment = Enumerable.Repeat(2, centerSize);
                segment = segment.Take(margins).Concat(centerSegment).Concat(segment.Skip(margins + centerSize));
                return true;
            }

        }

        return false;
    }
    
}