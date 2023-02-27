using PuzzleModeler;

namespace PuzzleModelerTests;

public class SegmentTests
{

    [Test]
    public void TestNoSplitMarkingsWhenNoneMarked()
    {
        var segment = L(1, 1, 1, 1, 1);
        Assert.That(segment.FindSplitMarkings(out _), Is.False);
    }
    
    [Test]
    public void TestNoSplitMarkingsWhenOneMarked()
    {
        var segment = L(1, 1, 2, 1, 1);
        Assert.That(segment.FindSplitMarkings(out _), Is.False);
    }
    
    [Test]
    public void TestFoundSplitMarkingsWhenTwoMarked()
    {
        var segment = L(1, 2, 1, 2, 1);
        Assert.That(segment.FindSplitMarkings(out var indices), Is.True);
        Assert.That(indices.Length, Is.EqualTo(2));
        Assert.That(indices[0], Is.EqualTo(1));
        Assert.That(indices[1], Is.EqualTo(3));
    }
    
    [Test]
    public void TestNoSplitMarkingsWhenTwoMarkedAreAdjacent()
    {
        var segment = L(1, 1, 2, 2, 1);
        Assert.That(segment.FindSplitMarkings(out _), Is.False);
    }
    
    // TODO rewrite the split finder to return list of pairs instead of 1D list
    // this is because this can't really be expressed correctly:
    // 0##00##0 -> 0, 4, 7 when it really needs to show that it's 0..3 and 4..7
    // also probably just a better approach

}