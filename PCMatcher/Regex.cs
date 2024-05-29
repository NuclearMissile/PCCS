namespace PCMatcher;

using static IMatcher;

public class Regex
{
    public static IMatcher Parse(string expr)
    {
        var index = 0;
        return ParseExpr(expr, ref index);
    }

    /*
     * expr = term ('|' term)*
     */
    private static IMatcher ParseExpr(string expr, ref int index)
    {
        var m = ParseTerm(expr, ref index);
        while (index < expr.Length && expr[index] == '|')
        {
            index++;
            m = m.Or(ParseTerm(expr, ref index));
        }

        return m;
    }

    /*
     * term = factor+
     */
    private static IMatcher ParseTerm(string term, ref int index)
    {
        var m = ParseFactor(term, ref index);
        while (index < term.Length && term[index] != ')' && term[index] != '|')
        {
            m = m.And(ParseFactor(term, ref index));
        }

        return m;
    }

    /*
     * factor = elem '*'
     *        | elem '+'
     *        | elem
     */
    private static IMatcher ParseFactor(string factor, ref int index)
    {
        var m = ParseElem(factor, ref index);
        if (index < factor.Length)
        {
            switch (factor[index])
            {
                case '*':
                    index++;
                    return m.Many0();
                case '+':
                    index++;
                    return m.Many1();
            }
        }

        return m;
    }

    /*
     * elem = '(' expr ')'
     *      | '[' range ']'
     *      | '.'
     *      | '\' char
     *      | char
     */
    private static IMatcher ParseElem(string elem, ref int index)
    {
        switch (elem[index])
        {
            case '(':
                index++;
                var m = ParseExpr(elem, ref index);
                Consume(elem, ref index, ')');
                return m;
            case '[':
                index++;
                m = ParseRange(elem, ref index);
                Consume(elem, ref index, ']');
                return m;
            case '.':
                index++;
                return Any;
            case '\\':
                index++;
                var ch = Ch(elem[index]);
                index++;
                return ch;
            default:
                ch = Ch(elem[index]);
                index++;
                return ch;
        }
    }

    /*
     * range = rangeItem+
     */
    private static IMatcher ParseRange(string range, ref int index)
    {
        var m = ParseRangeItem(range, ref index);
        while (index < range.Length && range[index] != ']')
        {
            m = m.Or(ParseRangeItem(range, ref index));
        }

        return m;
    }

    /*
     * rangeItem = char '-' char
     *           | char
     */
    private static IMatcher ParseRangeItem(string rangeItem, ref int index)
    {
        var ch = rangeItem[index];
        index++;
        if (rangeItem[index] == '-')
        {
            index++;
            var r = Range(ch, rangeItem[index]);
            index++;
            return r;
        }

        return Ch(ch);
    }

    private static void Consume(string expr, ref int index, char expected)
    {
        if (index >= expr.Length || expr[index] != expected)
        {
            throw new ArgumentException($"{expected} expected");
        }

        index++;
    }
}