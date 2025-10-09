namespace RegularParser.Tests.Evaluation;

public class EvalTests
{
    [Fact]
    public void SingleNumber() =>
        Assert.Equal(42, LostIn.Translation("42").Eval());

    [Fact]
    public void DecimalNumber() =>
        Assert.Equal(10, LostIn.Translation("2.5*4").Eval());

    [Fact]
    public void ParenthesizedAtom() =>
        Assert.Equal(7, LostIn.Translation("(7)").Eval());

    [Fact]
    public void NestedParentheses() =>
        Assert.Equal(1, LostIn.Translation("(((1)))").Eval());

    [Fact]
    public void WhitespaceIgnored() =>
        Assert.Equal(9, LostIn.Translation(" ( 1 + 2 ) * 3 ").Eval());

    [Fact]
    public void Addition() =>
        Assert.Equal(3, LostIn.Translation("1+2").Eval());

    [Fact]
    public void SubtractionLeftAssociative() =>
        Assert.Equal(-5, LostIn.Translation("2-3-4").Eval());

    [Fact]
    public void MixAddSub() =>
        Assert.Equal(0, LostIn.Translation("10-7-3+0").Eval());

    [Fact]
    public void Multiplication() =>
        Assert.Equal(56, LostIn.Translation("7*8").Eval());

    [Fact]
    public void DivisionLeftAssociativeChain() =>
        Assert.Equal(1, LostIn.Translation("8/4/2").Eval());

    [Fact]
    public void DivisionThenMultiplicationLeftAssociative() =>
        Assert.Equal(9, LostIn.Translation("6/2*3").Eval());

    [Fact]
    public void PowerSimple() =>
        Assert.Equal(81, LostIn.Translation("3^4").Eval());

    [Fact]
    public void PowerRightAssociative() =>
        Assert.Equal(512, LostIn.Translation("2^3^2").Eval());

    [Fact]
    public void UnaryMinusOnNumber() =>
        Assert.Equal(-5, LostIn.Translation("-5").Eval());

    [Fact]
    public void DoubleUnaryMinus() =>
        Assert.Equal(1, LostIn.Translation("--1").Eval());

    [Fact]
    public void UnaryMinusBindsTighterThanSum() =>
        Assert.Equal(2, LostIn.Translation("3+-1").Eval());

    [Fact]
    public void BinaryMinusFollowedByUnaryMinus() =>
        Assert.Equal(2, LostIn.Translation("1 - -1").Eval());

    [Fact]
    public void MulBeatsAdd() =>
        Assert.Equal(7, LostIn.Translation("1+2*3").Eval());

    [Fact]
    public void PowBeatsMulBeatsAdd() =>
        Assert.Equal(19, LostIn.Translation("1+2*3^2").Eval());

    [Fact]
    public void ParenthesesOverridePrecedence() =>
        Assert.Equal(27, LostIn.Translation("(1+2)*3^2").Eval());

    [Fact]
    public void UnaryMinusVsPower() =>
        Assert.Equal(-4, LostIn.Translation("-2^2").Eval());

    [Fact]
    public void ParenthesizedNegativeBaseToPower() =>
        Assert.Equal(4, LostIn.Translation("(-2)^2").Eval());

    [Fact]
    public void DecimalTimesInteger() =>
        Assert.Equal(6, LostIn.Translation("1.5*4").Eval());

    [Fact]
    public void MixedDecimalExpr() =>
        Assert.Equal(2, LostIn.Translation("(0.5+0.5)*2").Eval());
}
