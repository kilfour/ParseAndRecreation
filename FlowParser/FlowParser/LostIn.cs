using Definitions.AstNodes;

namespace FlowParser;

public class LostIn
{
    public static AstNode Translation(string input)
        => Parser.Parse(Lexer.Tokenize(input));
}