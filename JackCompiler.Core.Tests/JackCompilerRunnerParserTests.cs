using System.IO;
using System.Text.RegularExpressions;
using Xunit;

namespace JackCompiler.Core.Tests;

[Collection("Compiler XML output tests")]
public class JackCompilerRunnerParserTests
{
    [Theory]
    [InlineData("./Test Input Files/ArrayTest/Main.jack", "./Expected Output Files/ArrayTest/Main.xml")]
    [InlineData("./Test Input Files/ExpressionlessSquare/Main.jack", "./Expected Output Files/ExpressionlessSquare/Main.xml")]
    [InlineData("./Test Input Files/ExpressionlessSquare/SquareGame.jack", "./Expected Output Files/ExpressionlessSquare/SquareGame.xml")]
    [InlineData("./Test Input Files/ExpressionlessSquare/Square.jack", "./Expected Output Files/ExpressionlessSquare/Square.xml")]
    [InlineData("./Test Input Files/Square/Main.jack", "./Expected Output Files/Square/Main.xml")]
    [InlineData("./Test Input Files/Square/SquareGame.jack", "./Expected Output Files/Square/SquareGame.xml")]
    [InlineData("./Test Input Files/Square/Square.jack", "./Expected Output Files/Square/Square.xml")]
    
    public void Should_Give_Correct_Compiler_Output(string inputFilePath, string expectedOutputFile)
    {
        var compilerXmlOutputPath = GenParserXmlFile(inputFilePath);
        Assert.True(File.Exists(compilerXmlOutputPath));
        TestHelpers.AssertFilesAreEqualWithoutWhitespace(compilerXmlOutputPath, expectedOutputFile);
    }

    private string GenParserXmlFile(string inputFilePath)
    {
        
        var outputFileName = $"{Path.GetFileNameWithoutExtension(inputFilePath)}.xml";
        var outputFile = Path.Combine(Path.GetDirectoryName(inputFilePath)!, outputFileName);
        var sut = new JackCompilerRunner(inputFilePath, outputFile, true);
        var outputPath = sut.Run();
        return outputPath;
    }
}