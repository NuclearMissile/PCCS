using static PCMatcher.IMatcher;

namespace PCMatcher;

/*
 * json = number | string | bool | arr | obj
 * number = integer | decimal | '-' integer | '-' decimal
 * integer = [0-9]+
 * decimal = [0-9]* '.' [0-9]+
 * string = '"' (.*) '"'
 * bool = "true" | "false"
 * arr = "[]" | '[' json (',' json)* ']'
 * field = string : json
 * obj = "{}" | '{' field (',' field)* '}'
 */
public static class JsonMatcher
{
    private static readonly IMatcher Blank = Chs(' ', '\t', '\n', '\r').Many0();

    private static readonly IMatcher LeftBrace = Ch('{').WithBlank();
    private static readonly IMatcher RightBrace = Ch('}').WithBlank();
    private static readonly IMatcher LeftSquareBracket = Ch('[').WithBlank();
    private static readonly IMatcher RightSquareBracket = Ch(']').WithBlank();
    private static readonly IMatcher Colon = Ch(':').WithBlank();
    private static readonly IMatcher Comma = Ch(',').WithBlank();

    private static readonly IMatcher Json = OneOf(
        Lazy(() => Number), Lazy(() => String), Lazy(() => Bool), Lazy(() => Arr), Lazy(() => Obj)
    );

    private static readonly IMatcher Integer = Range('0', '9').Many1();
    private static readonly IMatcher Decimal = Seq(Range('0', '9').Many0(), Ch('.'), Range('0', '9').Many1());
    private static readonly IMatcher Number = OneOf(Integer, Decimal, Ch('-').And(Integer), Ch('-').And(Decimal));
    private static readonly IMatcher String = Seq(Ch('"'), Not('"').Many0(), Ch('"'));
    private static readonly IMatcher Bool = Strs("true", "false");

    private static readonly IMatcher Arr = OneOf(
        LeftSquareBracket.And(RightSquareBracket),
        Seq(LeftSquareBracket, Json.And(Comma.And(Json).Many0()), RightSquareBracket)
    );

    private static readonly IMatcher Field = Seq(String, Colon, Json);

    private static readonly IMatcher Obj = OneOf(
        LeftBrace.And(RightBrace),
        Seq(LeftBrace, Field.And(Comma.And(Field).Many0()), RightBrace)
    );

    private static IMatcher WithBlank(this IMatcher m) => Seq(Blank, m, Blank);
    private static IMatcher Not(char c) => Ch(ch => ch != c);

    public static bool Match(string s) => Json.Match(s);
}