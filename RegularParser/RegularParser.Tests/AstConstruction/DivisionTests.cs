using RegularParser.AstConstruction.AstNodes;

namespace RegularParser.Tests.AstConstruction;

public class DivisionTests
{
    [Fact]
    public void Simple()
    {
        var ast = LostIn.Translation("8/2");
        //   (/)
        //  /   \
        // 8     2
        var node = Assert.IsType<Division>(ast);
        Assert.Equal(8, Assert.IsType<Number>(node.Left).Value);
        Assert.Equal(2, Assert.IsType<Number>(node.Right).Value);
    }

    [Fact]
    public void Chained_LeftAssociative()
    {
        var ast = LostIn.Translation("8/4/2");
        var node = Assert.IsType<Division>(ast);
        //        (/)
        //       /   \
        //     (/)    2
        //    /   \
        //   8     4
        var left = Assert.IsType<Division>(node.Left);
        Assert.Equal(8, Assert.IsType<Number>(left.Left).Value);
        Assert.Equal(4, Assert.IsType<Number>(left.Right).Value);
        Assert.Equal(2, Assert.IsType<Number>(node.Right).Value);
    }
}
