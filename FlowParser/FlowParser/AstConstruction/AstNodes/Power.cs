namespace FlowParser.AstConstruction.AstNodes;

public record Power(AstNode Left, AstNode Right) : AstNode;

