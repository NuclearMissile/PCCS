using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PCMatcher;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace PCMatcher.Tests;

[TestClass]
[TestSubject(typeof(BracketValidator))]
public class BracketValidatorTest
{
    [TestMethod]
    public void TestBracketValidator()
    {
        IsFalse(BracketValidator.Match(""));
        IsFalse(BracketValidator.Match("("));
        IsFalse(BracketValidator.Match(")"));
        IsTrue(BracketValidator.Match("()"));
        IsFalse(BracketValidator.Match(")("));
        IsFalse(BracketValidator.Match("(("));
        IsFalse(BracketValidator.Match("))"));
        IsTrue(BracketValidator.Match("()()"));
        IsTrue(BracketValidator.Match("(())"));
        IsFalse(BracketValidator.Match("(()"));
        IsFalse(BracketValidator.Match("())"));
        IsTrue(BracketValidator.Match("()()()"));
        IsTrue(BracketValidator.Match("()(())"));
        IsTrue(BracketValidator.Match("(())()"));
        IsTrue(BracketValidator.Match("(()())()"));
        IsTrue(BracketValidator.Match("(())()((()))()"));
        IsFalse(BracketValidator.Match("(())()((())()"));
        IsFalse(BracketValidator.Match("(())()(()))()"));
    }
}