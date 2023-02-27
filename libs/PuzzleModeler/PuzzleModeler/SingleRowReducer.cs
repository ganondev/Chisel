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

        // face is larger than segment so it's not applicable
        // 5: .#### -> .....
        if (Face > segment.Count())
        {
            segment = from i in segment select 0;
            return true;
        }

        // other segment in row is already marked, this disqualifying this one
        // 2: #0#.### -> #0#....
        if (RowMarked > 0 && segment.ToList().NumberMarkedInSegment() == 0)
        {
            segment = from i in segment select 0;
            return true;
        }

        // face guarantees cover of the center of the segment
        // 3: .#### -> .#00#
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
                segment = segment
                    .Take(margins)
                    .Concat(centerSegment)
                    .Concat(
                        segment.Skip(margins + centerSize)
                    );
                return true;
            }

        }
        
        // TODO could possibly short circuit a false here if there are no existing marked cells in segment
        // 3: ####### -> ####### i.e. no change
        
        // separate segments are bridged together
        // 4: .###0#0### -> .###000###
        if (segment.ToList().FindSplitMarkings(out var indices))
        {
            var set = indices.Take(2).ToList();
            var left = set[0];
            var right = set[1];
            var pre = segment.Take(left);
            var middle = Enumerable.Repeat(2, right - left);
            var post = segment.Skip(right);
            segment = pre.Concat(middle).Concat(post);
            return true;
        }

        // face extends existing marking from left
        // 3: .#0#### -> .#00###
        // 3: .0##### -> .000###
        var leftBit = segment.Take(Face);
        if (leftBit.Any(i => i == 2))
        {
            var leftGap = leftBit.TakeWhile(i => i == 1);
            var toMark = Face - leftGap.Count();
            var adjusted = leftGap.Concat(Enumerable.Repeat(2, toMark));
            if (!leftBit.SequenceEqual(adjusted))
            {
                segment = adjusted.Concat(segment.Skip(Face));
                return true;
            }
            // TODO can short circuit marking from the edge
            // 3: .0##### -> .000...
        }
        
        // face extends existing marking from right
        // 3: .####0# -> .###00#
        // 3: .#####0 -> .###000
        var leftSkip = segment.Count() - Face;
        var rightBit = segment.Skip(leftSkip).Take(Face); 
        if (rightBit.Any(i => i == 2))
        {
            var rightGap = rightBit.Reverse() .TakeWhile(i => i == 1);
            var toMark = Face - rightGap.Count();
            var adjusted = Enumerable.Repeat(2, toMark).Concat(rightGap);
            if (!rightBit.SequenceEqual(adjusted))
            {
                segment = segment.Take(leftSkip).Concat(adjusted);
                return true;
            }
            // TODO can short circuit marking from the edge
        }

        return false;
    }
    
}