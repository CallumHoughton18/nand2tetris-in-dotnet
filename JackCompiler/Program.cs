
using JackCompiler.Core;

string[] arguments = Environment.GetCommandLineArgs();

if (arguments.Length < 3)
{
    Console.WriteLine("Missing arguments...");
    Console.WriteLine("Example Usage:");
    Console.WriteLine("JackCompiler \"path/to/jack/code.jack\"  \"output/directory/path/\"");
    Console.WriteLine("JackCompiler \"path/to/jack/code_directory/\"  \"output/directory/path/\"");

    return;
}

var inputFilePath = arguments[1];
var outputDirectory = Path.GetDirectoryName(arguments[1]);
if (outputDirectory is null)
{
    Console.WriteLine("The input path must contain a directory");
}

try
{
    var runner = new JackCompilerRunner(inputFilePath, outputDirectory);
    var outputPath = runner.Run();
    Console.WriteLine($"vm code generation successful, output path: {outputPath}");
}
catch (Exception e)
{
    Console.WriteLine($"An unexpected error occured...{e}");
}