using System.Globalization;
using System.Reflection;
using System.Text;
using static System.Char;

namespace NaiveParser;

public class Json
{
    private static bool IsNumeric(object o) => Type.GetTypeCode(o.GetType()) switch
    {
        TypeCode.Byte or TypeCode.SByte or TypeCode.UInt16 or TypeCode.UInt32 or TypeCode.UInt64 or TypeCode.Int16
            or TypeCode.Int32 or TypeCode.Int64 or TypeCode.Decimal or TypeCode.Double or TypeCode.Single => true,
        _ => false
    };

    public static string ToJson(object? obj)
    {
        string ret;

        if (obj is string) ret = $"\"{obj}\"";
        else if (obj is null) ret = "null";
        else if (obj is bool b) ret = b ? "true" : "false";
        else if (IsNumeric(obj)) ret = obj.ToString()!;
        else if (obj is IEnumerable<object?> list)
            ret = $"[{string.Join(", ", list.Select(ToJson))}]";
        else if (obj is Dictionary<string, object?> map)
            ret = $"{{{string.Join(", ", map.Select(pair => $"\"{pair.Key}\": {ToJson(pair.Value)}"))}}}";
        else
            ret = $"{{{string.Join(", ", obj.GetType().GetFields(BindingFlags.Public)
                .Select(f => $"\"{f.Name}\": {ToJson(f.GetValue(obj))}"))}}}";

        return ret;
    }

    private string _input = "";
    private int _pos = 0;

    public object? Parse(string json)
    {
        _input = json;
        _pos = 0;
        SkipWhitespace();
        return ParseValue();
    }

    private object? ParseValue()
    {
        var peek = Peek();
        return peek switch
        {
            null => throw new ArgumentException($"Unexpected EOF"),
            '{' => ParseObject(),
            '[' => ParseArray(),
            '"' => ParseString(),
            't' or 'f' => ParseBoolean(),
            'n' => ParseNull(),
            _ when "0123456789-.".Contains(peek.Value) => ParseNumber(),
            _ => throw new ArgumentException($"Unexpected character: {peek}")
        };
    }

    private Dictionary<string, object?> ParseObject()
    {
        var ret = new Dictionary<string, object?>();
        Match('{');
        SkipWhitespace();
        while (Peek() != '}')
        {
            var key = ParseString();
            SkipWhitespace();
            Match(':');
            SkipWhitespace();
            var value = ParseValue();
            ret[key] = value;
            SkipWhitespace();
            if (Peek() == ',')
            {
                Next();
                SkipWhitespace();
            }
        }
        Match('}');
        return ret;
    }

    private List<object?> ParseArray()
    {
        var ret = new List<object?>();
        Match('[');
        SkipWhitespace();
        while (Peek() != ']')
        {
            ret.Add(ParseValue());
            SkipWhitespace();
            if (Peek() == ',')
            {
                Next();
                SkipWhitespace();
            }
        }

        Match(']');
        return ret;
    }

    private string ParseString()
    {
        Match('"');
        var sb = new StringBuilder();
        while (Peek() != '"')
        {
            var next = Next();
            if (next == '\\')
            {
                var escaped = Next();
                switch (escaped)
                {
                    case '"':
                        sb.Append('"');
                        break;
                    case '\\':
                        sb.Append('\\');
                        break;
                    case '/':
                        sb.Append('/');
                        break;
                    case 'b':
                        sb.Append('\b');
                        break;
                    case 'f':
                        sb.Append('\f');
                        break;
                    case 'n':
                        sb.Append('\n');
                        break;
                    case 'r':
                        sb.Append('\r');
                        break;
                    case 't':
                        sb.Append('\t');
                        break;
                    case 'u':
                        var unicode = string.Join("", Enumerable.Repeat(0, 4).Select(_ => Next()));
                        sb.Append((char) Convert.ToUInt16(unicode, 16));
                        break;
                    default:
                        throw new ArgumentException($"Invalid escape character: {escaped}");
                }
            }
            else
            {
                sb.Append(next);
            }
        }

        Match('"');
        return sb.ToString();
    }

    private bool ParseBoolean()
    {
        var peek = Peek();
        switch (peek)
        {
            case 't':
                Match("true");
                return true;
            case 'f':
                Match("false");
                return false;
            default:
                throw new ArgumentException($"Expected 't' or 'f', but got '{peek}");
        }
    }

    private object? ParseNull()
    {
        Match("null");
        return null;
    }

    private object ParseNumber()
    {
        var start = _pos;
        while ("1234567890+-.eE".Contains(Peek().GetValueOrDefault('$')))
        {
            Next();
        }

        var numberString = _input.AsSpan(new Range(start, _pos));
        return numberString.Contains('.') ? double.Parse(numberString) : int.Parse(numberString);
    }

    private char? Peek() => _pos >= _input.Length ? null : _input[_pos];

    private char Next()
    {
        var nextChar = Peek() ?? throw new ArgumentException("Unexpected EOF");
        _pos++;
        return nextChar;
    }

    private void Match(char expected)
    {
        var nextChar = Next();
        if (nextChar != expected)
            throw new ArgumentException($"Expected '{expected}' but got '{nextChar}'");
    }

    private void Match(string expected)
    {
        foreach (var c in expected)
        {
            Match(c);
        }
    }

    private void SkipWhitespace()
    {
        while (IsWhiteSpace(Peek() ?? '$'))
        {
            Next();
        }
    }
}