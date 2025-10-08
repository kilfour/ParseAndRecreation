namespace Definitions.AstNodes;

public record Multiplication(AstNode Left, AstNode Right) : AstNode
{
    public override double Eval() => Left.Eval() * Right.Eval();
}

