using RegularParser.AstConstruction.AstNodes;

namespace RegularParser.Tests.AstConstruction;

public class SubtractionTests
{
    [Fact]
    public void Simple()
    {
        var ast = LostIn.Translation("5-2");
        //   (-)
        //  /   \
        // 5     2
        var node = Assert.IsType<Subtraction>(ast);
        Assert.Equal(5, Assert.IsType<Number>(node.Left).Value);
        Assert.Equal(2, Assert.IsType<Number>(node.Right).Value);
    }

    [Fact]
    public void Chained_LeftAssociative()
    {
        var ast = LostIn.Translation("10-3-2");
        var node = Assert.IsType<Subtraction>(ast);
        //       (-)
        //      /   \
        //    (-)    2
        //   /   \
        // 10     3
        var left = Assert.IsType<Subtraction>(node.Left);
        Assert.Equal(10, Assert.IsType<Number>(left.Left).Value);
        Assert.Equal(3, Assert.IsType<Number>(left.Right).Value);
        Assert.Equal(2, Assert.IsType<Number>(node.Right).Value);
    }
}
