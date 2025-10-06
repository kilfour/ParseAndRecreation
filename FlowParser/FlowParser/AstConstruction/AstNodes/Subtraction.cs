namespace FlowParser.AstConstruction.AstNodes;

public record Subtraction(AstNode Left, AstNode Right) : AstNode;

