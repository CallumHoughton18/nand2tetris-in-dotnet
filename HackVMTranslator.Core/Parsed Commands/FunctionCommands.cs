using HackVMTranslator.Core.Command_Translators;

namespace HackVMTranslator.Core.Parsed_Commands;


internal class CallFunctionCommand : BaseVirtualMachineCommand
{
    private readonly string _functionName;
    private readonly uint _numberOfArgs;
    private readonly uint _index;

    public CallFunctionCommand(string functionName, uint numberOfArgs, uint index)
    {
        _functionName = functionName;
        _numberOfArgs = numberOfArgs;
        _index = index;
    }
    public override string ToAssembly()
    {
        return FunctionCommandTranslations.CallToAssembly(_functionName, _numberOfArgs, _index);
    }
}

internal class FunctionFunctionCommand : BaseVirtualMachineCommand
{
    private readonly string _functionName;
    private readonly uint _numberOfLocalVars;
    private readonly uint _index;

    public FunctionFunctionCommand(string functionName, uint numberOfLocalVars, uint index)
    {
        _functionName = functionName;
        _numberOfLocalVars = numberOfLocalVars;
        _index = index;
    }
    public override string ToAssembly()
    {
        return FunctionCommandTranslations.FunctionToAssembly(_functionName, _numberOfLocalVars, _index);
    }
}
internal class ReturnFunctionCommand : BaseVirtualMachineCommand
{
    public override string ToAssembly()
    {
        return FunctionCommandTranslations.ReturnToAssembly();
    }
}