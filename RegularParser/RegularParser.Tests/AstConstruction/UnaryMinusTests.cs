using RegularParser.AstConstruction.AstNodes;

namespace RegularParser.Tests.AstConstruction;

public class UnaryMinusTests
{
    [Fact]
    public void Single()
    {
        var ast = LostIn.Translation("-1");
        var node = Assert.IsType<UnaryMinus>(ast);
        //    (-)
        //     |
        //     1
        Assert.Equal(1, Assert.IsType<Number>(node.Value).Value);
    }

    [Fact]
    public void Double()
    {
        var ast = LostIn.Translation("--1");
        var node = Assert.IsType<UnaryMinus>(ast);
        //    (-)
        //     |
        //    (-)
        //     |
        //     1
        var min = Assert.IsType<UnaryMinus>(node.Value);
        Assert.Equal(1, Assert.IsType<Number>(min.Value).Value);
    }
}