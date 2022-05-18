using HackVMTranslator.Core.Parsed_Commands;

namespace HackVMTranslator.Core;

public class VmTranslator
{
    private readonly string _staticPrefix;
    private const string CommentString = "//";

    public VmTranslator(string staticPrefix)
    {
        _staticPrefix = staticPrefix;
    }

    public void ConvertToAssembly(StreamReader streamReader, StreamWriter streamWriter)
    {
        string? line;
        int index = 0;
        while (!streamReader.EndOfStream)
        {
            line = streamReader.ReadLine()?.Trim().Split(CommentString)[0];
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith(CommentString)) continue;

            var parsedCommand = line.ToParsedCommand(index, _staticPrefix);
            var assemblyChunk = parsedCommand.ToAssembly();
            streamWriter.Write(assemblyChunk);

            index++;
        }
    }
}