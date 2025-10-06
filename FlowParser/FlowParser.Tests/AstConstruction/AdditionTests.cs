using FlowParser.AstConstruction.AstNodes;

namespace FlowParser.Tests.AstConstruction;

public class AdditionTests
{
    [Fact]
    public void Simple()
    {
        var ast = LostIn.Translation("1+2");
        //   (+)
        //  /   \
        // 1     2
        var node = Assert.IsType<Addition>(ast);
        Assert.Equal(1, Assert.IsType<Number>(node.Left).Value);
        Assert.Equal(2, Assert.IsType<Number>(node.Right).Value);
    }

    [Fact]
    public void Chained()
    {
        var ast = LostIn.Translation("1+2+3");
        var node = Assert.IsType<Addition>(ast);
        //      (+)
        //     /   \
        //   (+)    3
        //  /   \
        // 1     2
        var nodeL = Assert.IsType<Addition>(node.Left);
        Assert.Equal(1, Assert.IsType<Number>(nodeL.Left).Value);
        Assert.Equal(2, Assert.IsType<Number>(nodeL.Right).Value);
        Assert.Equal(3, Assert.IsType<Number>(node.Right).Value);
    }
}