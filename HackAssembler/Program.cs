using System.Reflection;
using HackAssembler.Core;

string[] arguments = Environment.GetCommandLineArgs();

if (arguments.Length < 3)
{
    Console.WriteLine("Missing arguments...");
    Console.WriteLine("Example Usage:");
    Console.WriteLine("HackAssembler \"path/to/asm/file.asm\"  \"output/directory/path/\"");
    return;
}

var inputFilePath = arguments[1];
var outputDirectory = Path.GetDirectoryName(arguments[1]);
if (outputDirectory is null)
{
    Console.WriteLine("The input path must contain a directory");
}
var outputFilePath = Path.Combine(outputDirectory!, 
    $"{Path.GetFileNameWithoutExtension(inputFilePath)}.hack");

try
{
    var runner = new HackAssemblerRunner(inputFilePath, outputFilePath);
    var outputPath = runner.Run();
    Console.WriteLine($"Assembly successful, output file path: {outputPath}");
}
catch (AssemblyException e)
{
    Console.WriteLine($"An error occurred during assembly: {e} ");
}
catch (Exception e)
{
    Console.WriteLine($"An unexpected error occured...{e}");
}