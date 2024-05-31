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
            Consume(expr, ref index);
            m = m.Or(ParseTerm(expr, ref index));
        }

        return m;
    }

    /*
     * term = factor+
     */
    private static IMatcher ParseTerm(string expr, ref int index)
    {
        var m = ParseFactor(expr, ref index);
        while (index < expr.Length && expr[index] != ')' && expr[index] != '|')
        {
            m = m.And(ParseFactor(expr, ref index));
        }

        return m;
    }

    /*
     * factor = elem '*'
     *        | elem '+'
     *        | elem '?'
     *        | elem repeat
     *        | elem
     */
    private static IMatcher ParseFactor(string expr, ref int index)
    {
        var m = ParseElem(expr, ref index);
        if (index < expr.Length)
        {
            switch (expr[index])
            {
                case '*':
                    Consume(expr, ref index);
                    return m.Many0();
                case '+':
                    Consume(expr, ref index);
                    return m.Many1();
                case '?':
                    Consume(expr, ref index);
                    return m.Repeat(0, 1);
                case '{':
                    var (num1, num2) = ParseRepeat(expr, ref index);
                    return num2 == -1 ? m.Repeat(num1) : m.Repeat(num1, num2);
            }
        }

        return m;
    }

    /*
     * repeat = '{' num1 '}'
     *        | '{' num1 ',' num2 '}'
     */
    private static (int, int) ParseRepeat(string expr, ref int index)
    {
        Consume(expr, ref index, '{');
        var start = index;
        while ("1234567890".Contains(Peek(expr, index)))
        {
            Consume(expr, ref index);
        }

        var num1 = int.Parse(expr.Substring(start, index - start));
        switch (Peek(expr, index))
        {
            case '}':
                Consume(expr, ref index, '}');
                return (num1, -1);
            case ',':
                Consume(expr, ref index, ',');
                start = index;
                while ("1234567890".Contains(Peek(expr, index)))
                {
                    Consume(expr, ref index);
                }

                var num2 = int.Parse(expr.Substring(start, index - start));
                Consume(expr, ref index, '}');
                return (num1, num2);
            default:
                throw new ArgumentException($"unexpected {Peek(expr, index)}");
        }
    }

    /*
     * elem = '(' expr ')'
     *      | '[' range ']'
     *      | '.'
     *      | '\' char
     *      | char
     */
    private static IMatcher ParseElem(string expr, ref int index)
    {
        switch (expr[index])
        {
            case '(':
                Consume(expr, ref index);
                var m = ParseExpr(expr, ref index);
                Consume(expr, ref index, ')');
                return m;
            case '[':
                Consume(expr, ref index);
                m = ParseRange(expr, ref index);
                Consume(expr, ref index, ']');
                return m;
            case '.':
                Consume(expr, ref index);
                return Any;
            case '\\':
                Consume(expr, ref index);
                switch (Peek(expr, index))
                {
                    case 'w':
                        Consume(expr, ref index);
                        return Range('A', 'Z').Or(Range('a', 'z')).Or(Range('0', '9'));
                    case 'd':
                        Consume(expr, ref index);
                        return Range('0', '9');
                    case 's':
                        Consume(expr, ref index);
                        return Chs(' ', '\f', '\n', '\r', '\t', '\v');
                    case 'f':
                        Consume(expr, ref index);
                        return Ch('\f');
                    case 'n':
                        Consume(expr, ref index);
                        return Ch('\n');
                    case 'r':
                        Consume(expr, ref index);
                        return Ch('\r');
                    default:
                        var escaped = Ch(expr[index]);
                        Consume(expr, ref index);
                        return escaped;
                }
            default:
                var ch = Ch(expr[index]);
                Consume(expr, ref index);
                return ch;
        }
    }

    /*
     * range = rangeItem+
     */
    private static IMatcher ParseRange(string expr, ref int index)
    {
        var m = ParseRangeItem(expr, ref index);
        while (index < expr.Length && expr[index] != ']')
        {
            m = m.Or(ParseRangeItem(expr, ref index));
        }

        return m;
    }

    /*
     * rangeItem = char '-' char
     *           | char
     */
    private static IMatcher ParseRangeItem(string expr, ref int index)
    {
        var ch = expr[index];
        Consume(expr, ref index);
        if (expr[index] == '-')
        {
            Consume(expr, ref index);
            var r = Range(ch, expr[index]);
            Consume(expr, ref index);
            return r;
        }

        return Ch(ch);
    }

    private static char Peek(string expr, int index)
    {
        if (index >= expr.Length)
        {
            throw new ArgumentException("unexpected EOF");
        }

        return expr[index];
    }

    private static void Consume(string expr, ref int index, char? expected = null)
    {
        if (index >= expr.Length)
        {
            throw new ArgumentException("unexpected EOF");
        }

        if (expected != null && expr[index] != expected)
        {
            throw new ArgumentException($"{expected} expected");
        }

        index++;
    }
}