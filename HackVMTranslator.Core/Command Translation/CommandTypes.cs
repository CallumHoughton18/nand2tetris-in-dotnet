namespace HackVMTranslator.Core.Command_Translation;

internal class LogicalCommand : BaseVirtualMachineCommand
{
    public LogicalCommands Command { get; init; }
}

internal class MemoryAccessCommand : BaseVirtualMachineCommand
{
    public MemoryAccessCommands Command { get; init; }
}

abstract class BaseVirtualMachineCommand {}

// internal static class Test
// {
//     public static string PatternMatchingTest<T>(T command) where T: BaseVirtualMachineCommand
//     {
//         return command switch
//         {
//             LogicalCommand{ Command: LogicalCommands.EQ } => VmToAssembly.EqualCommandToAssembly()
//         };
//         // switch (command)
//         // {
//         //     case LogicalCommand:
//         //         break;
//         //     case MemoryAccessCommand:
//         //         break;
//         // }
//     }
// }