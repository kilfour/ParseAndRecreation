namespace Definitions.AstNodes;

public record Division(AstNode Left, AstNode Right) : AstNode
{
    public override double Eval() => Left.Eval() / Right.Eval();
}

