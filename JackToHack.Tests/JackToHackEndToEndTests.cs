using System.IO;
using Xunit;

namespace JackToHack.Tests;

public class JackToHackEndToEndTests
{
    [Theory]
    [InlineData("./Test Input Files/HelloWorld", "./Expected Output Files/HelloWorld")]

    public void Should_Generate_Machine_Code(string codeDirectory, string outputDirectory)
    {
        var sut = new JackToHackRunner(codeDirectory, outputDirectory);
        var result = sut.Run();
        Assert.True(File.Exists(result));
    }
}