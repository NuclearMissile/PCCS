using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using static PCMatcher.ArithmeticExprValidator;

namespace PCMatcher.Tests;

[TestClass]
[TestSubject(typeof(ArithmeticExprValidator))]
public class ArithmeticExprValidatorTest
{
    [TestMethod]
    public void TestArithmeticExprValidator()
    {
        IsFalse(Match(""));
        IsTrue(Match("123"));
        IsTrue(Match("-6"));
        IsTrue(Match("2*(3+4)"));
        IsFalse(Match("abc"));
        IsFalse(Match("12+"));
        IsFalse(Match("12*"));
        IsFalse(Match("+3"));
        IsFalse(Match("/6"));
        IsFalse(Match("6+3-"));
        IsTrue(Match("(12+345)*(67-890)+10/6"));
        IsTrue(Match("-6*18+(-3/978)"));
        IsTrue(Match("24/5774*(6/357+637)-2*7/52+5"));
        IsFalse(Match("24/5774*(6/357+637-2*7/52+5"));
        IsTrue(Match("7758*(6/314+552234)-2*61/(10+2/(40-38*5))"));
        IsFalse(Match("7758*(6/314+552234)-2*61/(10+2/40-38*5))"));
    }
}