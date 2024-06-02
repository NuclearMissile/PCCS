using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PCMatcher;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace PCMatcher.Tests;

[TestClass]
[TestSubject(typeof(BracketMatcher))]
public class BracketMatcherTest
{
    [TestMethod]
    public void TestBracketValidator()
    {
        IsFalse(BracketMatcher.Match(""));
        IsFalse(BracketMatcher.Match("("));
        IsFalse(BracketMatcher.Match(")"));
        IsTrue(BracketMatcher.Match("()"));
        IsFalse(BracketMatcher.Match(")("));
        IsFalse(BracketMatcher.Match("(("));
        IsFalse(BracketMatcher.Match("))"));
        IsTrue(BracketMatcher.Match("()()"));
        IsTrue(BracketMatcher.Match("(())"));
        IsFalse(BracketMatcher.Match("(()"));
        IsFalse(BracketMatcher.Match("())"));
        IsTrue(BracketMatcher.Match("()()()"));
        IsTrue(BracketMatcher.Match("()(())"));
        IsTrue(BracketMatcher.Match("(())()"));
        IsTrue(BracketMatcher.Match("(()())()"));
        IsTrue(BracketMatcher.Match("(())()((()))()"));
        IsFalse(BracketMatcher.Match("(())()((())()"));
        IsFalse(BracketMatcher.Match("(())()(()))()"));
    }
}