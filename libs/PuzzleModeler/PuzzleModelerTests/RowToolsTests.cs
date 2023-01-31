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

    private static IEnumerable<TestCaseData> SingleGroupFullMarkTestCases()
    {
        
        for (var rowLength = 10; rowLength > 0; rowLength--)
        {
            for (var face = rowLength; face > 0; face--)
            {
                var margin = rowLength - face;
                for (var offset = margin; offset > 0; offset--)
                {

                    var input =
                        Enumerable.Repeat(0, offset)
                            .Concat(
                                Enumerable.Repeat(1, face)
                            )
                            .Concat(
                                Enumerable.Repeat(0, margin - offset)
                            ).ToList();

                    var expected = (from i in input select i == 1 ? 2 : 0).ToList();
                    var caseData = new TestCaseData(face, input, expected);
                    caseData.SetName(
                        $"{nameof(TestSingleGroupFaceMatchingRowSizeIsFullyMarked)}(face={face}, rowLength={rowLength}, offset={offset})");
                    yield return caseData;

                }
            }
        }

    }
    
    [Test, TestCaseSource(nameof(SingleGroupFullMarkTestCases))]
    public void TestSingleGroupFaceMatchingRowSizeIsFullyMarked(int face, List<int> input, List<int> expected)
    {
        const HintGrouping grouping = HintGrouping.One;

        var result = RowTools.ReduceRow((face, grouping), ref input);
        Assert.Multiple(() =>
        {
            Assert.That(result, Is.True);
            Assert.That(input, Is.EquivalentTo(expected));
        });
    }
}