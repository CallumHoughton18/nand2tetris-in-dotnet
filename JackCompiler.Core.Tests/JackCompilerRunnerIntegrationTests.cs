using System.IO;
using Xunit;

namespace JackCompiler.Core.Tests;

public class UnitTest1
{
    [Theory]
    [InlineData("./Test Input Files/Main.jack")]
    public void Should_Give_Correct_Output_For_Asm_Without_Symbols(string inputFilePath)
    {
        var outputFilePath = AssertFileGeneratedSuccessfully(inputFilePath);
    }

    private string AssertFileGeneratedSuccessfully(string inputFilePath)
    {
        var outputFile = $"./{Path.GetFileNameWithoutExtension(inputFilePath)}.xml";
        var sut = new JackCompilerRunner(inputFilePath, outputFile);
        var outputPath = sut.Run();
        Assert.True(File.Exists(outputPath));

        return outputPath;
    }
}