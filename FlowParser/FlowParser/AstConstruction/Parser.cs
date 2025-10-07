using FlowParser.AstConstruction.AstNodes;
using FlowParser.Lexing;
using QuickPulse;
using QuickPulse.Arteries;
using QuickPulse.Bolts;

namespace FlowParser.AstConstruction;

public sealed class Parser
{
    private static Flow<Token> TokenFlow(Func<Token, Flow<ParseState>> flow) =>
        from token in Pulse.Start<Token>()
        from _ in flow(token)
        select token;

    private static readonly Flow<Token> onNumber =
        TokenFlow(a => Pulse.Manipulate<ParseState>(ctx => ctx.PushNumber(a.Text)));

    private static Flow<Token> onOperator =>
        TokenFlow(a => Pulse.Manipulate<ParseState>(ctx => ctx.PushOperator(a.Kind)));
    private static readonly Flow<Token> onLParen =
        TokenFlow(a => Pulse.Manipulate<ParseState>(ctx => ctx.OpenParen()));
    private static readonly Flow<Token> onRParen =
        TokenFlow(a => Pulse.Manipulate<ParseState>(ctx => ctx.CloseParen()));

    private static readonly Flow<Token> onEnd =
        from token in Pulse.Start<Token>()
        from ctx in Pulse.Gather(new ParseState())
        from _ in Pulse.Trace(ctx.Value.Finish())
        select token;

    private static readonly TokenKind[] operators =
        [ TokenKind.Plus
        , TokenKind.Minus
        , TokenKind.Star
        , TokenKind.Slash
        , TokenKind.Caret
        ];

    private static bool IsOperator(Token token)
        => operators.Contains(token.Kind);

    private static readonly Flow<Token> flow =
        from token in Pulse.Start<Token>()
        from _ in Pulse.Gather(new ParseState())
        from __ in Pulse.FirstOf(
            (() => token.Kind == TokenKind.Number, () => Pulse.ToFlow(onNumber, token)),
            (() => IsOperator(token), () => Pulse.ToFlow(onOperator, token)),
            (() => token.Kind == TokenKind.LeftParenthesis, () => Pulse.ToFlow(onLParen, token)),
            (() => token.Kind == TokenKind.RightParenthesis, () => Pulse.ToFlow(onRParen, token)),
            (() => token.Kind == TokenKind.End, () => Pulse.ToFlow(onEnd, token))
        )
        select token;

    public static AstNode Parse(IEnumerable<Token> tokens) =>
        Signal.From(flow)
            .SetArtery(new TheCollector<AstNode>())
            .Pulse(tokens)
            .GetArtery<TheCollector<AstNode>>()
            .TheExhibit.Last();
}

