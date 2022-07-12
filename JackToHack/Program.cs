// See https://aka.ms/new-console-template for more information

using System.Reflection;
using HackAssembler.Core;

string[] arguments = Environment.GetCommandLineArgs();

if (arguments.Length < 3)
{
    Console.WriteLine("Missing arguments...");
    Console.WriteLine("Example Usage:");
    Console.WriteLine("JackToHack \"path/to/Jack/file.jack\"  \"output/directory/path/\"");
    Console.WriteLine("JackToHack \"path/to/Jack/Directory\"  \"output/directory/path/\"");
    return;
}

var inputFilePath = arguments[1];
var outputDirectory = Path.GetDirectoryName(arguments[1]);
throw new NotImplementedException("JackToHack is not currently implemented");