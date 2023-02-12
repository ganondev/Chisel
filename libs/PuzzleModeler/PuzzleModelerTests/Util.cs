namespace PuzzleModelerTests;

public static class Util
{
    public static List<int> L(params int[] values)
    {
        return new List<int>(values);
    }

    public static List<int> S(int value, int length)
    {
        return Enumerable.Repeat(value, length).ToList();
    }
}