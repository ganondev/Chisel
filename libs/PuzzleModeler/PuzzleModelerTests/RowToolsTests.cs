using PuzzleModeler;

namespace PuzzleModelerTests;

public class RowToolsTests
{
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestZeroFaceHintClearsRow()
    {
        const HintGrouping grouping = HintGrouping.One;
        const int face = 0;

        var row = Enumerable.Repeat(1, 10).ToList();

        var result = RowTools.ReduceRow((face, grouping), ref row);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(row, Is.EquivalentTo(Enumerable.Repeat(0, 10)));
        });
    }
}