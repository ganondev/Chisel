namespace PuzzleModeler;

public enum HintGrouping
{
    One,
    Two,
    ThreePlus,
}

public static class RowTools
{
    
    public static bool ReduceRow((int, HintGrouping) hint, ref List<int> row)
    {
		
        var (face, grouping) = hint;
        IEnumerable<int>? updated = null;
		
        if (face == 0)
        {
            updated = from i in row select 0;
            goto done;
        }

        var rowSize = row.Count(i => i > 0);
        if (face == rowSize)
        {
            // mark any remaining cells
            updated = from i in row select i == 0 ? 0 : 2;
            goto done;
        }

        if (grouping == HintGrouping.One)
        {

            if (face > rowSize / 2)
            {
                var margins = rowSize - face;

                var segmentSize = rowSize - margins * 2;
                var segment = row.Skip(margins).Take(segmentSize);
                if (segment.Any(i => i == 1))
                {
                    segment = Enumerable.Repeat(2, segmentSize);
                    updated = row.Take(margins).Concat(segment).Concat(row.Skip(margins + segmentSize));
                    goto done;
                }

            }
			
        }
        
        done:

        if (updated != null)
        {
            row = updated.ToList();
            return true;
        }
        return false;
    }
    
}