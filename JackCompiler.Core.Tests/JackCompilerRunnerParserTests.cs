using System.IO;
using System.Text.RegularExpressions;
using Xunit;

namespace JackCompiler.Core.Tests;

public class JackCompilerRunnerParserTests
{
    [Theory]
    [InlineData("./Test Input Files/ArrayTest/Main.jack")]
    public void Should_Give_Correct_Tokenizer_Output(string inputFilePath)
    {
        var tokenizerOutputPath = GenParserXmlFile(inputFilePath);
        Assert.True(File.Exists(tokenizerOutputPath));
    }

    private string GenParserXmlFile(string inputFilePath)
    {
        
        var outputFileName = $"{Path.GetFileNameWithoutExtension(inputFilePath)}.xml";
        var outputFile = Path.Combine(Path.GetDirectoryName(inputFilePath)!, outputFileName);
        var sut = new JackCompilerRunner(inputFilePath, outputFile, true);
        var outputPath = sut.Run();
        return outputPath;
    }
    
    // private void AssertFilesAreEqualWithoutWhitespace(string actualFile, string expectedFile)
    // {
    //     var expectedOutputLines = File.ReadAllLines(expectedFile);
    //     var actualOutputLines = File.ReadAllLines(actualFile);
    //     
    //     Assert.True(expectedOutputLines.Length == actualOutputLines.Length);
    //     for (int i = 0; i < expectedOutputLines.Length; i++)
    //     {
    //         var expectedOutputLine = Regex.Replace(expectedOutputLines[i], @"\s+", ""); ;
    //         var actualOutputLine = Regex.Replace(actualOutputLines[i], @"\s+", "");
    //         Assert.True(expectedOutputLine == actualOutputLine, $"Mismatch on on line {i+1}");
    //     }
    // }
}