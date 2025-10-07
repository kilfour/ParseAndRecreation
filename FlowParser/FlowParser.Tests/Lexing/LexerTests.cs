using Definitions.Lexing;

namespace FlowParser.Tests.Lexing;

public class LexerTests
{
    [Fact]
    public void SingleCharTokens_Are_Recognized()
    {
        var tokens = Lexer.Tokenize("()+-*/^");
        Assert.Collection(tokens,
            t => Assert.Equal(TokenKind.LParen, t.Kind),
            t => Assert.Equal(TokenKind.RParen, t.Kind),
            t => Assert.Equal(TokenKind.Plus, t.Kind),
            t => Assert.Equal(TokenKind.Minus, t.Kind),
            t => Assert.Equal(TokenKind.Star, t.Kind),
            t => Assert.Equal(TokenKind.Slash, t.Kind),
            t => Assert.Equal(TokenKind.Caret, t.Kind),
            t => Assert.Equal(TokenKind.End, t.Kind)
        );
    }

    [Fact]
    public void Single_Number()
    {
        var tokens = Lexer.Tokenize("1").ToArray();
        Assert.Equal(("1", TokenKind.Number), (tokens[0].Text, tokens[0].Kind));
    }

    [Fact]
    public void Numbers_Are_Grouped()
    {
        var tokens = Lexer.Tokenize("123 45");
        Assert.Collection(tokens,
            t => Assert.Equal(("123", TokenKind.Number), (t.Text, t.Kind)),
            t => Assert.Equal(("45", TokenKind.Number), (t.Text, t.Kind)),
            t => Assert.Equal(TokenKind.End, t.Kind)
        );
    }

    [Fact]
    public void Add_Then_Number()
    {
        var tokens = Lexer.Tokenize("+1").ToArray();
        Assert.Equal(("+", TokenKind.Plus), (tokens[0].Text, tokens[0].Kind));
    }

    [Fact]
    public void Number_Then_Add()
    {
        var tokens = Lexer.Tokenize("+1").ToArray();
        Assert.Equal(("+", TokenKind.Plus), (tokens[0].Text, tokens[0].Kind));
    }

    [Fact]
    public void Mix_Of_Numbers_And_Ops()
    {
        var tokens = Lexer.Tokenize("12+3*(45-6)").ToArray();
        Assert.Equal(("12", TokenKind.Number), (tokens[0].Text, tokens[0].Kind));
        Assert.Equal(("+", TokenKind.Plus), (tokens[1].Text, tokens[1].Kind));
        Assert.Equal(("3", TokenKind.Number), (tokens[2].Text, tokens[2].Kind));
        Assert.Equal(("*", TokenKind.Star), (tokens[3].Text, tokens[3].Kind));
        Assert.Equal(("(", TokenKind.LParen), (tokens[4].Text, tokens[4].Kind));
        Assert.Equal(("45", TokenKind.Number), (tokens[5].Text, tokens[5].Kind));
        Assert.Equal(("-", TokenKind.Minus), (tokens[6].Text, tokens[6].Kind));
        Assert.Equal(("6", TokenKind.Number), (tokens[7].Text, tokens[7].Kind));
        Assert.Equal((")", TokenKind.RParen), (tokens[8].Text, tokens[8].Kind));
    }

    [Fact]
    public void Ignores_Whitespace()
    {
        var tokens = Lexer.Tokenize("  7   +   8 ");
        Assert.Collection(tokens,
            t => Assert.Equal(("7", TokenKind.Number), (t.Text, t.Kind)),
            t => Assert.Equal(("+", TokenKind.Plus), (t.Text, t.Kind)),
            t => Assert.Equal(("8", TokenKind.Number), (t.Text, t.Kind)),
            t => Assert.Equal(TokenKind.End, t.Kind)
        );
    }
}
