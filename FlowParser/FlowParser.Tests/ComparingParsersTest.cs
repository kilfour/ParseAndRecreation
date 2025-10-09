using Definitions.Lexing;
using QuickAcid;
using QuickFuzzr;
using QuickFuzzr.UnderTheHood;
using QuickPulse.Show;

namespace FlowParser.Tests;

public class ComparingParsersTest
{
    [Fact]
    public void Check()
    {
        var script =
            from expression in "expr".Input(Expression(2), a => a.ReduceWith(Reducer))
            from traceExpression in "original expr".Trace(() => expression)
            from rpResult in "RP".ActCarefully(() => RegularParser.LostIn.Translation(expression).Eval())
            from fpResult in "FP".Act(() => FlowParser.LostIn.Translation(expression).Eval())
            from traceResult in "RP vs FP".Trace(() => rpResult.Value.ToString() + " /= " + fpResult.ToString())
            from noExc in "RP does not throw".Spec(() => !rpResult.Threw)
            from match in "Match".SpecIf(() => !rpResult.Threw, () => fpResult == rpResult.Value)
            select Acid.Test;
        QState.Run(script, 215942572)
            .Options(a => a with { FileAs = "RPvsFP" })//, Diagnose = WriteData.ToFile() })
            .With(20.Runs())
            .AndOneExecutionPerRun();
    }

    [Fact]
    public void Generation()
    {
        Expression(10).Many(20).Generate().PulseToLog("generated.log");
    }

    private static readonly Generator<char> Digit = Fuzz.ChooseFromThese("0123456789".ToCharArray());
    private static readonly Generator<char> NonZero = Fuzz.ChooseFromThese("123456789".ToCharArray());

    public static readonly Generator<string> Number = Fuzz.Int(1, 4).AsString();

    private static readonly Generator<string> AddOp = Fuzz.ChooseFromThese("+", "-");
    private static readonly Generator<string> MulOp = Fuzz.ChooseFromThese("*", "/");
    private static readonly Generator<string> PowOp = Fuzz.Constant("^");

    private static readonly Generator<string> WS =
        from chs in Fuzz.Constant(' ').Many(0, 3)
        select new string([.. chs]);

    private static readonly Generator<string> UnaryOp = Fuzz.ChooseFromThese("-");

    private static Generator<string> Primary(int maxDepth) =>
        maxDepth <= 0
        ? Number
        : Fuzz.ChooseGenerator(
            Number,
            from l in Fuzz.Constant("(")
            from e in Expr(maxDepth - 1)
            from r in Fuzz.Constant(")")
            select l + e + r,
            from op in UnaryOp
            from ws in WS
            from inner in Primary(maxDepth - 1)
            select op + ws + inner
          );

    private static Generator<string> Factor(int maxDepth) =>
        from a in Primary(maxDepth)
        from tail in maxDepth <= 0
            ? Fuzz.Constant("")
            : Fuzz.ChooseGenerator(
                  Fuzz.Constant(""),
                  from w1 in WS
                  from op in PowOp
                  from w2 in WS
                  from b in Factor(maxDepth - 1)
                  select w1 + op + w2 + b)
        select a + tail;

    private static Generator<string> Term(int maxDepth) =>
        from head in Factor(maxDepth)
        from pieces in (
            from w1 in WS
            from op in MulOp
            from w2 in WS
            from rhs in Factor(maxDepth - 1)
            select w1 + op + w2 + rhs
        ).Many(0, 3).ToArray()
        select head + string.Concat(pieces);

    private static Generator<string> Expr(int maxDepth) =>
        from depth in Fuzz.Int(0, maxDepth)
        from head in Term(depth)
        from pieces in (
            from w1 in WS
            from op in AddOp
            from w2 in WS
            from rhs in Term(depth - 1)
            select w1 + op + w2 + rhs
        ).Many(0, 3).ToArray()
        select head + string.Concat(pieces);

    public static Generator<string> Expression(int maxDepth = 4) => Expr(maxDepth);

    private IEnumerable<string> Reducer(string expr)
    {
        var tokens = Lexer.Tokenize(expr);
        tokens = tokens.Where(a => a.Kind != TokenKind.LParen && a.Kind != TokenKind.RParen);
        yield return TokensToString(tokens);


        var list = tokens.ToList();
        for (int i = 0; i < list.Count; i++)
        {
            if (list[i].IsOperator && !list[i + 1].IsOperator)
                yield return TokensToString(tokens.Skip(i + 1));
        }

        for (int i = list.Count - 1; i > 0; i--)
        {
            if (list[i].IsOperator && !list[i - 1].IsOperator)
                yield return TokensToString(tokens.Take(i));
        }

    }

    private string TokensToString(IEnumerable<Token> tokens)
        => string.Join("", tokens.Select(a => a.Text));
}