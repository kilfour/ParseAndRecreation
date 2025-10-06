namespace FlowParser.AstConstruction.AstNodes;

public record Multiplication(AstNode Left, AstNode Right) : AstNode;

