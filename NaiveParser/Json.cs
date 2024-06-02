using System.Collections;
using System.Reflection;

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
        else if (obj is null || IsNumeric(obj)) ret = obj?.ToString() ?? "null";
        else if (obj is IEnumerable<object?> list)
            ret = $"[{string.Join(", ", list.Select(o => o?.ToString() ?? "null"))}]";
        else if (obj is Dictionary<string, object?> map)
            ret = $"{{{string.Join(", ", map.Select(pair => $"{pair.Key}: {pair.Value?.ToString() ?? "null"}"))}}}";
        else
            ret = $"{{{string.Join(", ", obj.GetType().GetFields(BindingFlags.Public)
                .Select(f => $"{f.Name}: {f.GetValue(obj)?.ToString() ?? "null"}"))}}}";

        return ret;
    }
}