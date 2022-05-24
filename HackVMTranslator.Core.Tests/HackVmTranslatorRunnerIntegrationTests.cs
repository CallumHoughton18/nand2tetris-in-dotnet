using System.IO;
using Xunit;

namespace HackVMTranslator.Core.Tests;

public class HackVmTranslatorRunnerIntegrationTests
{
    [Theory]
    [InlineData("./Test Input Files/SimpleAdd.vm")]
    [InlineData("./Test Input Files/StackTest.vm")]
    [InlineData("./Test Input Files/BasicTest.vm")]
    [InlineData("./Test Input Files/StaticTest.vm")]
    [InlineData("./Test Input Files/PointerTest.vm")]
    [InlineData("./Test Input Files/BasicLoop.vm")]
    [InlineData("./Test Input Files/FibonacciSeries.vm")]
    [InlineData("./Test Input Files/SimpleFunction.vm")]
    public void Should_Give_Correct_Output_For_Asm_Without_Symbols(string inputFilePath)
    {        
        var outputFilePath = AssertFileGeneratedSuccessfully(inputFilePath);
        CopyFileToNand2TetrisTestFolder(outputFilePath);
    }

    private string AssertFileGeneratedSuccessfully(string inputFilePath)
    {
        var outputFile = $"./{Path.GetFileNameWithoutExtension(inputFilePath)}.asm";
        var sut = new HackVmTranslatorRunner(inputFilePath, outputFile);
        var outputPath = sut.Run();
        Assert.True(File.Exists(outputPath));

        return outputPath;
    }

    private void CopyFileToNand2TetrisTestFolder(string outputFilePath)
    {
        var parentDirectory = Directory.GetParent(outputFilePath);
        var nand2TetrisPath = Path.Combine(parentDirectory!.FullName, "nand2tetris CPU Emulator Test Files");
        var fileName = Path.GetFileName(outputFilePath);
        File.Copy(outputFilePath, Path.Combine(nand2TetrisPath, fileName), true);
    }
}