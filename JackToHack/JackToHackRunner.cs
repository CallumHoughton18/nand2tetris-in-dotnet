using HackAssembler.Core;
using HackVMTranslator.Core;
using JackCompiler.Core;

namespace JackToHack;

public class JackToHackRunner
{
    private readonly string _codeLocation;
    private readonly string _outputDirectory;

    public JackToHackRunner(string codeLocation, string outputDirectory)
    {
        _codeLocation = codeLocation;
        if (!Directory.Exists(outputDirectory))
        {
            throw new DirectoryNotFoundException($"{outputDirectory} is not a directory...");
        }
        _outputDirectory = outputDirectory;
    }

    public string Run()
    {
        var compilerOutputFiles = RunJackCompiler();
        var asmFilePath = RunJackVmTranslator(compilerOutputFiles);
        var hackMachineCodeFilePath = RunHackAssembler(asmFilePath);
        return hackMachineCodeFilePath;
    }

    private string RunHackAssembler(string assemblyFilePath)
    {
        var outputFileName = $"{Path.GetDirectoryName(assemblyFilePath)}.hack";
        var outputFilePath = Path.Combine(_outputDirectory, outputFileName);
        var runner = new HackAssemblerRunner(assemblyFilePath, outputFilePath );
        return runner.Run();
    }

    private string RunJackVmTranslator(IEnumerable<string> compilerOutputFiles)
    {
        var asmFiles = new List<string>();

        var outputFiles = compilerOutputFiles as string[] ?? compilerOutputFiles.ToArray();
        var outputFileName = $"{Path.GetDirectoryName(outputFiles.First())}.asm";
        var outputFilePath = Path.Combine(_outputDirectory, outputFileName);
        var compilerOutputDirectory = Path.GetDirectoryName(outputFiles.First());
        
        var vmTranslator = new HackVmTranslatorRunner(compilerOutputDirectory, outputFilePath);

        return outputFilePath;
    }

    private string[] RunJackCompiler()
    {
        var vmFilePaths = GetFilesForConversion("*.jack");
        var compilerOutputPath = "";

        foreach (var vmFile in vmFilePaths)
        {
            var outputFilePath = Path.Combine(_outputDirectory, $"{Path.GetFileNameWithoutExtension(vmFile)}.vm");
            
            var runner = new JackToVmConverter(vmFile, outputFilePath);
            compilerOutputPath = runner.Run();
        }
        
        return Directory.Exists(_outputDirectory) ? Directory.GetFiles(_outputDirectory, "*.vm") : new[]{compilerOutputPath};
    }

    private List<string> GetFilesForConversion(string searchPattern)
    {
        var filesForConversion = new List<string>();
        if (File.Exists(_outputDirectory)) File.Delete(_outputDirectory);

        if (Directory.Exists(_codeLocation))
        {
            filesForConversion = Directory.GetFiles(_codeLocation, searchPattern).ToList();
        }
        else if (File.Exists(_codeLocation))
        {
            filesForConversion.Add(_codeLocation);
        }

        return filesForConversion;
    }
}