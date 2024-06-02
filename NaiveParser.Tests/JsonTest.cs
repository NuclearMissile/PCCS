using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NaiveParser.Tests;

[TestClass]
[TestSubject(typeof(Json))]
public class JsonTest
{
    [TestMethod]
    public void ToJsonTest0()
    {
        var obj = new Dictionary<string, object>()
        {
            { "name", "Alice" },
            { "age", -.30e16 }
        };

        var json = Json.ToJson(obj);
    }
}