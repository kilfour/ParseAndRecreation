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

    private static Flow<Unit> Manipulate(Func<ParseState, ParseState> manipulate) =>
        TokenFlow(a => Pulse.Manipulate(manipulate)).Then(Pulse.NoOp());

    private static Flow<Unit> Manipulate(Func<Token, ParseState, ParseState> manipulate) =>
        TokenFlow(a => Pulse.Manipulate<ParseState>(ctx => manipulate(a, ctx))).Then(Pulse.NoOp());

    private static readonly Flow<Token> onEnd =
        from token in Pulse.Start<Token>()
        from ctx in Pulse.Gather(new ParseState())
        from _ in Pulse.Trace(ctx.Value.Finish())
        select token;

    private static readonly Flow<Token> flow =
        from token in Pulse.Start<Token>()
        from _ in Pulse.Gather(new ParseState())
        from __ in Pulse.FirstOf(
            (() => token.Kind == TokenKind.Number, /**/ () => Manipulate((a, ctx) => ctx.PushNumber(a.Text))),
            (() => token.IsOperator,               /**/ () => Manipulate((a, ctx) => ctx.PushOperator(a.Kind))),
            (() => token.Kind == TokenKind.LParen, /**/ () => Manipulate(ctx => ctx.OpenParen())),
            (() => token.Kind == TokenKind.RParen, /**/ () => Manipulate(ctx => ctx.CloseParen())),
            (() => token.Kind == TokenKind.End,    /**/ () => Pulse.ToFlow(onEnd, token))
        )
        select token;

    public static AstNode Parse(IEnumerable<Token> tokens) =>
        Signal.From(flow)
            .SetArtery(new TheCollector<AstNode>())
            .Pulse(tokens)
            .GetArtery<TheCollector<AstNode>>()
            .TheExhibit.Last();
}

