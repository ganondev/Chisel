using PuzzleModeler;

namespace PuzzleModelerTests;

public class RowReducerTests
{

    [Test]
    public void RowCompletedByRemainingCellsMatchingFaceValue()
    {

        var reducer = new SegmentedRowReducer(6, new []
        {
            L(1, 1),
            L(0),
            L(1),
            L(0),
            L(1, 1, 1),
            L(0, 0)
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(reducer.RunReduction(out var result), Is.True);
            Assert.That(result[0], Is.EquivalentTo(L(2, 2)));
            Assert.That(result[1], Is.EquivalentTo(L(0)));
            Assert.That(result[2], Is.EquivalentTo(L(2)));
            Assert.That(result[3], Is.EquivalentTo(L(0)));
            Assert.That(result[4], Is.EquivalentTo(L(2, 2, 2)));
            Assert.That(result[5], Is.EquivalentTo(L(0, 0)));
        });

    }

    [Test]
    public void RowCompletedByMarkedCellsMatchingFaceValue()
    {
        var reducer = new SegmentedRowReducer(4, new []
        {
            L(1, 2),
            L(0),
            L(2),
            L(0),
            L(2, 1, 2),
            L(0, 0)
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(reducer.RunReduction(out var result), Is.True);
            Assert.That(result[0], Is.EquivalentTo(L(0, 2)));
            Assert.That(result[1], Is.EquivalentTo(L(0)));
            Assert.That(result[2], Is.EquivalentTo(L(2)));
            Assert.That(result[3], Is.EquivalentTo(L(0)));
            Assert.That(result[4], Is.EquivalentTo(L(2, 0, 2)));
            Assert.That(result[5], Is.EquivalentTo(L(0, 0)));
        });
    }

}