using JetBrains.Annotations;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using static Microsoft.VisualStudio.TestTools.UnitTesting.Assert;
using PCMatcher;
using static PCMatcher.JsonValidator;

namespace PCMatcher.Tests;

[TestClass]
[TestSubject(typeof(JsonValidator))]
public class JsonValidatorTest
{
    private const string JSON = """
                                {
                                    "a": 123,
                                    "aa": -123,
                                    "b": 3.14,
                                    "bb": -3.14,
                                    "bbb": -.14,
                                    "c": "hello",
                                    "d": {
                                        "x": 100,
                                        "y": "world!"
                                    },
                                    "e": [
                                        12,
                                        34.56,
                                        {
                                            "name": "Xiao Ming",
                                            "age": 18,
                                            "score": [99.8, 87.5, 60.0]
                                        },
                                        "abc"
                                    ],
                                    "f": [],
                                    "g": {},
                                    "h": [true, {"m": false}]
                                }
                                """;

    [TestMethod]
    public void TestJsonValidator()
    {
        IsTrue(Match(JSON));
        IsTrue(Match("123"));
        IsTrue(Match("34.56"));
        IsTrue(Match("\"hello\""));
        IsTrue(Match("true"));
        IsTrue(Match("false"));
        IsTrue(Match("{}"));
        IsTrue(Match("[]"));
        IsTrue(Match("[{}]"));
        IsFalse(Match(""));
        IsFalse(Match("{"));
        IsFalse(Match("}"));
        IsFalse(Match("{}}"));
        IsFalse(Match("[1, 2 3]"));
        IsFalse(Match("{1, 2, 3}"));
    }
}