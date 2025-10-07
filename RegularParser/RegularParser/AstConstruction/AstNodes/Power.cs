namespace RegularParser.AstConstruction.AstNodes;

public record Power(AstNode Left, AstNode Right) : AstNode;

