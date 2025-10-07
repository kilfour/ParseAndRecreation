using FlowParser.AstConstruction.AstNodes;
using FlowParser.Lexing;

namespace FlowParser.AstConstruction;

public sealed class ParseState
{
    public Stack<AstNode> Values { get; } = new();
    public Stack<TokenKind> Operators { get; } = new();

    private TokenKind? _lastToken;

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
        _lastToken = TokenKind.Number;
        return this;
    }

    public ParseState PushOperator(TokenKind op)
    {
        if (op == TokenKind.Minus &&
            (_lastToken is null
             || _lastToken is TokenKind.LeftParenthesis
             || _lastToken is TokenKind.Plus
             || _lastToken is TokenKind.Minus
             || _lastToken is TokenKind.Star
             || _lastToken is TokenKind.Slash
             || _lastToken is TokenKind.Caret
             || _lastToken is TokenKind.UnaryMinus))
        {
            op = TokenKind.UnaryMinus;
        }

        while (Operators.Count > 0 && Operators.Peek() != TokenKind.LeftParenthesis)
        {
            var top = Operators.Peek();
            var cond = IsRightAssociative(op)
                ? Precedence(top) > Precedence(op)
                : Precedence(top) >= Precedence(op);
            if (!cond) break;
            ApplyTop();
        }

        Operators.Push(op);
        _lastToken = op;
        return this;
    }


    public ParseState OpenParen()
    {
        Operators.Push(TokenKind.LeftParenthesis);
        _lastToken = TokenKind.LeftParenthesis;
        return this;
    }

    public ParseState CloseParen()
    {
        while (Operators.Count > 0 && Operators.Peek() != TokenKind.LeftParenthesis)
            ApplyTop();
        if (Operators.Count == 0 || Operators.Pop() != TokenKind.LeftParenthesis)
            throw new Exception("Mismatched parentheses");
        _lastToken = TokenKind.RightParenthesis;
        return this;
    }

    public AstNode Finish()
    {
        while (Operators.Count > 0)
        {
            if (Operators.Peek() == TokenKind.LeftParenthesis)
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

        var rhs = Values.Pop();
        var lhs = Values.Pop();

        Values.Push(op switch
        {
            TokenKind.Plus => new Addition(lhs, rhs),
            TokenKind.Minus => new Subtraction(lhs, rhs),
            TokenKind.Star => new Multiplication(lhs, rhs),
            TokenKind.Slash => new Division(lhs, rhs),
            TokenKind.Caret => new Power(lhs, rhs),
            _ => throw new Exception($"Unexpected operator {op}")
        });
    }
}
