namespace RegularParser.AstConstruction.AstNodes;

public record Multiplication(AstNode Left, AstNode Right) : AstNode;

