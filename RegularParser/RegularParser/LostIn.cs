using RegularParser.AstConstruction;
using RegularParser.AstConstruction.AstNodes;
using RegularParser.Lexing;

namespace RegularParser;

public class LostIn
{
    public static AstNode Translation(string input)
        => Parser.Parse(Lexer.Tokenize(input));
}