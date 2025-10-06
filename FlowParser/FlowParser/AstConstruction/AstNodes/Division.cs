namespace FlowParser.AstConstruction.AstNodes;

public record Division(AstNode Left, AstNode Right) : AstNode;

