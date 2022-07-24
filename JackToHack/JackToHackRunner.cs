using HackAssembler.Core;
using HackVMTranslator.Core;
using JackCompiler.Core;

namespace JackToHack;

public class JackToHackRunner
{
    private readonly string _codeLocation;
    private readonly string _outputDirectory;
    private readonly string _jackOSLocation = "./JackOS/custom";

    public JackToHackRunner(string codeLocation, string outputDirectory)
    {
        _codeLocation = codeLocation;
        if (!Directory.Exists(outputDirectory))
        {
            Directory.CreateDirectory(outputDirectory);
        }
        _outputDirectory = outputDirectory;
    }

    public void CleanUpOutputDirectory()
    {
        var filesForDeletion =
            Directory.GetFiles(_outputDirectory)
                .Where(x => x.EndsWith(".jack") | x.EndsWith(".vm") | x.EndsWith(".asm") | x.EndsWith(".hack"));
        
        foreach (var file in filesForDeletion)
        {
            File.Delete(file);
        }
    }

    public string Run()
    {
        CleanUpOutputDirectory();
        var compilerOutputFiles = RunJackCompiler().ToList();
        var osVmFilePaths = CopyJackOS().ToList();
        compilerOutputFiles.AddRange(osVmFilePaths);
        var asmFilePath = RunJackVmTranslator(compilerOutputFiles);
        var hackMachineCodeFilePath = RunHackAssembler(asmFilePath);
        return hackMachineCodeFilePath;
    }

    private string RunHackAssembler(string assemblyFilePath)
    {
        
        var outputFileName = $"{Path.GetFileNameWithoutExtension(assemblyFilePath)}.hack";
        var outputFilePath = Path.Combine(_outputDirectory, outputFileName);
        var runner = new HackAssemblerRunner(assemblyFilePath, outputFilePath );
        return runner.Run();
    }

    private IEnumerable<string> CopyJackOS()
    {
        var files = Directory.GetFiles(_jackOSLocation);
        foreach (var jackOSVmFile in files)
        {
            var outputFilePath = Path.Combine(_outputDirectory, Path.GetFileName(jackOSVmFile));
            File.Copy(jackOSVmFile, outputFilePath);
            yield return outputFilePath;
        }
    }

    private string RunJackVmTranslator(IEnumerable<string> vmFilePaths)
    {
        var outputFiles = vmFilePaths as string[] ?? vmFilePaths.ToArray();
        
        var outputFileName = $"{Path.GetDirectoryName(outputFiles.First())}.asm";
        var outputFilePath = Path.Combine(_outputDirectory, Path.GetFileName(outputFileName));
        var compilerOutputDirectory = Path.GetDirectoryName(outputFiles.First());
        
        var vmTranslator = new HackVmTranslatorRunner(compilerOutputDirectory, outputFilePath);
        vmTranslator.Run();

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