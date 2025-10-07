using FlowParser.AstConstruction.AstNodes;

namespace FlowParser.Tests.AstConstruction;

public class EdgeCaseTests
{
    [Fact]
    public void UnaryMinus_Simple()
    {
        var ast = LostIn.Translation("-3");
        var node = Assert.IsType<UnaryMinus>(ast);
        //      (-)
        //       |
        //       3
        Assert.Equal(3, Assert.IsType<Number>(node.Value).Value);
    }

    [Fact]
    public void UnaryMinus_OnParenthesized()
    {
        var ast = LostIn.Translation("-(1+2)");
        var node = Assert.IsType<UnaryMinus>(ast);
        //      (-)
        //       |
        //      (+)
        //     /   \
        //    1     2
        var add = Assert.IsType<Addition>(node.Value);
        Assert.Equal(1, Assert.IsType<Number>(add.Left).Value);
        Assert.Equal(2, Assert.IsType<Number>(add.Right).Value);
    }

    [Fact]
    public void UnaryMinus_BindsTighterThan_Add()
    {
        var ast = LostIn.Translation("1+-2");
        var node = Assert.IsType<Addition>(ast);
        //      (+)
        //     /   \
        //    1    (-)
        //          |
        //          2
        Assert.Equal(1, Assert.IsType<Number>(node.Left).Value);
        var neg = Assert.IsType<UnaryMinus>(node.Right);
        Assert.Equal(2, Assert.IsType<Number>(neg.Value).Value);
    }

    [Fact]
    public void Power_Vs_UnaryMinus()
    {
        var ast = LostIn.Translation("-1^2");
        var node = Assert.IsType<Power>(ast);
        //       (^)
        //      /   \
        //    (-)     2
        //     |
        //     1
        var min = Assert.IsType<UnaryMinus>(node.Left);
        Assert.Equal(1, Assert.IsType<Number>(min.Value).Value);
        Assert.Equal(2, Assert.IsType<Number>(node.Right).Value);
    }

    [Fact]
    public void Parenthesized_Negative_ToPower()
    {
        var ast = LostIn.Translation("(-2)^2");
        var node = Assert.IsType<Power>(ast);
        //        (^)
        //       /   \
        //     (-)    2
        //      |
        //      2
        var neg = Assert.IsType<UnaryMinus>(node.Left);
        Assert.Equal(2, Assert.IsType<Number>(neg.Value).Value);
        Assert.Equal(2, Assert.IsType<Number>(node.Right).Value);
    }

    [Fact]
    public void Division_LeftAssociative()
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

    [Fact]
    public void Whitespace_Robust()
    {
        var ast = LostIn.Translation("  \t 1 +  2 *\n ( 3 + 4 )   ");
        var node = Assert.IsType<Addition>(ast);
        //      (+)
        //     /   \
        //    1    (*)
        //        /   \
        //       2    (+)
        //           /   \
        //          3     4
        Assert.Equal(1, Assert.IsType<Number>(node.Left).Value);
        var mul = Assert.IsType<Multiplication>(node.Right);
        Assert.Equal(2, Assert.IsType<Number>(mul.Left).Value);
        var add = Assert.IsType<Addition>(mul.Right);
        Assert.Equal(3, Assert.IsType<Number>(add.Left).Value);
        Assert.Equal(4, Assert.IsType<Number>(add.Right).Value);
    }

    [Fact]
    public void Decimal_Edge_Valid()
    {
        var ast = LostIn.Translation("0.5+2.0");
        var node = Assert.IsType<Addition>(ast);
        Assert.Equal(0.5, Assert.IsType<Number>(node.Left).Value);
        Assert.Equal(2.0, Assert.IsType<Number>(node.Right).Value);
    }

    [Fact]
    public void Decimal_Edge_Invalid_LeadingDot()
    {
        Assert.Throws<Exception>(() => LostIn.Translation(".5+1"));
    }

    [Fact]
    public void Decimal_Edge_Invalid_TrailingDot()
    {
        Assert.Throws<Exception>(() => LostIn.Translation("1.+2"));
    }

    [Fact]
    public void Empty_Input_Fails()
    {
        Assert.Throws<Exception>(() => LostIn.Translation(""));
    }

    [Fact]
    public void Mismatched_Paren_Fails()
    {
        Assert.Throws<Exception>(() => LostIn.Translation("(1+2"));
        Assert.Throws<Exception>(() => LostIn.Translation("1+2)"));
    }

    [Fact]
    public void Long_AddChain_LeftAssociative_Shape()
    {
        var ast = LostIn.Translation("1+2+3+4+5");
        var a1 = Assert.IsType<Addition>(ast);
        var a2 = Assert.IsType<Addition>(a1.Left);
        var a3 = Assert.IsType<Addition>(a2.Left);
        var a4 = Assert.IsType<Addition>(a3.Left);
        Assert.Equal(1, Assert.IsType<Number>(a4.Left).Value);
        Assert.Equal(2, Assert.IsType<Number>(a4.Right).Value);
        Assert.Equal(3, Assert.IsType<Number>(a3.Right).Value);
        Assert.Equal(4, Assert.IsType<Number>(a2.Right).Value);
        Assert.Equal(5, Assert.IsType<Number>(a1.Right).Value);
    }

    [Fact]
    public void Subtract_Then_Add_Precedence()
    {
        var ast = LostIn.Translation("10-2+3");
        var add = Assert.IsType<Addition>(ast);
        //      (+)
        //     /   \
        //   (-)    3
        //  /   \
        // 10    2
        var sub = Assert.IsType<Subtraction>(add.Left);
        Assert.Equal(10, Assert.IsType<Number>(sub.Left).Value);
        Assert.Equal(2, Assert.IsType<Number>(sub.Right).Value);
        Assert.Equal(3, Assert.IsType<Number>(add.Right).Value);
    }
}
