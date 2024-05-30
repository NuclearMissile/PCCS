using System;
using System.IO;
using System.Reflection;
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

        m = Regex.Parse("a+");
        IsFalse(m.Match(""));
        IsTrue(m.Match("a"));
        IsTrue(m.Match("aa"));
        IsTrue(m.Match("aaa"));
        IsFalse(m.Match("ab"));
        IsFalse(m.Match("aab"));
        IsFalse(m.Match(" "));

        m = Regex.Parse("a?");
        IsTrue(m.Match(""));
        IsTrue(m.Match("a"));
        IsFalse(m.Match("aa"));
        IsFalse(m.Match("aaa"));
        IsFalse(m.Match("ab"));
        IsFalse(m.Match("aab"));
        IsFalse(m.Match(" "));
    }

    [TestMethod]
    public void Test3()
    {
        var m = Regex.Parse("abc");
        IsFalse(m.Match(""));
        IsFalse(m.Match("ab"));
        IsTrue(m.Match("abc"));
        IsFalse(m.Match("abcd"));
        IsFalse(m.Match(" "));
    }

    [TestMethod]
    public void Test4()
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
    public void Test5()
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
    public void Test6()
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
    public void Test7()
    {
        var m = Regex.Parse("a*a*a*a*aaaaaaaaaa");
        IsTrue(m.Match("aaaaaaaaaa"));
        IsFalse(m.Match("aaaaaaaaa"));

        m = Regex.Parse("((a*)*)*");
        IsTrue(m.Match(
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"));
        IsFalse(m.Match(
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaab"));

        m = Regex.Parse("((a+)+)+");
        IsTrue(m.Match(
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"));
        IsFalse(m.Match(
            "aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaab"));

        m = Regex.Parse("a(.+)+a");
        IsTrue(m.Match(
            "abbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbba"));
        IsFalse(m.Match(
            "abbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"));
    }

    [TestMethod]
    public void Test8()
    {
        var m = Regex.Parse(".*.*=.*");
        IsTrue(m.Match("a=bbb"));
        IsTrue(m.Match("aaaaaaa=bbbbbbbbbbbbbbbbbbb"));

        m = Regex.Parse("(.*)ABCD(\\d*)");
        IsTrue(m.Match("#$%^ABCD1234"));
        IsTrue(m.Match("#$%^ABCD"));
        IsTrue(m.Match("ABCD1234"));
        IsFalse(m.Match("1234ABCD#$%^"));
    }

    [TestMethod]
    public void Test9()
    {
        var m = Regex.Parse("a{3}");
        IsFalse(m.Match("a"));
        IsFalse(m.Match("aa"));
        IsTrue(m.Match("aaa"));
        IsFalse(m.Match("aaaa"));

        m = Regex.Parse("a{1,3}");
        IsFalse(m.Match(""));
        IsTrue(m.Match("a"));
        IsTrue(m.Match("aa"));
        IsTrue(m.Match("aaa"));
        IsFalse(m.Match("aaaa"));
    }

    [TestMethod]
    public void Test10()
    {
        var m = Regex.Parse("\\d");
        IsTrue(m.Match("1"));
        IsFalse(m.Match("123"));
        IsFalse(m.Match("a"));

        m = Regex.Parse("\\w");
        IsTrue(m.Match("a"));
        IsTrue(m.Match("A"));
        IsTrue(m.Match("1"));
        IsFalse(m.Match("aaaa"));

        m = Regex.Parse("\\*");
        IsTrue(m.Match("*"));
        IsFalse(m.Match("a"));
        IsFalse(m.Match("b"));
        IsFalse(m.Match("aaaa"));
        
        m = Regex.Parse("\\S");
        IsTrue(m.Match("*"));
        IsTrue(m.Match("a"));
        IsTrue(m.Match("b"));
        IsTrue(m.Match("a"));
        IsFalse(m.Match(" "));
        
        m = Regex.Parse("\\s");
        IsFalse(m.Match("*"));
        IsFalse(m.Match("a"));
        IsFalse(m.Match("b"));
        IsFalse(m.Match("a"));
        IsTrue(m.Match(" "));
    }

    [TestMethod]
    public void Test11()
    {
        var m = Regex.Parse("[1-3]");
        IsFalse(m.Match("0"));
        IsTrue(m.Match("1"));
        IsTrue(m.Match("2"));
        IsTrue(m.Match("3"));
        IsFalse(m.Match("4"));
        
        m = Regex.Parse("[^1-3]");
        IsTrue(m.Match("A"));
        IsTrue(m.Match("0"));
        IsFalse(m.Match("1"));
        IsFalse(m.Match("2"));
        IsFalse(m.Match("3"));
        IsTrue(m.Match("4"));
        IsTrue(m.Match("z"));

        m = Regex.Parse("[1-3b-d]");
        IsFalse(m.Match("0"));
        IsTrue(m.Match("1"));
        IsTrue(m.Match("2"));
        IsTrue(m.Match("3"));
        IsFalse(m.Match("4"));
        IsFalse(m.Match("a"));
        IsTrue(m.Match("b"));
        IsTrue(m.Match("c"));
        IsTrue(m.Match("d"));
        IsFalse(m.Match("e"));
        
        m = Regex.Parse("[1-32-4]");
        IsFalse(m.Match("0"));
        IsTrue(m.Match("1"));
        IsTrue(m.Match("2"));
        IsTrue(m.Match("3"));
        IsTrue(m.Match("4"));
        IsFalse(m.Match("5"));
        
        m = Regex.Parse("[^1-3b-d]");
        IsTrue(m.Match("0"));
        IsFalse(m.Match("1"));
        IsFalse(m.Match("2"));
        IsFalse(m.Match("3"));
        IsTrue(m.Match("4"));
        IsTrue(m.Match("a"));
        IsFalse(m.Match("b"));
        IsFalse(m.Match("c"));
        IsFalse(m.Match("d"));
        IsTrue(m.Match("e"));

        m = Regex.Parse("[aeiou]");
        IsTrue(m.Match("a"));
        IsTrue(m.Match("e"));
        IsTrue(m.Match("i"));
        IsTrue(m.Match("o"));
        IsTrue(m.Match("u"));
        IsFalse(m.Match("b"));
        
        
    }

    [TestMethod]
    public void Test12()
    {
        var m = Regex.Parse(
            "[0369]*(([147][0369]*|[258][0369]*[258][0369]*)([147][0369]*[258][0369]*)*([258][0369]*|[147][0369]*[147][0369]*)|[258][0369]*[147][0369]*)*");

        for (var i = 0; i <= 100000; ++i)
        {
            if (i % 3 == 0)
            {
                IsTrue(m.Match(i.ToString()));
            }
            else
            {
                IsFalse(m.Match(i.ToString()));
            }
        }

        IsTrue(m.Match("1306037620370620974"));
        IsFalse(m.Match("1306037620370620975"));
        IsFalse(m.Match("1306037620370620976"));
        IsTrue(m.Match("1306037620370620977"));
    }

    [TestMethod]
    public void Test13()
    {
        var m = Regex.Parse("(a*)*");
        IsTrue(m.Match(new string('a', 5000)));
        IsFalse(m.Match(new string('a', 5000) + "b"));
    }

    [TestMethod]
    public void Test14()
    {
        var assembly = Assembly.GetExecutingAssembly();
        for (var i = 1; i <= 11; i++)
        {
            var fileIn = $"PCMatcher.Tests.resources.regular{i}.in";
            var fileOut = $"PCMatcher.Tests.resources.regular{i}.out";

            using (Stream streamIn = assembly.GetManifestResourceStream(fileIn)!)
            using (Stream streamOut = assembly.GetManifestResourceStream(fileOut)!)
            using (StreamReader readerIn = new StreamReader(streamIn))
            using (StreamReader readerOut = new StreamReader(streamOut))
            {
                while (!readerIn.EndOfStream)
                {
                    var expr = readerIn.ReadLine()!;
                    var str = readerIn.ReadLine()!;
                    var expected = readerOut.ReadLine();
                    Console.WriteLine($"expr: {expr}, str: {str}");
                    Console.WriteLine("====================================");

                    var m = Regex.Parse(expr);
                    if (expected == "Yes")
                    {
                        IsTrue(m.Match(str));
                    }
                    else
                    {
                        IsFalse(m.Match(str));
                    }
                }
            }
        }
    }
}