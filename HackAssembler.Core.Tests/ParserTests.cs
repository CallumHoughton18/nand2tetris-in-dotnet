using Xunit;

namespace HackAssembler.Core.Tests;

public class ParserTests
{
    // These tests were used on the go as I was writing the parser class,
    // I opted instead to focus on the integration tests to verify the file contents match rather
    // than bunch of unit tests for all the functions.
    
    [Fact]
    public void Should_Convert_To_16_Bit_A_Instruction()
    {
        string aInstructionAssembly = "@21";
        string aInstructionParsed = Parser.ParseAInstruction(aInstructionAssembly);
        
        Assert.Equal("0000000000010101", aInstructionParsed);
    }

    [Fact]
    public void Should_Convert_To_16_Bit_C_Instruction()
    {
        string cInstructionAssembly = "MD=D+1";
        string cInstructionParsed = Parser.ParseCInstruction(cInstructionAssembly);
        Assert.Equal("1110011111011000", cInstructionParsed);
    }
}