using Definitions.AstNodes;
using Definitions.Lexing;
using QuickPulse;
using QuickPulse.Arteries;

namespace FlowParser;

public sealed class Parser
{
    private static Flow<Unit> Manipulate(Func<ParseState, ParseState> manipulate) =>
        Pulse.Manipulate(manipulate).Dissipate();

    private static Flow<Unit> Manipulate(Func<Token, ParseState, ParseState> manipulate) =>
        Pulse.Drain((Token a) => Pulse.Manipulate<ParseState>(ctx => manipulate(a, ctx)));

    private static readonly Flow<Token> flow =
        from token in Pulse.Start<Token>()
        from _ in Pulse.Prime(() => new ParseState())
        from __ in Pulse.FirstOf(
            (() => token.Kind == TokenKind.Number, /**/ () => Manipulate((a, ctx) => ctx.PushNumber(a.Text))),
            (() => token.IsOperator,               /**/ () => Manipulate((a, ctx) => ctx.PushOperator(a.Kind))),
            (() => token.Kind == TokenKind.LParen, /**/ () => Manipulate(ctx => ctx.OpenParen())),
            (() => token.Kind == TokenKind.RParen, /**/ () => Manipulate(ctx => ctx.CloseParen()))
        )
        select token;

    public static AstNode Parse(IEnumerable<Token> tokens) =>
        Signal.From(flow)
            .SetArtery(TheCollector.Exhibits<AstNode>())
            .Pulse(tokens)
            .FlatLine(Pulse.Trace<ParseState>(a => a.Finish()))
            .GetArtery<Collector<AstNode>>()
            .TheExhibit.Last();
}

