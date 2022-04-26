using System.IO;
using Xunit;

namespace HackAssembler.Core.Tests;

public class HackAssemblerRunnerIntegrationTests
{
    [Theory]
    [InlineData("./Test Input Files/Without Symbols/Add.asm", "./Expected Output Files/Add.hack")]
    [InlineData("./Test Input Files/Without Symbols/MaxL.asm", "./Expected Output Files/MaxL.hack")]
    [InlineData("./Test Input Files/Without Symbols/PongL.asm", "./Expected Output Files/PongL.hack")]
    [InlineData("./Test Input Files/Without Symbols/RectL.asm", "./Expected Output Files/RectL.hack")]
    public void Should_Give_Correct_Output_For_Asm_Without_Symbols(string inputFilePath, string expectedOutputFilePath)
    {        
        AssertFilesAreEqual(inputFilePath, expectedOutputFilePath);
    }
    
    [Theory]
    [InlineData("./Test Input Files/With Symbols/Max.asm", "./Expected Output Files/Max.hack")]
    [InlineData("./Test Input Files/With Symbols/Pong.asm", "./Expected Output Files/Pong.hack")]
    [InlineData("./Test Input Files/With Symbols/Rect.asm", "./Expected Output Files/RectL.hack")]
    public void Should_Give_Correct_Output_For_Asm_With_Symbols(string inputFilePath, string expectedOutputFilePath)
    {
        AssertFilesAreEqual(inputFilePath, expectedOutputFilePath);
    }

    private void AssertFilesAreEqual(string inputFilePath, string expectedOutputFilePath)
    {
        var expectedOutputLines = File.ReadAllLines(expectedOutputFilePath);
        var outputFile = $"./{Path.GetFileNameWithoutExtension(inputFilePath)}.jack";
        var sut = new HackAssemblerRunner(inputFilePath, outputFile);
        var outputPath = sut.Run();

        var actualOutputLines = File.ReadAllLines(outputPath);
        Assert.True(expectedOutputLines.Length == actualOutputLines.Length);
        for (int i = 0; i < expectedOutputLines.Length; i++)
        {
            var expectedOutputLine = expectedOutputLines[i];
            var actualOutputLine = actualOutputLines[i];
            Assert.True(expectedOutputLine == actualOutputLine, $"Mismatch on on line {i+1}");
        }
    }
}