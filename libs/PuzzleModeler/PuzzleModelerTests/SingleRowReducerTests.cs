using PuzzleModeler;

namespace PuzzleModelerTests;

public class SingleRowReducerTests
{

    [Test]
    public void ZeroFaceClearsAllSegments()
    {

        var reducer = new SingleRowReducer(0, new []
        {
            L(1, 1, 1),
            L(0),
            L(1, 1, 1, 1)
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(reducer.RunReduction(out var result), Is.True);
            Assert.That(result[0], Is.EquivalentTo(L(0, 0, 0)));
            Assert.That(result[1], Is.EquivalentTo(L(0)));
            Assert.That(result[2], Is.EquivalentTo(L(0, 0, 0, 0)));
        });

    }

    [Test]
    public void DoesNotCenterFillMultipleValidPossibleSegments()
    {
        var reducer = new SingleRowReducer(3, new []
        {
            L(1, 1, 1),
            L(0),
            L(1, 1, 1, 1)
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(reducer.RunReduction(out var result), Is.False);
            Assert.That(result[0], Is.EquivalentTo(L(1, 1, 1)));
            Assert.That(result[1], Is.EquivalentTo(L(0)));
            Assert.That(result[2], Is.EquivalentTo(L(1, 1, 1, 1)));
        });
    }

    [Test]
    public void RemovesSegmentThatIsTooSmall()
    {
        var reducer = new SingleRowReducer(4, new []
        {
            L(1, 1, 1),
            L(0),
            L(1, 1, 1, 1)
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(reducer.RunReduction(out var result), Is.True);
            Assert.That(result[0], Is.EquivalentTo(L(0, 0, 0)));
            Assert.That(result[1], Is.EquivalentTo(L(0)));
            Assert.That(result[2], Is.EquivalentTo(L(1, 1, 1, 1)));
        });
    }

    [Test]
    public void TestNoMarkingIfMultipleValidSegments()
    {
        var reducer = new SingleRowReducer(2, new []
        {
            L(1, 1, 1),
            L(0),
            L(1, 1)
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(reducer.RunReduction(out var result), Is.False);
            Assert.That(result[0], Is.EquivalentTo(L(1, 1, 1)));
            Assert.That(result[1], Is.EquivalentTo(L(0)));
            Assert.That(result[2], Is.EquivalentTo(L(1, 1)));
        });
        
    }
    
    [Test]
    public void TestOnlyMarkedSegmentRemains()
    {
        var reducer = new SingleRowReducer(2, new []
        {
            L(1, 2, 1),
            L(0),
            L(1, 1)
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(reducer.RunReduction(out var result), Is.True);
            Assert.That(result[0], Is.EquivalentTo(L(1, 2, 1)));
            Assert.That(result[1], Is.EquivalentTo(L(0)));
            Assert.That(result[2], Is.EquivalentTo(L(0, 0)));
        });
        
    }
    
    [Test]
    public void TestMarkingExtendedFromLeftSideWithGap()
    {
        var reducer = new SingleRowReducer(3, new []
        {
            L(1, 2, 1, 1, 1, 1, 1, 1, 1, 1)
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(reducer.RunReduction(out var result), Is.True);
            Assert.That(result[0], Is.EquivalentTo(L(1, 2, 2, 1, 1, 1, 1, 1, 1, 1)));
        });
        
    }
    
    [Test]
    public void TestMarkingExtendedFromLeftSideNoGap()
    {
        var reducer = new SingleRowReducer(3, new []
        {
            L(2, 1, 1, 1, 1, 1, 1, 1, 1, 1)
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(reducer.RunReduction(out var result), Is.True);
            Assert.That(result[0], Is.EquivalentTo(L(2, 2, 2, 1, 1, 1, 1, 1, 1, 1)));
        });
        
    }
    
    [Test]
    public void TestMarkingExtendedFromRightSideWithGap()
    {
        var reducer = new SingleRowReducer(3, new []
        {
            L(1, 1, 1, 1, 1, 1, 1, 1, 2, 1)
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(reducer.RunReduction(out var result), Is.True);
            Assert.That(result[0], Is.EquivalentTo(L(1, 1, 1, 1, 1, 1, 1, 2, 2, 1)));
        });
        
    }
    
    [Test]
    public void TestMarkingExtendedFromRightSideNoGap()
    {
        var reducer = new SingleRowReducer(3, new []
        {
            L(1, 1, 1, 1, 1, 1, 1, 1, 1, 2)
        });
        
        Assert.Multiple(() =>
        {
            Assert.That(reducer.RunReduction(out var result), Is.True);
            Assert.That(result[0], Is.EquivalentTo(L(1, 1, 1, 1, 1, 1, 1, 2, 2, 2)));
        });
        
    }
    
}