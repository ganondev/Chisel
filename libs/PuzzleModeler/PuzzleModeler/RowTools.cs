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

        var segments = GetRowSegments(row);
        // var reducer = new SingleRowReducer(face, segments);
        RowReducer reducer = grouping switch
        {
            HintGrouping.One => new SingleRowReducer(face, segments),
            HintGrouping.Two => new SplitRowReducer(face, segments),
            HintGrouping.ThreePlus => new SegmentedRowReducer(face, segments),
            _ => throw new ArgumentException($"Bad grouping {grouping}", nameof(grouping))
        };
        if (reducer.RunReduction(out var update))
        {
            row = AssembleSegments(update);
            return true;
        }

        return false;
        
    }

    private static bool IsEmpty(this int cell) => cell == 0;

    public static List<int>[] GetRowSegments(this List<int> row)
    {

        var section = 0;
        return row.Select( (item,index) =>  new {
            item,
            index = index == 0 || row[index-1].IsEmpty() == item.IsEmpty() ? section : ++section
        })
        .GroupBy(x => x.index, x => x.item)
        .Select(Enumerable.ToList)
        .ToArray();

    }

    public static List<int> AssembleSegments(this List<int>[] segments)
    {

        return segments
            .Cast<IEnumerable<int>>()
            .Aggregate(Enumerable.Concat)
            .ToList();
        
    }
    
}