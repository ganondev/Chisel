using PuzzleModeler;
using HG = PuzzleModeler.HintGrouping;

namespace PuzzleModelerTests;

public class RowToolsTests
{
    private static void ReduceAndAssertLists(List<int> input, int face, HG grouping, List<int> expected)
    {
        
        Assert.Multiple(() =>
        {
            Assert.That(RowTools.ReduceRow((face, grouping), ref input), Is.True);
            Assert.That(input, Is.EquivalentTo(expected));
        });
        
    }

    private static List<int> L(params int[] values)
    {
        return new List<int>(values);
    }

    private static List<int> S(int value, int length)
    {
        return Enumerable.Repeat(value, length).ToList();
    }
    
    [SetUp]
    public void Setup()
    {
    }

    [Test]
    public void TestZeroFaceHintClearsRow()
    {

        ReduceAndAssertLists(
            Enumerable.Repeat(1, 10).ToList(),
            0,
            HG.One,
            Enumerable.Repeat(0, 10).ToList()
        );
        
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
        ReduceAndAssertLists(
            input,
            face,
            HG.One,
            expected
        );
    }

    // TODO parameterize
    [Test]
    public void TestDoubleGroupFaceMatchingRowSizeIsFullyMarked()
    {

        ReduceAndAssertLists(
            L( 1, 1, 0, 0, 0, 0, 0, 1, 1, 1 ),
            5,
            HG.Two,
            L( 2, 2, 0, 0, 0, 0, 0, 2, 2, 2 )
        );
        
    }

    // TODO parameterize
    [Test]
    public void TestMultipleGroupFaceMatchingRowSizeIsFullyMarked()
    {

        ReduceAndAssertLists(
            L( 1, 1, 0, 1, 1, 0, 0, 1, 1, 1 ),
            7,
            HG.ThreePlus,
            L( 2, 2, 0, 2, 2, 0, 0, 2, 2, 2 )
        );
        
    }

    [Test]
    public void TestOddFaceGuaranteeingCenterCoverIsMarkedForFullRowOfEvenSize()
    {

        ReduceAndAssertLists(
            L( 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 ),
            9,
            HG.One,
            L( 1, 2, 2, 2, 2, 2, 2, 2, 2, 1 )
        );

    }
    
    [Test]
    public void TestEvenFaceGuaranteeingCenterCoverIsMarkedForFullRowOfEvenSize()
    {

        ReduceAndAssertLists(
            L( 1, 1, 1, 1, 1, 1, 1, 1, 1, 1 ),
            6,
            HG.One,
            L( 1, 1, 1, 1, 2, 2, 1, 1, 1, 1 )
        );

    }
    
    [Test]
    public void TestOddFaceGuaranteeingCenterCoverIsMarkedForFullRowOfOddSize()
    {

        ReduceAndAssertLists(
            L( 1, 1, 1, 1, 1, 1, 1, 1, 1 ),
            7,
            HG.One,
            L( 1, 1, 2, 2, 2, 2, 2, 1, 1 )
        );

    }
    
    [Test]
    public void TestEvenFaceGuaranteeingCenterCoverIsMarkedForFullRowOfOddSize()
    {

        ReduceAndAssertLists(
            L( 1, 1, 1 ),
            2,
            HG.One,
            L( 1, 2, 1 )
        );
        
        ReduceAndAssertLists(
            L( 1, 1, 1, 1, 1, 1, 1, 1, 1),
            8,
            HG.One,
            L( 1, 2, 2, 2, 2, 2, 2, 2, 1 )
        );

    }

    [Test]
    public void TestReturnsFullUnmarkedRowAsOneSegment()
    {

        var input = L(1, 1, 1, 1, 1);
        var result = RowTools.GetRowSegments(input);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Length.EqualTo(1));
            Assert.That(result[0], Is.EquivalentTo(input));
        });
        
    }
    
    [Test]
    public void TestReturnsFullPartiallyMarkedRowAsOneSegment()
    {

        var input = L(1, 2, 1, 2, 1);
        var result = RowTools.GetRowSegments(input);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Length.EqualTo(1));
            Assert.That(result[0], Is.EquivalentTo(input));
        });
        
    }
    
    [Test]
    public void TestReturnsEmptyRowAsOneSegment()
    {

        var input = L(0, 0, 0, 0, 0);
        var result = RowTools.GetRowSegments(input);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Length.EqualTo(1));
            Assert.That(result[0], Is.EquivalentTo(input));
        });
        
    }

    [Test]
    public void TestReturnsTwoSegmentsWhenLeftIsEmptySingle()
    {
        
        var input = L(0, 2, 1, 2, 1);
        var result = RowTools.GetRowSegments(input);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That(result[0], Is.EquivalentTo(L(0)));
            Assert.That(result[1], Is.EquivalentTo(L(2, 1, 2, 1)));
        });
        
    }
    
    [Test]
    public void TestReturnsTwoSegmentsWhenRightIsEmptySingle()
    {
        
        var input = L(2, 1, 2, 1, 0);
        var result = RowTools.GetRowSegments(input);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That(result[0], Is.EquivalentTo(L(2, 1, 2, 1)));
            Assert.That(result[1], Is.EquivalentTo(L(0)));
        });
        
    }
    
    [Test]
    public void TestReturnsTwoSegmentsWhenLeftIsEmptyMultiple()
    {
        
        var input = L(0, 0, 0, 0, 1);
        var result = RowTools.GetRowSegments(input);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That(result[0], Is.EquivalentTo(S(0, 4)));
            Assert.That(result[1], Is.EquivalentTo(L(1)));
        });
        
    }
    
    [Test]
    public void TestReturnsTwoSegmentsWhenRightIsEmptyMultiple()
    {
        
        var input = L(1, 0, 0, 0, 0);
        var result = RowTools.GetRowSegments(input);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Length.EqualTo(2));
            Assert.That(result[0], Is.EquivalentTo(L(1)));
            Assert.That(result[1], Is.EquivalentTo(S(0, 4)));
        });
        
    }
    
    [Test]
    public void TestReturnsThreeSegmentsWhenCenterIsEmptySingle()
    {
        
        var input = L(1, 2, 0, 1, 2);
        var result = RowTools.GetRowSegments(input);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Length.EqualTo(3));
            Assert.That(result[0], Is.EquivalentTo(L(1, 2)));
            Assert.That(result[1], Is.EquivalentTo(L(0)));
            Assert.That(result[2], Is.EquivalentTo(L(1, 2)));
        });
        
    }
    
    [Test]
    public void TestReturnsThreeSegmentsWhenCenterIsEmptyMultiple()
    {
        
        var input = L(1, 0, 0, 0, 2);
        var result = RowTools.GetRowSegments(input);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Length.EqualTo(3));
            Assert.That(result[0], Is.EquivalentTo(L(1)));
            Assert.That(result[1], Is.EquivalentTo(L(0, 0, 0)));
            Assert.That(result[2], Is.EquivalentTo(L(2)));
        });
        
    }
    
    [Test]
    public void TestReturnsThreeSegmentsWhenSidesAreEmpty()
    {
        
        var input = L(0, 1, 2, 0, 0);
        var result = RowTools.GetRowSegments(input);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Length.EqualTo(3));
            Assert.That(result[0], Is.EquivalentTo(L(0)));
            Assert.That(result[1], Is.EquivalentTo(L(1, 2)));
            Assert.That(result[2], Is.EquivalentTo(L(0, 0)));
        });
        
    }
    
    [Test]
    public void TestReturnsAllSegmentsFromListOfSingles()
    {
        
        var input = L(0, 1, 0, 2, 0);
        var result = RowTools.GetRowSegments(input);
        
        Assert.Multiple(() =>
        {
            Assert.That(result, Has.Length.EqualTo(5));
            Assert.That(result[0], Is.EquivalentTo(L(0)));
            Assert.That(result[1], Is.EquivalentTo(L(1)));
            Assert.That(result[2], Is.EquivalentTo(L(0)));
            Assert.That(result[3], Is.EquivalentTo(L(2)));
            Assert.That(result[4], Is.EquivalentTo(L(0)));
        });
        
    }

    [Test]
    public void TestAssembleSingleSegment()
    {
        
        var input = new[]
        {
            L(0, 0, 0)
        };
        
        var result = input.AssembleSegments();
        Assert.That(result, Is.EquivalentTo(L(0, 0, 0)));
        
    }

    [Test]
    public void TestAssembleMultipleSegments()
    {
        
        var input = new[]
        {
            L(0),
            L(1),
            L(0),
            L(2),
            L(0)
        };

        var result = input.AssembleSegments();
        Assert.That(result, Is.EquivalentTo(L(0, 1, 0, 2, 0)));

    }
    
}