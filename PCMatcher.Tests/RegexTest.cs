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
    }
}