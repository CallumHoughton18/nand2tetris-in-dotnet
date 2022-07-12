using System.IO;
using System.Text.RegularExpressions;
using Xunit;

namespace JackCompiler.Core.Tests;

internal static class TestHelpers
{
    public static void AssertFilesAreEqualLineByLine(string actualFile, string expectedFile)
    {
        var expectedOutputLines = File.ReadAllLines(expectedFile);
        var actualOutputLines = File.ReadAllLines(actualFile);
        
        Assert.True(expectedOutputLines.Length == actualOutputLines.Length);
        for (int i = 0; i < expectedOutputLines.Length; i++)
        {
            var expectedOutputLine = expectedOutputLines[i];
            var actualOutputLine = actualOutputLines[i];
            Assert.True(expectedOutputLine == actualOutputLine, $"Mismatch on line {i+1}");
        }
    }
}