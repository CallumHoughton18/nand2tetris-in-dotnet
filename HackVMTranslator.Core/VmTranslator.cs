namespace HackVMTranslator.Core;

public class VmTranslator
{
    private const string CommentString = "//";

    public void ConvertToAssembly(string staticPrefix, StreamReader streamReader, StreamWriter streamWriter)
    {
        string? line;
        int index = 0;
        while (!streamReader.EndOfStream)
        {
            line = streamReader.ReadLine()?.Trim().Split(CommentString)[0];
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith(CommentString)) continue;

            var lineSplit = line.Split(' ');
            var command = lineSplit[0].ToCommandType();

            switch (command)
            {
                case VmCommands.ADD:
                    streamWriter.Write(VmToAssembly.AddCommandToAssembly());
                    break;
                case VmCommands.SUB:
                    streamWriter.Write(VmToAssembly.SubCommandToAssembly());
                    break;
                case VmCommands.NEG:
                    streamWriter.Write(VmToAssembly.NegateCommandToAssembly());
                    break;
                case VmCommands.EQ:
                    streamWriter.Write(VmToAssembly.EqualCommandToAssembly(index));
                    break;
                case VmCommands.GT:
                    streamWriter.Write(VmToAssembly.GreaterThanCommand(index));
                    break;
                case VmCommands.LT:
                    streamWriter.Write(VmToAssembly.LessThanCommand(index));
                    break;
                case VmCommands.AND:
                    streamWriter.Write(VmToAssembly.AndCommand());
                    break;
                case VmCommands.OR:
                    streamWriter.Write(VmToAssembly.OrCommand());
                    break;
                case VmCommands.NOT:
                    streamWriter.Write(VmToAssembly.NotCommand());
                    break;
                case VmCommands.POP:
                    var popSegment = Enum.Parse<MemorySegment>(lineSplit[1].ToUpper());
                    var popValue = int.Parse(lineSplit[2]);
                    streamWriter.Write(VmToAssembly.PopCommandToAssembly(staticPrefix, popSegment, popValue));
                    break;
                case VmCommands.PUSH:
                    var segment = Enum.Parse<MemorySegment>(lineSplit[1].ToUpper());
                    var value = int.Parse(lineSplit[2]);
                    streamWriter.Write(VmToAssembly.PushCommandToAssembly(staticPrefix, segment, value));
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            index++;
        }
    }
}