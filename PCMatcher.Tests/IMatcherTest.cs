using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static PCMatcher.IMatcher;

namespace PCMatcher.Tests;

[TestClass]
[TestSubject(typeof(IMatcher))]
// ReSharper disable once InconsistentNaming
public class IMatcherTest
{
    [TestMethod]
    public void TestCh()
    {
        var m = Ch('a');
        IsTrue(m.Match("a"));
        IsFalse(m.Match("b"));
        IsFalse(m.Match("aa"));
        IsFalse(m.Match("bb"));
    }

    [TestMethod]
    public void TestChs()
    {
        var m = Chs('a', 'b', 'c');
        IsTrue(m.Match("a"));
        IsTrue(m.Match("b"));
        IsTrue(m.Match("c"));
        IsFalse(m.Match("d"));
        IsFalse(m.Match("aa"));
        IsFalse(m.Match("bb"));
    }

    [TestMethod]
    public void TestAny()
    {
        var m = Any;
        IsTrue(m.Match("a"));
        IsTrue(m.Match("b"));
        IsTrue(m.Match("c"));
        IsTrue(m.Match("d"));
        IsFalse(m.Match("aa"));
        IsFalse(m.Match("bb"));
    }

    [TestMethod]
    public void TestRange()
    {
        var m = Range('0', '3');
        IsTrue(m.Match("0"));
        IsTrue(m.Match("1"));
        IsTrue(m.Match("2"));
        IsTrue(m.Match("3"));
        IsFalse(m.Match("9"));
        IsFalse(m.Match("a"));
    }

    [TestMethod]
    public void TestNot()
    {
        var m = Not('a');
        IsTrue(m.Match("b"));
        IsFalse(m.Match("a"));
        IsFalse(m.Match("bb"));
    }

    [TestMethod]
    public void TestStr()
    {
        var m = Str("abc");
        IsFalse(m.Match(""));
        IsFalse(m.Match("a"));
        IsFalse(m.Match("ab"));
        IsTrue(m.Match("abc"));
        IsFalse(m.Match("abcd"));
    }

    [TestMethod]
    public void TestAnd()
    {
        var m = Ch('a').And('b');
        IsTrue(m.Match("ab"));
        IsFalse(m.Match("a"));
        IsFalse(m.Match("b"));
        IsFalse(m.Match("abc"));
        IsFalse(m.Match(""));

        m = Ch('a').And(Ch('b'));
        IsTrue(m.Match("ab"));
        IsFalse(m.Match("a"));
        IsFalse(m.Match("b"));
        IsFalse(m.Match("abc"));
        IsFalse(m.Match(""));

        m = Ch('a').And("b");
        IsTrue(m.Match("ab"));
        IsFalse(m.Match("a"));
        IsFalse(m.Match("b"));
        IsFalse(m.Match("abc"));
        IsFalse(m.Match(""));
    }

    [TestMethod]
    public void TestOr()
    {
        var m = Ch('a').Or('b');
        IsTrue(m.Match("a"));
        IsTrue(m.Match("b"));
        IsFalse(m.Match("ab"));
        IsFalse(m.Match("ac"));
        IsFalse(m.Match(""));

        m = Ch('a').Or(Ch('b'));
        IsTrue(m.Match("a"));
        IsTrue(m.Match("b"));
        IsFalse(m.Match("ab"));
        IsFalse(m.Match("ac"));
        IsFalse(m.Match(""));

        m = Ch('a').Or("b");
        IsTrue(m.Match("a"));
        IsTrue(m.Match("b"));
        IsFalse(m.Match("ab"));
        IsFalse(m.Match("ac"));
        IsFalse(m.Match(""));
    }

    [TestMethod]
    public void TestRepeat()
    {
        var m = Ch('a').Repeat(1, 3);
        IsTrue(m.Match("a"));
        IsTrue(m.Match("aa"));
        IsTrue(m.Match("aaa"));
        IsFalse(m.Match("aaaa"));
        IsFalse(m.Match(""));

        m = Ch('a').Repeat(3);
        IsFalse(m.Match("a"));
        IsFalse(m.Match("aa"));
        IsTrue(m.Match("aaa"));
        IsFalse(m.Match("aaaa"));
        IsFalse(m.Match(""));

        m = Ch('a').Repeat(3, int.MaxValue);
        IsFalse(m.Match("a"));
        IsFalse(m.Match("aa"));
        IsTrue(m.Match("aaa"));
        IsTrue(m.Match("aaaa"));
        IsTrue(m.Match(new string('a', 10000)));
        IsFalse(m.Match(""));
        IsFalse(m.Match("bb"));
    }

    [TestMethod]
    public void TestMany()
    {
        var m = Ch('a').Many0();
        IsTrue(m.Match(""));
        IsTrue(m.Match("a"));
        IsTrue(m.Match("aa"));
        IsTrue(m.Match("aaa"));
        IsFalse(m.Match("b"));
        IsFalse(m.Match("ab"));
        IsFalse(m.Match("aba"));

        m = Ch('a').Many1();
        IsFalse(m.Match(""));
        IsTrue(m.Match("a"));
        IsTrue(m.Match("aa"));
        IsTrue(m.Match("aaa"));
        IsFalse(m.Match("b"));
        IsFalse(m.Match("ab"));
        IsFalse(m.Match("aba"));

        m = Ch('a').Many(2);
        IsFalse(m.Match(""));
        IsFalse(m.Match("a"));
        IsTrue(m.Match("aa"));
        IsTrue(m.Match("aaa"));
        IsFalse(m.Match("b"));
        IsFalse(m.Match("ab"));
        IsFalse(m.Match("aba"));
    }

    [TestMethod]
    public void TestFlatMap()
    {
        var m = Not(' ').Many1().FlatMap(s => Ch(' ').And(Str("xxx")).And(' ').And(Str(s)));
        IsTrue(m.Match("a xxx a"));
        IsFalse(m.Match("a xxx b"));
        IsFalse(m.Match("aa xxx a"));
        IsFalse(m.Match("a  xxx a"));

        m = Any.Many1().FlatMap(s => Any.Repeat(s.Length));
        IsTrue(m.Match("aaabbb"));
        IsFalse(m.Match("aaabbbb"));
        IsFalse(m.Match("a"));
    }

    [TestMethod]
    public void TestLazy()
    {
        var i = 123;
        var m = Lazy(() =>
        {
            i = 456;
            return Ch('a');
        });
        AreEqual(123, i);
        IsTrue(m.Match("a"));
        AreEqual(456, i);
    }

    [TestMethod]
    public void TestStrs()
    {
        var m = Strs("a", "b", "c", "d");
        IsTrue(m.Match("a"));
        IsTrue(m.Match("b"));
        IsTrue(m.Match("c"));
        IsTrue(m.Match("d"));
        IsFalse(m.Match("abcd"));
    }
    
    [TestMethod]
    public void TestSeq()
    {
        var m = Seq(Str("a"), Str("b"), Str("c"), Str("d"));
        IsFalse(m.Match("a"));
        IsFalse(m.Match("b"));
        IsFalse(m.Match("c"));
        IsFalse(m.Match("d"));
        IsTrue(m.Match("abcd"));
        IsFalse(m.Match("dcba"));
        IsFalse(m.Match("abcde"));
    }
    
    [TestMethod]
    public void TestOneOf()
    {
        var m = OneOf(Str("a"), Str("b"), Str("c"), Str("d"));
        IsTrue(m.Match("a"));
        IsTrue(m.Match("b"));
        IsTrue(m.Match("c"));
        IsTrue(m.Match("d"));
        IsFalse(m.Match("abcd"));
        IsFalse(m.Match("abcde"));
    }
}