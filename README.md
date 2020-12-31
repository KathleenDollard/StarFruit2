# StarFruit2

StarFruit is a way to define a [System.CommandLine](https://github.com/dotnet/command-line-api) CLI, without having to know much, or anything, about System.CommandLine. If you want to build a CLI today, you want to use System.CommandLine directly. StarFruit aims to simplify this.

This is currently in prototype stage and I do not yet have a NuGet package available.

## How to use it

The programmer defines models of the data that they need users of their tool to enter. This can be via a class or a method.

A couple of convention based hints tell StarFruit what is an argument, option or subcommand. Additional customizations can be done with attributes.

### Method based CLI

Simple CLIs with no subcommands can be expressed with a method. The code below supplies a complete CLI with help, tab completion (if [registered]()), diagramming, strong typing, and the many other features of System.CommandLine. Posix style is used.

* The method is discovered by its name being passed to the Create method. It must be unique in the class.
* Parameters are options by default.
* Arguments are specified with an "Arg" suffix or an Argument attribute
* Help description comes from the XML documentation.
* Strong typing is maintained. 
  * Bool values can be listed as simply the option without argument (or supply arguments).
  * IEnumerable indicates that multiple files can be entered.
  * Enums such as the VerbosityLevel are respected and used for tab completion 
  * System.CommandLine supports tab completion on some types like DirectoryInfo.
* Aliases and required are defined via attributes.

```c#
class Program
{
    static int Main(string[] args)
        => return CommandSource.Create(nameof(Format)).Run(args);

    /// <summary> Formats source code. </summary>
    /// <param name="folder">Folder to treat the `project` path as a folder of files.</param>
    /// <param name="files">A list of relative file or folder paths to include in formatting.All files are formatted if empty.</param>
    /// <param name="verbosity">Set the verbosity level.</param>
    /// <returns></returns>
    public static int Format([Aliases("f")] DirectoryInfo folder,
                             [Aliases("include")] IEnumerable<FileInfo> files,
                             VerbosityLevel verbosity)
    {
        Console.WriteLine(@$"Folder: {folder} Files: {files} Verbosity: {verbosity}");
        return 0;
    }
}
```

### CLIs with SubCommands - class based CLI

Class based CLI support subcommands. All of the features of Method based CLIs are supported. The method is wrapped in a class that defines the parent CLI command. You would generally use subcommands only when you have several subcommands, but I have used only one to keep the sample simple.

> NOTE: The format sometimes used for .NET (Global) Tools, such as dotnet-toolname is **not** a subcommand. This is a single executable entry point with any CLI you chose to attach.

```c#
/// <summary>
/// This is the entry point, the end user types the executable name
/// </summary>
public partial class CliRoot : ICliRoot
{
    /// <summary> </summary>
    /// <param name="ctorParam">This is a constructor parameter</param>
    public CliRoot(bool ctorParam) { }

    /// <summary> This is a string property </summary>
    [Aliases("o")]
    [Required]
    public string StringProperty { get; set; } = "abc";

    /// <summary> Use this to find things </summary>
    /// <param name="stringOption">This is an string argument</param>
    /// <param name="boolOption">This is an bool argument</param>
    /// <param name="intArg">This is an integer argument</param>
    /// <returns></returns>
    public int Find(string stringOption, bool boolOption, int intArg = 42)
    {
        Console.WriteLine($"StringProperty: {StringProperty}   stringOption: {stringOption}   boolOption: {boolOption}    intArg: {intArg}");
        return  0;
    }
}
```

### Deeper CLI

CLIs can be deeply nested by using inheritance. The class with your command in it can derived from another class that would be its parent command. While this may not be what you expected, it allows the data to be naturally available in the executing method.

## A little about the code

StarFruit started out to be a way to insulate programmers from needing to understand System.CommandLine to use it. Its a powerful parser, but takes some getting use to. StarFruit2 gives you the power from C# code you are familiar with, via Roslyn Source Generation.

As the prototype evolved, it wound up illustrating several things:

* Wrapping an niche problem in a common interface
* Splitting up source generation steps, most essentially reading and evaluating source from outputting source
* How to test and validate generators
* A mechanism for outputting source that is language agnostic and I think improves readability of non-trivial templates

I definitely intend this to be an illustration of how to write a non-trivial generator in a way that will be maintainable. That's the thing about generators. Any competent coder will succeed at the job of outputting code. However, the results for non-trivial cases will not be maintainable. And, even trivial generators need to be fully testable in isolation. I decided to use a separate repo for my writing on code generation, but since it is nothing more than document headings, it is currently private.

The mechanism for outputting code (currently called FluentDom) is intended to be split out, but I feel like it is too early in its life. If you want to use it in your generator, I would like to chat with you - you can open an issue here.

And yes, there was a StarFruit one. It was reflection based. I learned a lot from it, but see no benefit to it in a world with Roslyn Source Generators so have abandoned it.

## Help out!

This is currently in an early prototype stage, and contributors are welcome. I am working on issues, but if you are interested in something, post an issue. The focus is on creating a mechanism for a complex source code generators that isolate the sections to allow folks to portion on only one part. I talked about how  thought about and built this in the [.NET Community Standup Oct 8, 2020](https://www.youtube.com/watch?v=A4479Etdx4I&list=PL1rZQsJPBU2St9-Mz1Kaa7rofciyrwWVx&index=0).

