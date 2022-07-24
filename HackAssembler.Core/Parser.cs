using System.Text.RegularExpressions;

namespace HackAssembler.Core;

public static class Parser
{
    public static string ParseAInstruction(string assemblyInstruction)
    {
        if (!assemblyInstruction.StartsWith("@")) throw new FormatException("Not an A instruction");
        
        string aOpCode = "0";
        var valueAsInt16 = int.Parse(assemblyInstruction.Trim('@'));
        if (valueAsInt16 < 0) throw new Exception("Addresses must be positive");
        string binary = Convert.ToString(valueAsInt16, 2).PadLeft(15, '0');
        return $"{aOpCode}{binary}";
    }

    public static string ParseCInstruction(string assemblyInstruction)
    {
        assemblyInstruction = Regex.Replace(assemblyInstruction, @"\s+", string.Empty);

        var (destPartAsm, compAsm, jumpAsm) = GetCInstructionParts(assemblyInstruction);

        string? dest = CInstructionTables.DestTable.TryGetOrDefault(destPartAsm, "000");
        
        // Comp already has the a instruction tacked onto the KeyValuePair value
        var comp = CInstructionTables.CompTable[compAsm];
        string? jmp = CInstructionTables.JmpTable.TryGetOrDefault(jumpAsm, "000");

        return $"111{comp}{dest}{jmp}";
    }
    
    /// <summary>
    /// Returns the parts of the given <param name="cInstruction"></param>
    /// </summary>
    /// <param name="cInstruction">C Instruction string</param>
    /// <returns>A string tuple with (dest, comp, jump) instruction parts in that order</returns>
    /// <exception cref="FormatException">If given C instruction contains too many parts due to syntax errors</exception>
    private static (string?, string, string?) GetCInstructionParts(string cInstruction)
    {
        string? dest = null;
        string comp; 
        string? jump = null;
        cInstruction = Regex.Replace(cInstruction, @"\s+", string.Empty);
        var instructionSplitByEquals = cInstruction.Split('=');
        if (instructionSplitByEquals.Length > 2) throw new FormatException($"Too many '=' in {instructionSplitByEquals}");
        
        var semiColonSplit = instructionSplitByEquals.Length == 1
            ? instructionSplitByEquals[0].Split(";")
            : instructionSplitByEquals[1].Split(";");

        comp = semiColonSplit[0];

        if (semiColonSplit.Length == 2)
        {
            jump = semiColonSplit[1];
        }

        if (instructionSplitByEquals.Length > 1)
        {
            dest = instructionSplitByEquals[0];
        }

        return (dest, comp, jump);
    }
}