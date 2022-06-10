using System.IO;
using System.Text.RegularExpressions;
using JackCompiler.Core.Syntax_Analyzer;
using Xunit;

namespace JackCompiler.Core.Tests;

public class JackCompilerRunnerIntegrationTests
{
    [Theory]
    [InlineData("./Test Input Files/ArrayTest/Main.jack", "./Expected Output Files/ArrayTest/MainT.xml")]
    [InlineData("./Test Input Files/ExpressionlessSquare/Main.jack", "./Expected Output Files/ExpressionlessSquare/MainT.xml")]
    [InlineData("./Test Input Files/ExpressionlessSquare/SquareGame.jack", "./Expected Output Files/ExpressionlessSquare/SquareGameT.xml")]
    [InlineData("./Test Input Files/ExpressionlessSquare/Square.jack", "./Expected Output Files/ExpressionlessSquare/SquareT.xml")]
    [InlineData("./Test Input Files/Square/Main.jack", "./Expected Output Files/Square/MainT.xml")]
    [InlineData("./Test Input Files/Square/SquareGame.jack", "./Expected Output Files/Square/SquareGameT.xml")]
    [InlineData("./Test Input Files/Square/Square.jack", "./Expected Output Files/Square/SquareT.xml")]
    public void Should_Give_Correct_Tokenizer_Output(string inputFilePath, string expectedOutputFilePath)
    {
        var tokenizerOutputPath = AssertTokenizerFileGenerated(inputFilePath);
        AssertFilesAreEqualWithoutWhitespace(tokenizerOutputPath, expectedOutputFilePath);
    }

    private string AssertTokenizerFileGenerated(string inputFilePath)
    {
        
        var outputFileName = $"{Path.GetFileNameWithoutExtension(inputFilePath)}.xml";
        var outputFile = Path.Combine(Path.GetDirectoryName(inputFilePath)!, outputFileName);
        var sut = new JackCompilerRunner(inputFilePath, outputFile, true);
        var outputPath = sut.Run();
        var tokenizerPath = Utils.AppendStringToFileName(outputFile, "T");
        Assert.True(File.Exists(tokenizerPath));
        return tokenizerPath;
    }
    
    private void AssertFilesAreEqualWithoutWhitespace(string actualFile, string expectedFile)
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