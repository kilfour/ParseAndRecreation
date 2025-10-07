using Definitions.AstNodes;
using RegularParser.AstConstruction;
using RegularParser.Lexing;

namespace RegularParser;

public class LostIn
{
    public static AstNode Translation(string input)
        => new Parser().Parse(Lexer.Tokenize(input));
}