using RegularParser.AstConstruction.AstNodes;
using RegularParser.Lexing;

namespace RegularParser.AstConstruction;

public sealed class Parser
{
    public static AstNode Parse(IEnumerable<Token> tokens)
    {
        var state = new ParseState();

        foreach (var token in tokens)
        {
            if (token.Kind == TokenKind.Number)
            {
                state = state.PushNumber(token.Text);
                continue;
            }

            if (token.IsOperator)
            {
                state = state.PushOperator(token.Kind);
                continue;
            }

            if (token.Kind == TokenKind.LParen)
            {
                state = state.OpenParen();
                continue;
            }

            if (token.Kind == TokenKind.RParen)
            {
                state = state.CloseParen();
                continue;
            }
        }
        return state.Finish();
    }
}
