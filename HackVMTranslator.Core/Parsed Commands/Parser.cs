namespace HackVMTranslator.Core.Parsed_Commands;

internal static class Parser
{
    public static BaseVirtualMachineCommand ToParsedCommand(this string vmLine, int uniqueCommandIndentifer, string staticPrefix)
    {
        var lineSplit = vmLine.Split(' ');
        if (TryAndParseToLogicCommand(lineSplit, uniqueCommandIndentifer, out var parsedCommand));
        if (parsedCommand != null) return parsedCommand;
        
        if (TryAndParseMemoryAccessCommand(lineSplit, staticPrefix, out var memoryAccessCommand));
        if (memoryAccessCommand != null) return memoryAccessCommand;
        
        if (TryAndParseBranchingCommand(lineSplit, out var branchingCommand));
        if (branchingCommand != null) return branchingCommand;

        throw new UnsupportedVmInstructionException($"{vmLine} is not a supported virtual machine instruction");
    }

    private static bool TryAndParseMemoryAccessCommand(string[] lineSplit, string staticPrefix,
        out BaseVirtualMachineCommand? memoryAccessCommand)
    {
        var memoryAccessCommandString = lineSplit[0].ToUpper();
        if (Enum.TryParse(memoryAccessCommandString, out MemoryAccessCommandsTypes memoryAccessCommandsTypes))
        {
            var memorySegment = Enum.Parse<MemorySegment>(lineSplit[1].ToUpper());
            var value = int.Parse(lineSplit[2]);
            {
                memoryAccessCommand = new MemoryAccessCommand(value, staticPrefix, memoryAccessCommandsTypes, memorySegment);
                return true;
            }
        }

        memoryAccessCommand = null;
        return false;
    }

    private static bool TryAndParseToLogicCommand(string[] lineSplit, int uniqueCommandIndentifer,
        out BaseVirtualMachineCommand? parsedCommand)
    {
        var command = lineSplit[0].ToUpper();
        if (Enum.TryParse(command, out BasicLogicalCommandsTypes logicalCommandType))
        {
            {
                parsedCommand = new BasicLogicalCommands(logicalCommandType);
                return true;
            }
        }

        if (Enum.TryParse(command, out ConditionalLogicalCommandTypes conditionalLogicCommandType))
        {
            {
                parsedCommand = new ConditionalLogicCommand(uniqueCommandIndentifer, conditionalLogicCommandType);
                return true;
            }
        }

        parsedCommand = null;
        return false;
    }
    
    private static bool TryAndParseBranchingCommand(string[] lineSplit, out BaseVirtualMachineCommand? branchingCommand)
    {
        if (Enum.TryParse(lineSplit[0].ToUpper().Replace("-", "_"), 
                out BranchingCommandTypes branchingCommandType))
        {
            branchingCommand = new BranchingCommand(lineSplit[1], branchingCommandType);
            return true;
        }
        branchingCommand = null;
        return false;
    }
}