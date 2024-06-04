using System;
using System.Collections.Generic;
using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NaiveParser.Tests;

[TestClass]
[TestSubject(typeof(Json))]
public class JsonTest
{
    [TestMethod]
    public void JsonTest0()
    {
        var json = """
                   {
                       "name": "John",
                       "age": -.30e16,
                       "isStudent": false,
                       "isTeacher": true,
                       "grades": [90, 85, 95],
                       "address": {
                           "city": "New York",
                           "zip": "10001"
                       },
                       "languages": ["English", "Spanish", "French"],
                       "contact": null
                   }
                   """;
        var obj = new Json().Parse(json);
        Console.WriteLine(Json.ToJson(obj));
        
    }
}