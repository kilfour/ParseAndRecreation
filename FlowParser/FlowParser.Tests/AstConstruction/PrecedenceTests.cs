using FlowParser.AstConstruction.AstNodes;

namespace FlowParser.Tests.AstConstruction;

public class PrecedenceTests
{
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

    [Fact]
    public void Add_Multiply()
    {
        var ast = LostIn.Translation("1+2*3");
        var node = Assert.IsType<Addition>(ast);
        //      (+)
        //     /   \
        //    1    (*)
        //        /   \
        //       2     3
        Assert.Equal(1, Assert.IsType<Number>(node.Left).Value);
        var mul = Assert.IsType<Multiplication>(node.Right);
        Assert.Equal(2, Assert.IsType<Number>(mul.Left).Value);
        Assert.Equal(3, Assert.IsType<Number>(mul.Right).Value);
    }

    [Fact]
    public void Multiply_Add()
    {
        var ast = LostIn.Translation("1*2+3");
        var node = Assert.IsType<Addition>(ast);
        //      (+)
        //     /   \
        //   (*)    3
        //  /   \
        // 1     2
        var mul = Assert.IsType<Multiplication>(node.Left);
        Assert.Equal(1, Assert.IsType<Number>(mul.Left).Value);
        Assert.Equal(2, Assert.IsType<Number>(mul.Right).Value);
        Assert.Equal(3, Assert.IsType<Number>(node.Right).Value);
    }

    [Fact]
    public void Parentheses_Override()
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
    public void Power_RightAssociative()
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

    [Fact]
    public void Power_BindsTighterThan_Multiply()
    {
        var ast = LostIn.Translation("2*3^2");
        var node = Assert.IsType<Multiplication>(ast);
        //      (*)
        //     /   \
        //    2    (^)
        //        /   \
        //       3     2
        Assert.Equal(2, Assert.IsType<Number>(node.Left).Value);
        var pow = Assert.IsType<Power>(node.Right);
        Assert.Equal(3, Assert.IsType<Number>(pow.Left).Value);
        Assert.Equal(2, Assert.IsType<Number>(pow.Right).Value);
    }

    [Fact]
    public void Nested_Parentheses()
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

    [Fact]
    public void Division_And_Subtraction()
    {
        var ast = LostIn.Translation("10-6/3");
        var node = Assert.IsType<Subtraction>(ast);
        //      (-)
        //     /   \
        //   10    (/)
        //        /   \
        //       6     3
        Assert.Equal(10, Assert.IsType<Number>(node.Left).Value);
        var div = Assert.IsType<Division>(node.Right);
        Assert.Equal(6, Assert.IsType<Number>(div.Left).Value);
        Assert.Equal(3, Assert.IsType<Number>(div.Right).Value);
    }

    [Fact]
    public void Decimals_Simple()
    {
        var ast = LostIn.Translation("1.5+2.25");
        var node = Assert.IsType<Addition>(ast);
        //     (+)
        //    /   \
        //  1.5  2.25
        Assert.Equal(1.5, Assert.IsType<Number>(node.Left).Value);
        Assert.Equal(2.25, Assert.IsType<Number>(node.Right).Value);
    }

    [Fact]
    public void Mixed_Complex()
    {
        var ast = LostIn.Translation("1+2*3^2-4/(5-3)");
        var node = Assert.IsType<Subtraction>(ast);
        //           (-)
        //         /     \
        //       (+)     (/)
        //      /   \   /   \
        //     1   (*)  4   (-)
        //        /  \      / \
        //       2  (^)    5   3
        //          /  \
        //         3    2
        var left = Assert.IsType<Addition>(node.Left);
        Assert.Equal(1, Assert.IsType<Number>(left.Left).Value);
        var mul = Assert.IsType<Multiplication>(left.Right);
        Assert.Equal(2, Assert.IsType<Number>(mul.Left).Value);
        var pow = Assert.IsType<Power>(mul.Right);
        Assert.Equal(3, Assert.IsType<Number>(pow.Left).Value);
        Assert.Equal(2, Assert.IsType<Number>(pow.Right).Value);

        var right = Assert.IsType<Division>(node.Right);
        Assert.Equal(4, Assert.IsType<Number>(right.Left).Value);
        var innerMinus = Assert.IsType<Subtraction>(right.Right);
        Assert.Equal(5, Assert.IsType<Number>(innerMinus.Left).Value);
        Assert.Equal(3, Assert.IsType<Number>(innerMinus.Right).Value);
    }

    [Fact]
    public void RightHeavy_AddChain()
    {
        var ast = LostIn.Translation("1+2+3+4");
        var n1 = Assert.IsType<Addition>(ast);
        //             (+)
        //           /     \
        //         (+)      4
        //       /    \
        //     (+)     3
        //    /   \
        //   1     2
        var n2 = Assert.IsType<Addition>(n1.Left);
        var n3 = Assert.IsType<Addition>(n2.Left);
        Assert.Equal(1, Assert.IsType<Number>(n3.Left).Value);
        Assert.Equal(2, Assert.IsType<Number>(n3.Right).Value);
        Assert.Equal(3, Assert.IsType<Number>(n2.Right).Value);
        Assert.Equal(4, Assert.IsType<Number>(n1.Right).Value);
    }
}
