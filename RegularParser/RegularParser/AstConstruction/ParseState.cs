using RegularParser.AstConstruction.AstNodes;
using RegularParser.Lexing;

namespace RegularParser.AstConstruction;

public sealed class ParseState
{
    public Stack<AstNode> Values { get; } = new();
    public Stack<TokenKind> Operators { get; } = new();

    private TokenKind? lastToken;

    public static int Precedence(TokenKind kind) => kind switch
    {
        TokenKind.UnaryMinus => 5,
        TokenKind.Caret => 4,
        TokenKind.Star or TokenKind.Slash => 3,
        TokenKind.Plus or TokenKind.Minus => 2,
        _ => 0
    };

    public static bool IsRightAssociative(TokenKind kind) =>
        kind is TokenKind.Caret or TokenKind.UnaryMinus;

    public ParseState PushNumber(string text)
    {
        Values.Push(new Number(double.Parse(text, System.Globalization.CultureInfo.InvariantCulture)));
        lastToken = TokenKind.Number;
        return this;
    }

    public ParseState PushOperator(TokenKind op)
    {
        if (IsUnaryMinus(op))
            op = TokenKind.UnaryMinus;

        while (Operators.Count > 0 && Operators.Peek() != TokenKind.LParen)
        {
            var top = Operators.Peek();
            var cond = IsRightAssociative(op)
                ? Precedence(top) > Precedence(op)
                : Precedence(top) >= Precedence(op);
            if (!cond) break;
            ApplyTop();
        }

        Operators.Push(op);
        lastToken = op;
        return this;
    }

    private bool IsUnaryMinus(TokenKind op) =>
        op == TokenKind.Minus && (
           lastToken is null
        || lastToken is TokenKind.LParen
        || lastToken is TokenKind.Plus
        || lastToken is TokenKind.Minus
        || lastToken is TokenKind.Star
        || lastToken is TokenKind.Slash
        || lastToken is TokenKind.Caret
        || lastToken is TokenKind.UnaryMinus);

    public ParseState OpenParen()
    {
        Operators.Push(TokenKind.LParen);
        lastToken = TokenKind.LParen;
        return this;
    }

    public ParseState CloseParen()
    {
        while (Operators.Count > 0 && Operators.Peek() != TokenKind.LParen)
            ApplyTop();
        if (Operators.Count == 0 || Operators.Pop() != TokenKind.LParen)
            throw new Exception("Mismatched parentheses");
        lastToken = TokenKind.RParen;
        return this;
    }

    public AstNode Finish()
    {
        while (Operators.Count > 0)
        {
            if (Operators.Peek() == TokenKind.LParen)
                throw new Exception("Mismatched parentheses");
            ApplyTop();
        }
        return Values.Count == 1 ? Values.Pop() : throw new Exception("Parse error");
    }

    private void ApplyTop()
    {
        var op = Operators.Pop();

        if (op == TokenKind.UnaryMinus)
        {
            var value = Values.Pop();
            Values.Push(new UnaryMinus(value));
            return;
        }

        var rightHandSide = Values.Pop();
        var leftHandSide = Values.Pop();

        Values.Push(op switch
        {
            TokenKind.Plus => new Addition(leftHandSide, rightHandSide),
            TokenKind.Minus => new Subtraction(leftHandSide, rightHandSide),
            TokenKind.Star => new Multiplication(leftHandSide, rightHandSide),
            TokenKind.Slash => new Division(leftHandSide, rightHandSide),
            TokenKind.Caret => new Power(leftHandSide, rightHandSide),
            _ => throw new Exception($"Unexpected operator {op}")
        });
    }
}
