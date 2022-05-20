using HackVMTranslator.Core.Command_Translators;

namespace HackVMTranslator.Core.Parsed_Commands;

internal class BranchingCommand : BaseVirtualMachineCommand
{
    private readonly string _labelName;
    private readonly BranchingCommandTypes _command;

    public BranchingCommand(string labelName, BranchingCommandTypes command)
    {
        _labelName = labelName;
        _command = command;
    }
    public override string ToAssembly()
    {
        switch (_command)
        {
            case BranchingCommandTypes.GOTO:
                return BranchingCommandTranslations.GoToToAssembly(_labelName);
            case BranchingCommandTypes.IF_GOTO:
                return BranchingCommandTranslations.IfGoToToAssembly(_labelName);
            case BranchingCommandTypes.LABEL:
                return BranchingCommandTranslations.LabelToAssembly(_labelName);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}