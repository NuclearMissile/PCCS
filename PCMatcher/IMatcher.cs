namespace PCCS;

public interface IMatcher
{
    private class MatcherImpl(Func<string, int, ISet<int>> parseFunc) : IMatcher
    {
        public Func<string, int, ISet<int>> ParseFunc { get; } = parseFunc;
    }

    Func<string, int, ISet<int>> ParseFunc { get; }

    bool Match(string s) => ParseFunc(s, 0).Any(i => i == s.Length);

    static IMatcher Any => Ch(c => true);

    static IMatcher Ch(char c) => Ch(ch => ch == c);

    static IMatcher Chs(params char[] chs) => Ch(new HashSet<char>(chs).Contains);

    static IMatcher Not(char c) => Ch(ch => ch != c);

    static IMatcher Range(char c1, char c2) => Ch(c => (c - c1) * (c - c2) <= 0);

    static IMatcher Ch(Predicate<char> predicate) => new MatcherImpl(
        (s, index) => index < s.Length && predicate(s[index]) ? [index + 1] : new HashSet<int>()
    );

    static IMatcher Str(string str) => new MatcherImpl(
        (s, index) => s[index..].StartsWith(str) ? [index + str.Length] : new HashSet<int>()
    );

    static IMatcher Strs(string s1, string s2, params string[] strs)
        => strs.Select(Str).Aggregate(Str(s1).Or(Str(s2)), (acc, m) => acc.Or(m));


    static IMatcher Lazy(Func<IMatcher> supplier)
        => new MatcherImpl((s, index) => supplier().ParseFunc(s, index));

    static IMatcher Seq(IMatcher m1, IMatcher m2, params IMatcher[] matchers)
        => matchers.Aggregate(m1.And(m2), (acc, m) => acc.And(m));

    static IMatcher OneOf(IMatcher m1, IMatcher m2, params IMatcher[] matchers)
        => matchers.Aggregate(m1.Or(m2), (acc, m) => acc.Or(m));

    IMatcher Repeat(int minTimes, int maxTimes)
    {
        return new MatcherImpl((s, index) =>
        {
            var set = new HashSet<int> { index };
            for (var i = 0; i < minTimes; i++) set = set.SelectMany(idx => ParseFunc(s, idx)).ToHashSet();

            var result = new HashSet<int>(set);
            var queue = new Queue<int>(set);
            var times = minTimes;
            while (queue.Count > 0 && times < maxTimes)
            {
                var cnt = queue.Count;
                while (cnt-- > 0)
                    foreach (var i in ParseFunc(s, queue.Dequeue()))
                    {
                        if (!result.Add(i)) continue;
                        queue.Enqueue(i);
                    }

                times++;
            }

            return result;
        });
    }

    IMatcher Repeat(int times) => Repeat(times, times);

    IMatcher And(IMatcher rhs)
    {
        return new MatcherImpl((s, index) =>
        {
            var result = new HashSet<int>();
            foreach (var i in ParseFunc(s, index)) result.UnionWith(rhs.ParseFunc(s, i));

            return result;
        });
    }

    IMatcher And(char c) => And(Ch(c));

    IMatcher And(string s) => And(Str(s));

    IMatcher Or(IMatcher rhs)
    {
        return new MatcherImpl((s, index) =>
        {
            var result = new HashSet<int>(ParseFunc(s, index));
            result.UnionWith(rhs.ParseFunc(s, index));
            return result;
        });
    }

    IMatcher Or(char c) => Or(Ch(c));

    IMatcher Or(string s) => Or(Str(s));

    IMatcher Many(int minTimes)
    {
        return new MatcherImpl((s, index) =>
        {
            var set = new HashSet<int> { index };
            for (var i = 0; i < minTimes; i++) set = set.SelectMany(idx => ParseFunc(s, idx)).ToHashSet();

            var queue = new Queue<int>(set);
            var result = new HashSet<int>(set);
            while (queue.Count > 0)
                foreach (var i in ParseFunc(s, queue.Dequeue()))
                {
                    if (!result.Add(i)) continue;
                    queue.Enqueue(i);
                }

            return result;
        });
    }

    IMatcher Many0() => Many(0);

    IMatcher Many1() => Many(1);

    IMatcher FlatMap(Func<string, IMatcher> mapper)
    {
        return new MatcherImpl((s, index) =>
        {
            var result = new HashSet<int>();
            foreach (var i in ParseFunc(s, index))
            {
                var matchStr = s.Substring(index, i - index);
                var next = mapper(matchStr);
                result.UnionWith(next.ParseFunc(s, i));
            }

            return result;
        });
    }
}