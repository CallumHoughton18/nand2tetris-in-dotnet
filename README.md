# Hack Assembler in C#

A Hack assembler, wrote in C#, for the [nand2tetris](https://www.coursera.org/learn/build-a-computer) course, project 6.

## Considerations

In line with the ethos of nand2tetris I tried to focus more on simplicity, the assembler contains the minimum amount of error checks
and sanitization and small suite of integration tests. As the assembler itself is designed to run against the given .hack files from the nand2tetris course.

There is also no simple way to represent a 'bit' (0 or 1) in C#, especially fixed size arrays of bits (which would have been useful for the command translations). So I instead opted
to just represent the 'bits' as strings (as this is essentially what they are when written to the .hack file anyway). 

The HackAssembler.Core project could be taken and reused across any project you wish, as this is a .NET 6 class library.

## Installation
Build the HackAssembler project in release mode, you can then use the generated binary like:

`HackAssembler "path/to/asm/file.asm"  "output/directory/path/"`

## Tests

A set of integration tests can be ran that assert the generated .hack machine code file output is equal to
the expected one provided by the nand2tetris course. These are located in the HackAssembler.Core.Tests project.

