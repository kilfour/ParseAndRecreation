namespace Definitions.AstNodes;

public record Number(double Value) : AstNode
{
    public override double Eval() => Value;
}

