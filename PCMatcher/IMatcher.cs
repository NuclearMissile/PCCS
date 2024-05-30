namespace PCMatcher;

public interface IMatcher
{
    private class MatcherImpl(Func<string, int, ISet<int>> parseFunc) : IMatcher
    {
        public Func<string, int, ISet<int>> ParseFunc { get; } = parseFunc;
    }

    Func<string, int, ISet<int>> ParseFunc { get; }

    bool Match(string s) => ParseFunc(s, 0).Contains(s.Length);

    static IMatcher Any => Ch(c => true);

    static IMatcher Ch(char c) => Ch(ch => ch == c);

    static IMatcher Chs(params char[] chs) => Ch(chs.Contains);

    static IMatcher Not(char c) => Ch(ch => ch != c);

    static IMatcher Nots(params char[] chs) => Ch(c => !chs.Contains(c));

    static IMatcher Range(char c1, char c2) => Ch(c => (c - c1) * (c - c2) <= 0);

    static IMatcher Ch(Predicate<char> predicate) => new MatcherImpl(
        (s, index) => index < s.Length && predicate(s[index]) ? [index + 1] : new HashSet<int>()
    );

    static IMatcher Str(string str) => new MatcherImpl(
        (s, index) => s.AsSpan(index).StartsWith(str) ? [index + str.Length] : new HashSet<int>()
    );

    static IMatcher Lazy(Func<IMatcher> supplier)
        => new MatcherImpl((s, index) => supplier().ParseFunc(s, index));

    static IMatcher Strs(string s1, string s2, params string[] strs)
        => strs.Select(Str).Aggregate(Str(s1).Or(Str(s2)), (acc, m) => acc.Or(m));

    static IMatcher Seq(IMatcher m1, IMatcher m2, params IMatcher[] matchers)
        => matchers.Aggregate(m1.And(m2), (acc, m) => acc.And(m));

    static IMatcher OneOf(IMatcher m1, IMatcher m2, params IMatcher[] matchers)
        => matchers.Aggregate(m1.Or(m2), (acc, m) => acc.Or(m));

    IMatcher Repeat(int minTimes, int maxTimes) => new MatcherImpl((s, index) =>
    {
        var ret = new HashSet<int> { index };
        for (var i = 0; i < minTimes; i++)
            ret = ret.SelectMany(idx => ParseFunc(s, idx)).ToHashSet();

        var queue = new Queue<int>(ret);
        var times = minTimes;
        while (queue.Count > 0 && times < maxTimes)
        {
            var cnt = queue.Count;
            while (cnt-- > 0)
                foreach (var i in ParseFunc(s, queue.Dequeue()))
                {
                    if (!ret.Add(i)) continue;
                    queue.Enqueue(i);
                }

            times++;
        }

        return ret;
    });

    IMatcher Repeat(int times) => Repeat(times, times);

    IMatcher And(IMatcher rhs) => new MatcherImpl((s, index) =>
    {
        var ret = new HashSet<int>();
        foreach (var i in ParseFunc(s, index))
            ret.UnionWith(rhs.ParseFunc(s, i));

        return ret;
    });

    IMatcher And(char c) => And(Ch(c));

    IMatcher And(string s) => And(Str(s));

    IMatcher Or(IMatcher rhs) => new MatcherImpl((s, index) =>
    {
        var ret = new HashSet<int>(ParseFunc(s, index));
        ret.UnionWith(rhs.ParseFunc(s, index));
        return ret;
    });

    IMatcher Or(char c) => Or(Ch(c));

    IMatcher Or(string s) => Or(Str(s));

    IMatcher Many(int minTimes) => Repeat(minTimes, int.MaxValue);

    IMatcher Many0() => Repeat(0, int.MaxValue);

    IMatcher Many1() => Repeat(1, int.MaxValue);

    IMatcher FlatMap(Func<string, IMatcher> matcher) => new MatcherImpl((s, index) =>
    {
        var ret = new HashSet<int>();
        foreach (var i in ParseFunc(s, index))
        {
            var matchStr = s.Substring(index, i - index);
            var next = matcher(matchStr);
            ret.UnionWith(next.ParseFunc(s, i));
        }

        return ret;
    });
}