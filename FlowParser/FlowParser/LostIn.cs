using FlowParser.AstConstruction;
using FlowParser.AstConstruction.AstNodes;
using FlowParser.Lexing;

namespace FlowParser;

public class LostIn
{
    public static AstNode Translation(string input)
        => Parser.Parse(Lexer.Tokenize(input));
}