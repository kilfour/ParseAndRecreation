namespace FlowParser.AstConstruction.AstNodes;

public record Addition(AstNode Left, AstNode Right) : AstNode;

