using static System.Char;
using static System.Math;

namespace NaiveParser;

public class Calculator
{
    private string _input = "";
    private int _pos = 0;
    private IDictionary<string, double> consts = new Dictionary<string, double>();

    private IDictionary<string, Func<IList<double>, double>> funcs =
        new Dictionary<string, Func<IList<double>, double>>();

    public Calculator RegisterConstant(string name, double value)
    {
        consts[name] = value;
        return this;
    }

    public Calculator RegisterFunc(string name, Func<IList<double>, double> func)
    {
        funcs[name] = func;
        return this;
    }

    public double Calculate(string expr)
    {
        _input = expr;
        _pos = 0;
        var ret = Parse0();
        return _pos == _input.Length ? ret : throw new ArgumentException($"unexpected character: {Peek()}");
    }

    private char? Peek() => _pos >= _input.Length ? null : _input[_pos];

    private char? Next()
    {
        _pos++;
        return Peek();
    }

    private bool Match(char expected)
    {
        if (Peek() != expected) return false;
        Next();
        return true;
    }

    private void SkipWhitespace()
    {
        while (IsWhiteSpace(Peek() ?? '$'))
        {
            Next();
        }
    }

    private double Parse0()
    {
        var result = Parse1();
        SkipWhitespace();
        while (true)
        {
            if (Match('+')) result += Parse1();
            else if (Match('-')) result -= Parse1();
            else return result;
        }
    }

    private double Parse1()
    {
        var result = Parse2();
        SkipWhitespace();
        while (true)
        {
            if (Match('*')) result *= Parse2();
            else if (Match('/')) result /= Parse2();
            else return result;
        }
    }

    private double Parse2()
    {
        var result = Parse3();
        SkipWhitespace();
        while (true)
        {
            if (Match('^')) result = Pow(result, Parse3());
            else return result;
        }
    }

    private double Parse3()
    {
        SkipWhitespace();

        double result;
        if (Match('('))
        {
            result = Parse0();
            if (!Match(')')) throw new ArgumentException("Expected ')'");
        }
        else if (Match('-'))
        {
            if (IsDigit(Peek() ?? '$') || Peek() == '.')
                result = -ParseNumber();
            else if (IsLetter(Peek() ?? '$'))
                result = -ParseFuncOrConst();
            else
                throw new ArgumentException($"Unexpected character: {Peek()}");
        }
        else if (IsDigit(Peek() ?? '$') || Peek() == '.')
            result = ParseNumber();
        else if (IsLetter(Peek() ?? '$'))
            result = ParseFuncOrConst();
        else throw new ArgumentException($"Unexpected character: {Peek()}");

        SkipWhitespace();
        return result;
    }

    private double ParseFuncOrConst()
    {
        var start = _pos;
        while (IsLetterOrDigit(Peek() ?? '$'))
        {
            Next();
        }

        var name = _input.Substring(start, _pos - start);

        if (Match('('))
        {
            var args = new List<double>();
            if (!Match(')'))
            {
                do
                {
                    args.Add(Parse0());
                } while (Match(','));

                if (!Match(')')) throw new ArgumentException("Expected ')' after function arguments");
            }

            try
            {
                return funcs[name](args);
            }
            catch (KeyNotFoundException)
            {
                throw new AggregateException($"Undefined func: {name}");
            }
        }
        else
        {
            try
            {
                return consts[name];
            }
            catch (KeyNotFoundException)
            {
                throw new AggregateException($"Undefined func: {name}");
            }
        }
    }

    private double ParseNumber()
    {
        var start = _pos;
        while (IsDigit(Peek() ?? '$') || Peek() == '.')
        {
            Next();
        }

        return double.Parse(_input.Substring(start, _pos - start));
    }
}