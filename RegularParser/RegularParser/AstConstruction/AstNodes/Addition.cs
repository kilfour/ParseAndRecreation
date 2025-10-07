namespace RegularParser.AstConstruction.AstNodes;

public record Addition(AstNode Left, AstNode Right) : AstNode;

