using RegularParser.AstConstruction.AstNodes;

namespace RegularParser.Tests.AstConstruction;

public class ParenthesesTests
{
    [Fact]
    public void Group_Addition_Then_Multiply()
    {
        var ast = LostIn.Translation("(1+2)*3");
        var node = Assert.IsType<Multiplication>(ast);
        //      (*)
        //     /   \
        //   (+)    3
        //  /   \
        // 1     2
        var add = Assert.IsType<Addition>(node.Left);
        Assert.Equal(1, Assert.IsType<Number>(add.Left).Value);
        Assert.Equal(2, Assert.IsType<Number>(add.Right).Value);
        Assert.Equal(3, Assert.IsType<Number>(node.Right).Value);
    }

    [Fact]
    public void Nested_Groups()
    {
        var ast = LostIn.Translation("((1+2)*(3+4))");
        var node = Assert.IsType<Multiplication>(ast);
        //        (*)
        //       /   \
        //     (+)   (+)
        //    /  \  /  \
        //   1    2 3   4
        var leftAdd = Assert.IsType<Addition>(node.Left);
        var rightAdd = Assert.IsType<Addition>(node.Right);
        Assert.Equal(1, Assert.IsType<Number>(leftAdd.Left).Value);
        Assert.Equal(2, Assert.IsType<Number>(leftAdd.Right).Value);
        Assert.Equal(3, Assert.IsType<Number>(rightAdd.Left).Value);
        Assert.Equal(4, Assert.IsType<Number>(rightAdd.Right).Value);
    }
}
