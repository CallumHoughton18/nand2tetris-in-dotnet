# Hack ToolChain in C# .NET

This repo contains C# .NET implementations of the software projects required in the [nand2tetris](https://www.coursera.org/learn/build-a-computer) course.

## Hack Assembler in C#

A Hack assembler, wrote in C#, for the [nand2tetris](https://www.coursera.org/learn/build-a-computer) course, project 6.

### Considerations

In line with the ethos of nand2tetris I tried to focus more on simplicity, the assembler contains the minimum amount of error checks
and sanitization and small suite of integration tests. As the assembler itself is designed to run against the given .hack files from the nand2tetris course.

There is also no simple way to represent a 'bit' (0 or 1) in C#, especially fixed size arrays of bits (which would have been useful for the command translations). So I instead opted
to just represent the 'bits' as strings (as this is essentially what they are when written to the .hack file anyway). 

The HackAssembler.Core project could be taken and reused across any project you wish, as this is a .NET 6 class library.

### Installation
Build the HackAssembler project in release mode, you can then use the generated binary like:

`HackAssembler "path/to/asm/file.asm"  "output/directory/path/"`

### Tests

A set of integration tests can be ran that assert the generated .hack machine code file output is equal to
the expected one provided by the nand2tetris course. These are located in the HackAssembler.Core.Tests project.

## VMTranslator in C#

A VM Translator, wrote in C#, for the [nand2tetris](https://www.coursera.org/learn/build-a-computer) course, project 7-8.

### Installation
Build the HackVMTranslator project in release mode, you can then use the generated binary like:

`HackVMTranslator "path/to/vm/file.vm"  "output/directory/path/"`

### Considerations
As this is a learning exercise getting something correct and passing the nand2tetris test files was the main focus,
rather than performance. So there may well be a few bottle necks if you run extremely large .vm files.

## Grid Drawer in Jack

In the `GridDrawerInJack` directory there are some Jack files which can be compiled and ran in the VM Emulator. This application draws a grid on the screen and allows the user to move the cursor with the arrow keys and fill cells to create drawings.

### Tests

As the assembly generated can vary greatly but still be a correct translation the integration tests purely checks if the vm file
as converted successfully, it then copies the generated .asm file into the `nand2tetris CPU Emulator Test Files` build directory to be used
by nand2tetris's .tst test files. You then need to use the CPU emulator provided by nand2tetris to run the .tst test files.

So as an example, the `BasicTest.vm` translation test, when running the test in debug mode, would copy the generated .asm file into
`..../Nand2TetrisToolChain/HackVMTranslator.Core.Tests/bin/debug/net6.0/nand2tetris CPU Emulator Test Files/BasicTest.asm`

Not the best solution, but it works well enough to use against their test files which is how they expect you to test the generated .asm files anyway

