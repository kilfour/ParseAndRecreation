using FlowParser.AstConstruction.AstNodes;

namespace FlowParser.Tests.AstConstruction;

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
        var ast = LostIn.Translation("2^3^2");
        var node = Assert.IsType<Power>(ast);
        //       (^)
        //      /   \
        //     2    (^)
        //         /   \
        //        3     2
        Assert.Equal(2, Assert.IsType<Number>(node.Left).Value);
        var right = Assert.IsType<Power>(node.Right);
        Assert.Equal(3, Assert.IsType<Number>(right.Left).Value);
        Assert.Equal(2, Assert.IsType<Number>(right.Right).Value);
    }
}
