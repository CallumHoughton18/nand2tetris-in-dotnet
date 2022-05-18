using HackVMTranslator.Core.Command_Translators.Memory_Access_Commands;

namespace HackVMTranslator.Core.Parsed_Commands;

internal class MemoryAccessCommand : BaseVirtualMachineCommand
{
    private readonly int _value;
    private readonly string _staticPrefix;
    private readonly MemoryAccessCommandsTypes _commandType;
    private readonly MemorySegment _memorySegment;
    
    public MemoryAccessCommand(int value, string staticPrefix,
        MemoryAccessCommandsTypes commandType, MemorySegment memorySegment)
    {
        _value = value;
        _staticPrefix = staticPrefix;
        _commandType = commandType;
        _memorySegment = memorySegment;
    }
    public override string ToAssembly()
    {
        switch (_commandType)
        {
            case MemoryAccessCommandsTypes.PUSH:
                return PushCommandToAssembly(_staticPrefix, _memorySegment, _value);
            case MemoryAccessCommandsTypes.POP:
                return PopCommandToAssembly(_staticPrefix, _memorySegment, _value);
            default:
                throw new ArgumentOutOfRangeException();
        }
    } 
    
    private string PushCommandToAssembly(string staticPrefix, MemorySegment segment, int value)
    {
        switch (segment)
        {
            case MemorySegment.LOCAL:
                return PushMemoryCommandTranslations.ToLocalAssembly(value);
            case MemorySegment.ARGUMENT:
                return PushMemoryCommandTranslations.ToArgumentAssembly(value);
            case MemorySegment.THIS:
                return PushMemoryCommandTranslations.ToThisAssembly(value);
            case MemorySegment.THAT:
                return PushMemoryCommandTranslations.ToThatAssembly(value);
            case MemorySegment.CONSTANT:
                return PushMemoryCommandTranslations.ToConstantAssembly(value);
            case MemorySegment.STATIC:
                return PushMemoryCommandTranslations.ToStaticAssembly(staticPrefix, value);
            case MemorySegment.POINTER:
                return PushMemoryCommandTranslations.ToPointerAssembly(value);
            case MemorySegment.TEMP:
                return PushMemoryCommandTranslations.ToTempAssembly(value);
            default:
                throw new ArgumentOutOfRangeException(nameof(segment), segment, null);
        }
    }
    
    private string PopCommandToAssembly(string staticPrefix, MemorySegment segment, int value)
    {
        switch (segment)
        {
            case MemorySegment.LOCAL:
                return PopMemoryCommandTranslations.ToLocalAssembly(value);
            case MemorySegment.ARGUMENT:
                return PopMemoryCommandTranslations.ToArgumentAssembly(value);
            case MemorySegment.THIS:
                return PopMemoryCommandTranslations.ToThisAssembly(value);
            case MemorySegment.THAT:
                return PopMemoryCommandTranslations.ToThatAssembly(value);
            case MemorySegment.CONSTANT:
                throw new InvalidDataException("Cannot perform POP command for constant memory segment");
            case MemorySegment.STATIC:
                return PopMemoryCommandTranslations.ToStaticAssembly(staticPrefix, value);
            case MemorySegment.POINTER:
                return PopMemoryCommandTranslations.ToPointerAssembly(value);
            case MemorySegment.TEMP:
                return PopMemoryCommandTranslations.ToTempAssembly(value);
            default:
                throw new ArgumentOutOfRangeException(nameof(segment), segment, null);
        }
    }
}