using System.Text.RegularExpressions;
using QuickPulse;
using QuickPulse.Arteries;
using QuickPulse.Bolts;

namespace FlowParser.Lexing;

public static partial class Lexer
{
    private static Dictionary<char, Func<int, Token>> singleChars =
        new() {
            { '(',  a => new Token(TokenKind.LeftParenthesis, "(", a) },
            { ')',  a => new Token(TokenKind.RightParenthesis, ")", a) },
            { '+',  a => new Token(TokenKind.Plus, "+", a) },
            { '-',  a => new Token(TokenKind.Minus, "-", a) },
            { '*',  a => new Token(TokenKind.Star, "*", a) },
            { '/',  a => new Token(TokenKind.Slash, "/", a) },
            { '^',  a => new Token(TokenKind.Caret, "^", a) },
            { '\0',  a => new Token(TokenKind.End, "\0", a) }

        };

    private static readonly Flow<char> number =
        from input in Pulse.Start<char>()
        from _ in Pulse.Manipulate<string>(a => a + input)
        select input;

    [GeneratedRegex(@"^(?:0|[1-9][0-9]*)(?:\.[0-9]+)?$", RegexOptions.Compiled)]
    private static partial Regex IsDecimalRegex();
    private static bool IsValidDecimal(string s) => IsDecimalRegex().IsMatch(s);

    private static readonly Flow<char> flushNumber =
        from input in Pulse.Start<char>()
        from valid in Pulse.EffectIf<string>(
            a => a != string.Empty && !IsValidDecimal(a),
            () => throw new Exception($"Invalid decimal"))
        from _ in Pulse.TraceIf<string>(
            a => a != string.Empty,
            a => new Token(TokenKind.Number, a, 666))
        from __ in Pulse.Manipulate<string>(a => string.Empty)
        select input;

    private static bool IsDecimalChar(char input) => char.IsDigit(input) || input == '.';

    private static readonly Flow<char> flow =
        from input in Pulse.Start<char>()
        from context in Pulse.Gather(string.Empty)
        from flush in Pulse.ToFlowIf(
            !IsDecimalChar(input) && context.Value != string.Empty,
            flushNumber, () => input
        )
        from _ in Pulse.FirstOf(
            (() => char.IsWhiteSpace(input), Pulse.NoOp),
            (() => IsDecimalChar(input), () => Pulse.ToFlow(number, input)),
            (() => singleChars.ContainsKey(input), () => Pulse.Trace(singleChars[input](666)))
        )
        select input;

    public static IEnumerable<Token> Tokenize(string input) =>
        Signal.From(flow)
            .SetArtery(new TheCollector<Token>())
            .Pulse(input.Append('\0'))
            .GetArtery<TheCollector<Token>>()
            .TheExhibit;
}


