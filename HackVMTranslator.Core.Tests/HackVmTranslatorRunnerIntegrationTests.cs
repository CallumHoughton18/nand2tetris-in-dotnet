using System.IO;
using Xunit;

namespace HackVMTranslator.Core.Tests;

public class HackVmTranslatorRunnerIntegrationTests
{
    [Theory]
    [InlineData("./Test Input Files/SimpleAdd.vm", "./Expected Output Files/SimpleAdd.asm")]
    [InlineData("./Test Input Files/StackTest.vm", "./Expected Output Files/StackTest.asm")]
    [InlineData("./Test Input Files/BasicTest.vm", "./Expected Output Files/BasicTest.asm")]
    [InlineData("./Test Input Files/StaticTest.vm", "./Expected Output Files/Statictest.asm")]
    [InlineData("./Test Input Files/PointerTest.vm", "./Expected Output Files/PointerTest.asm")]
    public void Should_Give_Correct_Output_For_Asm_Without_Symbols(string inputFilePath, string expectedOutputFilePath)
    {        
        AssertFilesAreEqual(inputFilePath, expectedOutputFilePath);
    }

    private void AssertFilesAreEqual(string inputFilePath, string expectedOutputFilePath)
    {
        var outputFile = $"./{Path.GetFileNameWithoutExtension(inputFilePath)}.asm";
        var sut = new HackVmTranslatorRunner(inputFilePath, outputFile);
        var outputPath = sut.Run();
    
        // var actualOutputLines = File.ReadAllLines(outputPath);
        // Assert.True(expectedOutputLines.Length == actualOutputLines.Length);
        // for (int i = 0; i < expectedOutputLines.Length; i++)
        // {
        //     var expectedOutputLine = expectedOutputLines[i];
        //     var actualOutputLine = actualOutputLines[i];
        //     Assert.True(expectedOutputLine == actualOutputLine, $"Mismatch on on line {i+1}");
        // }
    }
}