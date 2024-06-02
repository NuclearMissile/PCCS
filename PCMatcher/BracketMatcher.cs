using static PCMatcher.IMatcher;

namespace PCMatcher;

/*
 * expr = term+
 * term = ()
 *      | '(' expr ')'
 */
public class BracketMatcher
{
    private static readonly IMatcher Term = OneOf(Str("()"), Seq(Ch('('), Lazy(() => Expr), Ch(')')));

    private static readonly IMatcher Expr = Term.Many1();

    public static bool Match(string s) => Expr.Match(s);
}