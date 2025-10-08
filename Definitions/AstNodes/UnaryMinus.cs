namespace Definitions.AstNodes;

public record UnaryMinus(AstNode Value) : AstNode
{
    public override double Eval() => 0 - Value.Eval();
}

