using System.IO;
using Xunit;

namespace JackCompiler.Core.Tests;

public class JackCompilerRunnerIntegrationTests
{
    [Theory]
    [InlineData("./Test Input Files/Seven/Main.jack", "./Expected Output Files/Seven/Main.vm")]
    [InlineData("./Test Input Files/Average/Main.jack", "./Expected Output Files/Average/Main.vm")]
    [InlineData("./Test Input Files/Complex Arrays/Main.jack", "./Expected Output Files/Complex Arrays/Main.vm")]
    public void Should_Give_Correct_Compiler_Output_For_Single_File(string inputFilePath, string expectedOutputFile)
    {
        var vmFilePath = GenVmFile(inputFilePath);
        Assert.True(File.Exists(vmFilePath));
        TestHelpers.AssertFilesAreEqualLineByLine(vmFilePath, expectedOutputFile);
    }

    [Theory]
    [InlineData("./Test Input Files/Pong", "./Expected Output Files/Pong")]
    [InlineData("./Test Input Files/Square", "./Expected Output Files/Square")]
    [InlineData("./Test Input Files/Convert To Bin", "./Expected Output Files/Convert To Bin")] 
    public void Should_Give_Correct_Compiler_Output_For_Directory(string inputDirectory, string expectedFilesDirectory)
    {
        var outputFiles = GenVmFilesFromDirectory(inputDirectory);
        foreach (var outputFile in outputFiles)
        {
            var fileName = Path.GetFileName(outputFile);
            var expectedOutputFile = Path.Combine(expectedFilesDirectory, fileName);
            TestHelpers.AssertFilesAreEqualLineByLine(outputFile, expectedOutputFile);
        }
    }

    private string[] GenVmFilesFromDirectory(string inputDirectory)
    {
        var outputDirectory = inputDirectory;
        var sut = new JackCompilerRunner(inputDirectory, outputDirectory);
        var outputPath = sut.Run();
        return Directory.GetFiles(outputPath, "*.vm");
    }

    private string GenVmFile(string inputFilePath)
    {
        var outputFileName = $"{Path.GetFileNameWithoutExtension(inputFilePath)}.vm";
        var outputFile = Path.Combine(Path.GetDirectoryName(inputFilePath)!, outputFileName);
        var sut = new JackCompilerRunner(inputFilePath, outputFile);
        var outputPath = sut.Run();
        return outputPath;
    }
}