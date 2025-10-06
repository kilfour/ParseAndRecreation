namespace FlowParser.Lexing;

public record Token(TokenKind Kind, string Text, int Offset);