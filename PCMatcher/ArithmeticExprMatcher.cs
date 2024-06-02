using static PCMatcher.IMatcher;

namespace PCMatcher;

/*
 * expr = term ('+'|'-' term)*
 * term = factor ('*'|'/' factor)*
 * factor = [0-9]+
 *        | '-' factor
 *        | '(' expr ')'
 */
public class ArithmeticExprMatcher
{
    private static readonly IMatcher Factor = OneOf(
        Range('0', '9').Many1(),
        Seq(Ch('-'), Lazy(() => Factor)),
        Seq(Ch('('), Lazy(() => Expr), Ch(')'))
    );

    private static readonly IMatcher Term = Seq(Factor, Chs('*', '/').And(Factor).Many0());
    private static readonly IMatcher Expr = Seq(Term, Chs('+', '-').And(Term).Many0());

    public static bool Match(string s) => Expr.Match(s);
}