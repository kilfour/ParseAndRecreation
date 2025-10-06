using FlowParser.AstConstruction.AstNodes;
using FlowParser.Lexing;

namespace FlowParser.AstConstruction;

public sealed class ParseState
{
    public Stack<AstNode> Values { get; } = new();
    public Stack<TokenKind> Operators { get; } = new();

    public static int Precedence(TokenKind kind) => kind switch
    {
        TokenKind.Caret => 4,
        TokenKind.Star or TokenKind.Slash => 3,
        TokenKind.Plus or TokenKind.Minus => 2,
        _ => 0
    };

    public static bool IsRightAssociative(TokenKind kind) => kind == TokenKind.Caret;

    public ParseState PushNumber(string text)
    {
        Values.Push(new Number(double.Parse(text, System.Globalization.CultureInfo.InvariantCulture)));
        return this;
    }

    public ParseState PushOperator(TokenKind op)
    {
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
        return this;
    }

    public ParseState OpenParen()
    {
        Operators.Push(TokenKind.LeftParenthesis);
        return this;
    }

    public ParseState CloseParen()
    {
        while (Operators.Count > 0 && Operators.Peek() != TokenKind.LeftParenthesis)
            ApplyTop();
        if (Operators.Count == 0 || Operators.Pop() != TokenKind.LeftParenthesis)
            throw new Exception("Mismatched parentheses");
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
        var rightHandSide = Values.Pop();
        var LeftHandSide = Values.Pop();
        Values.Push(op switch
        {
            TokenKind.Plus => new Addition(LeftHandSide, rightHandSide),
            TokenKind.Minus => new Subtraction(LeftHandSide, rightHandSide),
            TokenKind.Star => new Multiplication(LeftHandSide, rightHandSide),
            TokenKind.Slash => new Division(LeftHandSide, rightHandSide),
            TokenKind.Caret => new Power(LeftHandSide, rightHandSide),
            _ => throw new Exception($"Unexpected op {op}")
        });
    }
}
