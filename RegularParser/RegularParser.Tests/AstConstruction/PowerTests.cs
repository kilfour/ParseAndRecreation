using Definitions.AstNodes;

namespace RegularParser.Tests.AstConstruction;

public class PowerTests
{
    [Fact]
    public void Simple()
    {
        var ast = LostIn.Translation("2^3");
        //   (^)
        //  /   \
        // 2     3
        var node = Assert.IsType<Power>(ast);
        Assert.Equal(2, Assert.IsType<Number>(node.Left).Value);
        Assert.Equal(3, Assert.IsType<Number>(node.Right).Value);
    }

    [Fact]
    public void Chained_RightAssociative()
    {
        var ast = LostIn.Translation("1^2^3");
        var node = Assert.IsType<Power>(ast);
        //       (^)
        //      /   \
        //     1    (^)
        //         /   \
        //        2     3
        Assert.Equal(1, Assert.IsType<Number>(node.Left).Value);
        var right = Assert.IsType<Power>(node.Right);
        Assert.Equal(2, Assert.IsType<Number>(right.Left).Value);
        Assert.Equal(3, Assert.IsType<Number>(right.Right).Value);
    }
}
