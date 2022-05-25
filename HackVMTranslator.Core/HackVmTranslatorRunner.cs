using System.Linq;
using HackVMTranslator.Core.Command_Translators;

namespace HackVMTranslator.Core;

public class HackVmTranslatorRunner
{
    private readonly string _vmCodeLocation;
    private readonly string _outPath;

    public HackVmTranslatorRunner(string vmCodeLocation, string outPath)
    {
        _vmCodeLocation = vmCodeLocation;
        _outPath = outPath;
    }

    public string Run()
    {
        // slightly messy, but this handles the test cases in the nand2tetris course where a single .vm file
        // should be translated with NO bootstrap code included, where as a directory with multiple .vm files
        // needs to have Sys.vm entry point to run properly (ie the program starts on the computer booting up).
        var vmFilePaths = GetVmFilesForConversion();
        var isMultiFile = vmFilePaths.Count > 1;
        var hasNeededMainAndSysFiles = vmFilePaths.Any(x =>
        {
            var fileName = Path.GetFileName(x);
            return fileName == "Sys.vm";
        });

        if (isMultiFile && !hasNeededMainAndSysFiles)
        {
            throw new ApplicationException("Cannot convert multiple .vm files without Sys.vm and Main.vm files");
        }

        if (isMultiFile)
        {
            WriteBootstrapCode(_outPath);
        }
        
        foreach (var vmFile in vmFilePaths)
        {
            var runner = new VmToHackAsmConverter(vmFile, _outPath);
            runner.Run();
        }

        return _outPath;
    }

    private List<string> GetVmFilesForConversion()
    {
        var vmFilePaths = new List<string>();
        if (File.Exists(_outPath)) File.Delete(_outPath);

        if (Directory.Exists(_vmCodeLocation))
        {
            vmFilePaths = Directory.GetFiles(_vmCodeLocation, "*.vm").ToList();
        }
        else if (File.Exists(_vmCodeLocation))
        {
            vmFilePaths.Add(_vmCodeLocation);
        }

        return vmFilePaths;
    }
    
    /// <summary>
    /// Writes asm that calls Sys.init function and initializes the stack pointer
    /// </summary>
    /// <param name="outPath"></param>
    private void WriteBootstrapCode(string outPath)
    {
        using var writer = new StreamWriter(outPath);
        var sysInitCall = FunctionCommandTranslations.CallToAssembly("Sys.init", 0, 8853348);
        writer.Write($@"
@256
D=A
@0
M=D
{sysInitCall}
");
    }
}