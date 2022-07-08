using System.IO;
using System.Text.RegularExpressions;
using Xunit;

namespace JackCompiler.Core.Tests;

internal static class TestHelpers
{
    public static void AssertFilesAreEqualWithoutWhitespace(string actualFile, string expectedFile)
    {
        var expectedOutputLines = File.ReadAllLines(expectedFile);
        var actualOutputLines = File.ReadAllLines(actualFile);
        
        Assert.True(expectedOutputLines.Length == actualOutputLines.Length);
        for (int i = 0; i < expectedOutputLines.Length; i++)
        {
            var expectedOutputLine = Regex.Replace(expectedOutputLines[i], @"\s+", ""); ;
            var actualOutputLine = Regex.Replace(actualOutputLines[i], @"\s+", "");
            Assert.True(expectedOutputLine == actualOutputLine, $"Mismatch on on line {i+1}");
        }
    }
}