namespace RegularParser.AstConstruction.AstNodes;

public record Division(AstNode Left, AstNode Right) : AstNode;

