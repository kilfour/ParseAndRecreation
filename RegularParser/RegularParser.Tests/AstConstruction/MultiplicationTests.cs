using RegularParser.AstConstruction.AstNodes;

namespace RegularParser.Tests.AstConstruction;

public class MultiplicationTests
{
    [Fact]
    public void Simple()
    {
        var ast = LostIn.Translation("2*4");
        //   (*)
        //  /   \
        // 2     4
        var node = Assert.IsType<Multiplication>(ast);
        Assert.Equal(2, Assert.IsType<Number>(node.Left).Value);
        Assert.Equal(4, Assert.IsType<Number>(node.Right).Value);
    }

    [Fact]
    public void Chained_LeftAssociative()
    {
        var ast = LostIn.Translation("2*3*4");
        var node = Assert.IsType<Multiplication>(ast);
        //        (*)
        //       /   \
        //     (*)    4
        //    /   \
        //   2     3
        var left = Assert.IsType<Multiplication>(node.Left);
        Assert.Equal(2, Assert.IsType<Number>(left.Left).Value);
        Assert.Equal(3, Assert.IsType<Number>(left.Right).Value);
        Assert.Equal(4, Assert.IsType<Number>(node.Right).Value);
    }
}
