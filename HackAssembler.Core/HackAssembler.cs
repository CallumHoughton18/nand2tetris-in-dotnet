namespace HackAssembler.Core;

public class HackAssembler
{
    private const string CommentString = "//";
    public void ConvertToMachineCode(StreamReader streamReader, StreamWriter streamWriter)
    {
        int variableSymbolAddress = 16;
        int currentLineNumber = 1;

        try
        {
            var symbolTable = new SymbolTable();
            symbolTable.GetLabelSymbols(streamReader, CommentString);
            
            string? line;
            while (!streamReader.EndOfStream)
            {
                line = streamReader.ReadLine()?.Trim().Split(CommentString)[0];
                if (string.IsNullOrWhiteSpace(line) || line.StartsWith(CommentString) || 
                    (line.StartsWith("(") && line.EndsWith(")")))
                {
                    currentLineNumber++;
                    continue;
                }
                
                if (line.StartsWith("@"))
                {
                    var (aInstructionConverted, newVariableSymbolAddress) 
                        = ConvertAInstruction(line, symbolTable, variableSymbolAddress);
                    
                    streamWriter.WriteLine(aInstructionConverted);
                    variableSymbolAddress = newVariableSymbolAddress;
                }
                else
                {
                    var cInstructionConverted = Parser.ParseCInstruction(line);
                    streamWriter.WriteLine(cInstructionConverted);
                }

                currentLineNumber++;
            }
        }
        catch (FormatException e)
        {
            throw new AssemblyException($"Formatting error on line {currentLineNumber}", e);
        }
    }

    private (string, int) ConvertAInstruction(string line, SymbolTable symbolTable,
        int variableSymbolAddress)
    {
        var variableName = line.TrimStart('@');
        var variableIsAddress = int.TryParse(variableName, out int _);
        if (!variableIsAddress && symbolTable.TryGetSymbol(variableName, out int variableValue))
        {
            line = line.Replace(variableName, variableValue.ToString());
        }
        else if (!variableIsAddress)
        {
            symbolTable[variableName] = variableSymbolAddress;
            line = line.Replace(variableName, variableSymbolAddress.ToString());
            variableSymbolAddress++;
        }

        var aInstructionConverted = Parser.ParseAInstruction(line);
        return (aInstructionConverted, variableSymbolAddress);
    }
}