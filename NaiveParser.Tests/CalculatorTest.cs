using System;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NaiveParser;
using static System.Math;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;

namespace NaiveParser.Tests;

[TestClass]
[TestSubject(typeof(Calculator))]
public class CalculatorTest
{
    [TestMethod]
    public void CalculatorTest0()
    {
        var calculator = new Calculator()
            .RegisterConstant("PI", 3.14)
            .RegisterFunc("log", args =>
            {
                if (args.Count != 2) throw new ArgumentException("log expects 2 args");
                return Log(args[1], args[0]);
            });
        AreEqual(19.14, calculator.Calculate("2 ^3 + log(2, 256) + PI"));
    }
}