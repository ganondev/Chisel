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
    
}