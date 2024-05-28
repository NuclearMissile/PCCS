using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PCCS;
using static PCCS.IMatcher;

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
        Assert.IsTrue(m.Match("a"));
        Assert.IsFalse(m.Match("b"));
        Assert.IsFalse(m.Match("aa"));
        Assert.IsFalse(m.Match("bb"));
    }

    [TestMethod]
    public void TestChs()
    {
        var m = Chs('a', 'b', 'c');
        Assert.IsTrue(m.Match("a"));
        Assert.IsTrue(m.Match("b"));
        Assert.IsTrue(m.Match("c"));
        Assert.IsFalse(m.Match("d"));
        Assert.IsFalse(m.Match("aa"));
        Assert.IsFalse(m.Match("bb"));
    }

    [TestMethod]
    public void TestAny()
    {
        var m = Any;
        Assert.IsTrue(m.Match("a"));
        Assert.IsTrue(m.Match("b"));
        Assert.IsTrue(m.Match("c"));
        Assert.IsTrue(m.Match("d"));
        Assert.IsFalse(m.Match("aa"));
        Assert.IsFalse(m.Match("bb"));
    }

    [TestMethod]
    public void TestRange()
    {
        var m = Range('0', '3');
        Assert.IsTrue(m.Match("0"));
        Assert.IsTrue(m.Match("1"));
        Assert.IsTrue(m.Match("2"));
        Assert.IsTrue(m.Match("3"));
        Assert.IsFalse(m.Match("9"));
        Assert.IsFalse(m.Match("a"));
    }

    [TestMethod]
    public void TestNot()
    {
        var m = Not('a');
        Assert.IsTrue(m.Match("b"));
        Assert.IsFalse(m.Match("a"));
        Assert.IsFalse(m.Match("bb"));
    }

    [TestMethod]
    public void TestStr()
    {
        var m = Str("abc");
        Assert.IsFalse(m.Match(""));
        Assert.IsFalse(m.Match("a"));
        Assert.IsFalse(m.Match("ab"));
        Assert.IsTrue(m.Match("abc"));
        Assert.IsFalse(m.Match("abcd"));
    }

    [TestMethod]
    public void TestAnd()
    {
        var m = Ch('a').And('b');
        Assert.IsTrue(m.Match("ab"));
        Assert.IsFalse(m.Match("a"));
        Assert.IsFalse(m.Match("b"));
        Assert.IsFalse(m.Match("abc"));
        Assert.IsFalse(m.Match(""));

        m = Ch('a').And(Ch('b'));
        Assert.IsTrue(m.Match("ab"));
        Assert.IsFalse(m.Match("a"));
        Assert.IsFalse(m.Match("b"));
        Assert.IsFalse(m.Match("abc"));
        Assert.IsFalse(m.Match(""));

        m = Ch('a').And("b");
        Assert.IsTrue(m.Match("ab"));
        Assert.IsFalse(m.Match("a"));
        Assert.IsFalse(m.Match("b"));
        Assert.IsFalse(m.Match("abc"));
        Assert.IsFalse(m.Match(""));
    }

    [TestMethod]
    public void TestOr()
    {
        var m = Ch('a').Or('b');
        Assert.IsTrue(m.Match("a"));
        Assert.IsTrue(m.Match("b"));
        Assert.IsFalse(m.Match("ab"));
        Assert.IsFalse(m.Match("ac"));
        Assert.IsFalse(m.Match(""));

        m = Ch('a').Or(Ch('b'));
        Assert.IsTrue(m.Match("a"));
        Assert.IsTrue(m.Match("b"));
        Assert.IsFalse(m.Match("ab"));
        Assert.IsFalse(m.Match("ac"));
        Assert.IsFalse(m.Match(""));

        m = Ch('a').Or("b");
        Assert.IsTrue(m.Match("a"));
        Assert.IsTrue(m.Match("b"));
        Assert.IsFalse(m.Match("ab"));
        Assert.IsFalse(m.Match("ac"));
        Assert.IsFalse(m.Match(""));
    }

    [TestMethod]
    public void TestRepeat()
    {
        var m = Ch('a').Repeat(1, 3);
        Assert.IsTrue(m.Match("a"));
        Assert.IsTrue(m.Match("aa"));
        Assert.IsTrue(m.Match("aaa"));
        Assert.IsFalse(m.Match("aaaa"));
        Assert.IsFalse(m.Match(""));

        m = Ch('a').Repeat(3);
        Assert.IsFalse(m.Match("a"));
        Assert.IsFalse(m.Match("aa"));
        Assert.IsTrue(m.Match("aaa"));
        Assert.IsFalse(m.Match("aaaa"));
        Assert.IsFalse(m.Match(""));

        m = Ch('a').Repeat(3, int.MaxValue);
        Assert.IsFalse(m.Match("a"));
        Assert.IsFalse(m.Match("aa"));
        Assert.IsTrue(m.Match("aaa"));
        Assert.IsTrue(m.Match("aaaa"));
        Assert.IsTrue(m.Match(new string('a', 10000)));
        Assert.IsFalse(m.Match(""));
        Assert.IsFalse(m.Match("bb"));
    }

    [TestMethod]
    public void TestMany()
    {
        var m = Ch('a').Many0();
        Assert.IsTrue(m.Match(""));
        Assert.IsTrue(m.Match("a"));
        Assert.IsTrue(m.Match("aa"));
        Assert.IsTrue(m.Match("aaa"));
        Assert.IsFalse(m.Match("b"));
        Assert.IsFalse(m.Match("ab"));
        Assert.IsFalse(m.Match("aba"));

        m = Ch('a').Many1();
        Assert.IsFalse(m.Match(""));
        Assert.IsTrue(m.Match("a"));
        Assert.IsTrue(m.Match("aa"));
        Assert.IsTrue(m.Match("aaa"));
        Assert.IsFalse(m.Match("b"));
        Assert.IsFalse(m.Match("ab"));
        Assert.IsFalse(m.Match("aba"));

        m = Ch('a').Many(2);
        Assert.IsFalse(m.Match(""));
        Assert.IsFalse(m.Match("a"));
        Assert.IsTrue(m.Match("aa"));
        Assert.IsTrue(m.Match("aaa"));
        Assert.IsFalse(m.Match("b"));
        Assert.IsFalse(m.Match("ab"));
        Assert.IsFalse(m.Match("aba"));
    }

    [TestMethod]
    public void TestFlatMap()
    {
        var m = Not(' ').Many1().FlatMap(s => Ch(' ').And(Str("xxx")).And(' ').And(Str(s)));
        Assert.IsTrue(m.Match("a xxx a"));
        Assert.IsFalse(m.Match("a xxx b"));
        Assert.IsFalse(m.Match("aa xxx a"));
        Assert.IsFalse(m.Match("a  xxx a"));

        m = Any.Many1().FlatMap(s => Any.Repeat(s.Length));
        Assert.IsTrue(m.Match("aaabbb"));
        Assert.IsFalse(m.Match("aaabbbb"));
        Assert.IsFalse(m.Match("a"));
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
        Assert.AreEqual(123, i);
        Assert.IsTrue(m.Match("a"));
        Assert.AreEqual(456, i);
    }
}