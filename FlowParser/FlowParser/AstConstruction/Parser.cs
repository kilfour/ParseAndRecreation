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
    private static readonly Flow<Token> onPlus =
        TokenFlow(a => Pulse.Manipulate<ParseState>(ctx => ctx.PushOperator(TokenKind.Plus)));
    private static readonly Flow<Token> onMinus =
        TokenFlow(a => Pulse.Manipulate<ParseState>(ctx => ctx.PushOperator(TokenKind.Minus)));
    private static readonly Flow<Token> onStar =
        TokenFlow(a => Pulse.Manipulate<ParseState>(ctx => ctx.PushOperator(TokenKind.Star)));
    private static readonly Flow<Token> onSlash =
        TokenFlow(a => Pulse.Manipulate<ParseState>(ctx => ctx.PushOperator(TokenKind.Slash)));
    private static readonly Flow<Token> onCaret =
        TokenFlow(a => Pulse.Manipulate<ParseState>(ctx => ctx.PushOperator(TokenKind.Caret)));
    private static readonly Flow<Token> onLParen =
        TokenFlow(a => Pulse.Manipulate<ParseState>(ctx => ctx.OpenParen()));
    private static readonly Flow<Token> onRParen =
        TokenFlow(a => Pulse.Manipulate<ParseState>(ctx => ctx.CloseParen()));

    private static readonly Flow<Token> onEnd =
        from token in Pulse.Start<Token>()
        from ctx in Pulse.Gather(new ParseState())
        from _ in Pulse.Trace(ctx.Value.Finish())
        select token;

    private static readonly Flow<Token> flow =
        from token in Pulse.Start<Token>()
        from _ in Pulse.Gather(new ParseState())
        from __ in Pulse.FirstOf(
            (() => token.Kind == TokenKind.Number, () => Pulse.ToFlow(onNumber, token)),
            (() => token.Kind == TokenKind.Plus, () => Pulse.ToFlow(onPlus, token)),
            (() => token.Kind == TokenKind.Minus, () => Pulse.ToFlow(onMinus, token)),
            (() => token.Kind == TokenKind.Star, () => Pulse.ToFlow(onStar, token)),
            (() => token.Kind == TokenKind.Slash, () => Pulse.ToFlow(onSlash, token)),
            (() => token.Kind == TokenKind.Caret, () => Pulse.ToFlow(onCaret, token)),
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

