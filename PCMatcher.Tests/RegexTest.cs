using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace PCMatcher.Tests;

[TestClass]
[TestSubject(typeof(Regex))]
public class RegexTest
{
    [TestMethod]
    public void Test0()
    {
        var m = Regex.Parse("a");
        IsTrue(m.Match("a"));
        IsFalse(m.Match("b"));
        IsFalse(m.Match("aa"));
        IsFalse(m.Match(""));

        m = Regex.Parse("(a)");
        IsTrue(m.Match("a"));
        IsFalse(m.Match("b"));
        IsFalse(m.Match("aa"));
        IsFalse(m.Match(""));
    }

    [TestMethod]
    public void Test1()
    {
        var m = Regex.Parse(".");
        IsTrue(m.Match("a"));
        IsTrue(m.Match("b"));
        IsTrue(m.Match(" "));
        IsTrue(m.Match("."));
        IsFalse(m.Match("aa"));
        IsFalse(m.Match(""));
    }

    [TestMethod]
    public void Test2()
    {
        var m = Regex.Parse("a*");
        IsTrue(m.Match(""));
        IsTrue(m.Match("a"));
        IsTrue(m.Match("aa"));
        IsTrue(m.Match("aaa"));
        IsFalse(m.Match("ab"));
        IsFalse(m.Match("aab"));
        IsFalse(m.Match(" "));
    }

    [TestMethod]
    public void Test3()
    {
        var m = Regex.Parse("a+");
        IsFalse(m.Match(""));
        IsTrue(m.Match("a"));
        IsTrue(m.Match("aa"));
        IsTrue(m.Match("aaa"));
        IsFalse(m.Match("ab"));
        IsFalse(m.Match("aab"));
        IsFalse(m.Match(" "));
    }

    [TestMethod]
    public void Test4()
    {
        var m = Regex.Parse("abc");
        IsFalse(m.Match(""));
        IsFalse(m.Match("ab"));
        IsTrue(m.Match("abc"));
        IsFalse(m.Match("abcd"));
        IsFalse(m.Match(" "));
    }

    [TestMethod]
    public void Test5()
    {
        var m = Regex.Parse("a|b");
        IsFalse(m.Match(""));
        IsTrue(m.Match("a"));
        IsTrue(m.Match("b"));
        IsFalse(m.Match("ab"));
        IsFalse(m.Match("abc"));
        IsFalse(m.Match(" "));

        m = Regex.Parse("a|b|cd|efg");
        IsFalse(m.Match(""));
        IsTrue(m.Match("a"));
        IsTrue(m.Match("b"));
        IsFalse(m.Match("ab"));
        IsFalse(m.Match("abc"));
        IsFalse(m.Match("abcdefg"));
        IsTrue(m.Match("cd"));
        IsTrue(m.Match("efg"));
        IsFalse(m.Match(" "));
    }

    [TestMethod]
    public void Test6()
    {
        var m = Regex.Parse("(a|b)*");
        IsTrue(m.Match(""));
        IsTrue(m.Match("a"));
        IsTrue(m.Match("b"));
        IsTrue(m.Match("ab"));
        IsTrue(m.Match("ababab"));
        IsFalse(m.Match("abc"));
        IsFalse(m.Match(" "));

        m = Regex.Parse("(a|b)+");
        IsFalse(m.Match(""));
        IsTrue(m.Match("a"));
        IsTrue(m.Match("b"));
        IsTrue(m.Match("ab"));
        IsTrue(m.Match("ababab"));
        IsFalse(m.Match("abc"));
        IsFalse(m.Match(" "));

        m = Regex.Parse("(ab)*");
        IsTrue(m.Match(""));
        IsFalse(m.Match("a"));
        IsFalse(m.Match("b"));
        IsTrue(m.Match("ab"));
        IsTrue(m.Match("ababab"));
        IsFalse(m.Match("abababa"));
        IsFalse(m.Match("abc"));
        IsFalse(m.Match(" "));

        m = Regex.Parse("(a|ab)c");
        IsFalse(m.Match(""));
        IsFalse(m.Match("a"));
        IsFalse(m.Match("b"));
        IsTrue(m.Match("ac"));
        IsTrue(m.Match("abc"));
        IsFalse(m.Match("ababab"));
        IsFalse(m.Match("c"));
        IsFalse(m.Match(" "));

        m = Regex.Parse("(a*)*");
        IsTrue(m.Match(""));
        IsTrue(m.Match("a"));
        IsTrue(m.Match("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"));
        IsFalse(m.Match("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaab"));

        m = Regex.Parse("(a+)+");
        IsFalse(m.Match(""));
        IsTrue(m.Match("a"));
        IsTrue(m.Match("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"));
        IsFalse(m.Match("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaab"));
    }

    [TestMethod]
    public void Test7()
    {
        var m = Regex.Parse("(0|1(01*0)*1)*");
        IsTrue(m.Match("0"));
        IsFalse(m.Match("1"));
        IsFalse(m.Match("10"));
        IsTrue(m.Match("11"));
        IsFalse(m.Match("100"));
        IsFalse(m.Match("101"));
        IsTrue(m.Match("110"));
        IsFalse(m.Match("111"));
        IsFalse(m.Match("1000"));
        IsTrue(m.Match("1000001001001111010"));
        IsFalse(m.Match("1000001001001111011"));
        IsFalse(m.Match("1000001001001111100"));
        IsTrue(m.Match("1000001001001111101"));
        IsTrue(m.Match("111010010101011001000001110011010111101110101111101110110"));
        IsFalse(m.Match("111010010101011001000001110011010111101110101111101110111"));
        IsFalse(m.Match("111010010101011001000001110011010111101110101111101111000"));
        IsTrue(m.Match("111010010101011001000001110011010111101110101111101111001"));
    }
    
    [TestMethod]
    public void Test8()
    {
        var m = Regex.Parse("a*a*a*a*aaaaaaaaaa");
        IsTrue(m.Match("aaaaaaaaaa"));
        IsFalse(m.Match("aaaaaaaaa"));
        
        m = Regex.Parse("((a*)*)*");
        IsTrue(m.Match("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"));
        IsFalse(m.Match("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaab"));
        
        m = Regex.Parse("((a+)+)+");
        IsTrue(m.Match("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"));
        IsFalse(m.Match("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaab"));
        
        m = Regex.Parse("a(.+)+a");
        IsTrue(m.Match("abbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbba"));
        IsFalse(m.Match("abbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"));
    }
    
    [TestMethod]
    public void Test9()
    {
        var m = Regex.Parse(".*.*=.*");
        IsTrue(m.Match("a=bbb"));
        IsTrue(m.Match("aaaaaaa=bbbbbbbbbbbbbbbbbbb"));
    }
}