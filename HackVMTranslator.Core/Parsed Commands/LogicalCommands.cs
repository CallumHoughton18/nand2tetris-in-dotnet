using System.Data;
using HackVMTranslator.Core.Command_Translators;

namespace HackVMTranslator.Core.Parsed_Commands;

internal class ConditionalLogicCommand : BaseVirtualMachineCommand
{
    private readonly int _uniqueCommandIdentifier;
    private readonly ConditionalLogicalCommandTypes _commandType;

    public ConditionalLogicCommand(int uniqueCommandIdentifier, ConditionalLogicalCommandTypes commandType)
    {
        _uniqueCommandIdentifier = uniqueCommandIdentifier;
        _commandType = commandType;
    }

    public override string ToAssembly()
    {
        switch (_commandType)
        {
            case ConditionalLogicalCommandTypes.EQ:
                return LogicalCommandTranslations.EqualCommandToAssembly(_uniqueCommandIdentifier);
            case ConditionalLogicalCommandTypes.GT:
                return LogicalCommandTranslations.GreaterThanCommandToAssembly(_uniqueCommandIdentifier);
            case ConditionalLogicalCommandTypes.LT:
                return LogicalCommandTranslations.LessThanCommandToAssembly(_uniqueCommandIdentifier);
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}

internal class BasicLogicalCommands : BaseVirtualMachineCommand
{
    private readonly BasicLogicalCommandsTypes _commandType;
    
    public BasicLogicalCommands(BasicLogicalCommandsTypes commandType)
    {
        _commandType = commandType;
    }
    
    public override string ToAssembly()
    {
        switch (_commandType)
        {
            case BasicLogicalCommandsTypes.ADD:
                return LogicalCommandTranslations.AddCommandToAssembly();
            case BasicLogicalCommandsTypes.SUB:
                return LogicalCommandTranslations.SubCommandToAssembly();
            case BasicLogicalCommandsTypes.NEG:
                return LogicalCommandTranslations.NegateCommandToAssembly();
            case BasicLogicalCommandsTypes.AND:
                return LogicalCommandTranslations.AndCommandToAssembly();
            case BasicLogicalCommandsTypes.OR:
                return LogicalCommandTranslations.OrCommandToAssembly();
            case BasicLogicalCommandsTypes.NOT:
                return LogicalCommandTranslations.NotCommandToAssembly();
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
}