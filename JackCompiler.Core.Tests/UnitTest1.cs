using System.IO;
using Xunit;

namespace JackCompiler.Core.Tests;

public class UnitTest1
{
    [Theory]
    [InlineData("./Test Input Files/Seven/Main.jack", "./Test Input Files/Seven/Main.vm")]
    [InlineData("./Test Input Files/Average/Main.jack", "./Test Input Files/Average/Main.vm")]
    [InlineData("./Test Input Files/Pong/Ball.jack", "./Test Input Files/Pong/Ball.vm")]
    [InlineData("./Test Input Files/Pong/Bat.jack", "./Test Input Files/Pong/Bat.vm")]
    [InlineData("./Test Input Files/Pong/Main.jack", "./Test Input Files/Pong/Main.vm")]
    [InlineData("./Test Input Files/Pong/PongGame.jack", "./Test Input Files/Pong/Main.vm")]

    public void Should_Give_Correct_Compiler_Output(string inputFilePath, string expectedOutputFile)
    {
        var vmFilePath = GenVmFile(inputFilePath);
        Assert.True(File.Exists(vmFilePath));
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