using HackVMTranslator.Core.Command_Translators.Memory_Access_Commands;

namespace HackVMTranslator.Core.Parsed_Commands;

internal class MemoryAccessCommand : BaseVirtualMachineCommand
{
    // For pop command, _value = memory segment address to pop top of stack into
    // For push command, _value = the ACTUAL value you want to push into a memory segment, not the address
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
    
    private string PopCommandToAssembly(string staticPrefix, MemorySegment segment, int segmentAddress)
    {
        switch (segment)
        {
            case MemorySegment.LOCAL:
                return PopMemoryCommandTranslations.ToLocalAssembly(segmentAddress);
            case MemorySegment.ARGUMENT:
                return PopMemoryCommandTranslations.ToArgumentAssembly(segmentAddress);
            case MemorySegment.THIS:
                return PopMemoryCommandTranslations.ToThisAssembly(segmentAddress);
            case MemorySegment.THAT:
                return PopMemoryCommandTranslations.ToThatAssembly(segmentAddress);
            case MemorySegment.CONSTANT:
                throw new InvalidDataException("Cannot perform POP command for constant memory segment");
            case MemorySegment.STATIC:
                return PopMemoryCommandTranslations.ToStaticAssembly(staticPrefix, segmentAddress);
            case MemorySegment.POINTER:
                return PopMemoryCommandTranslations.ToPointerAssembly(segmentAddress);
            case MemorySegment.TEMP:
                return PopMemoryCommandTranslations.ToTempAssembly(segmentAddress);
            default:
                throw new ArgumentOutOfRangeException(nameof(segment), segment, null);
        }
    }
}