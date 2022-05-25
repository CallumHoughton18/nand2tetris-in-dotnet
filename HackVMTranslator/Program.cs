using HackVMTranslator.Core;

string[] arguments = Environment.GetCommandLineArgs();

if (arguments.Length < 3)
{
    Console.WriteLine("Missing arguments...");
    Console.WriteLine("Example Usage:");
    Console.WriteLine("HackVMTranslator \"path/to/vm/file.vm\"  \"output/directory/path/\"");
    Console.WriteLine("HackVMTranslator \"path/to/vmfiles/directory/\"  \"output/directory/path/\"");

    return;
}

var inputFilePath = arguments[1];
var outputDirectory = Path.GetDirectoryName(arguments[1]);
if (outputDirectory is null)
{
    Console.WriteLine("The input path must contain a directory");
}
var outputFilePath = Path.Combine(outputDirectory!, 
    $"{Path.GetFileNameWithoutExtension(inputFilePath)}.asm");

try
{
    var runner = new HackVmTranslatorRunner(inputFilePath, outputFilePath);
    var outputPath = runner.Run();
    Console.WriteLine($"Assembly successful, output file path: {outputPath}");
}
catch (Exception e)
{
    Console.WriteLine($"An unexpected error occured...{e}");
}