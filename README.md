# StarFruit2

StarFruit is a way to define a System.CommandLine CLI, without having to know much, or anything, about System.CommandLine. If you want to build a CLI today, you want to use System.CommandLine directly. StarFruit aims to simplify this.

This is currently in an early prototype stage, and contributors are welcome. I am working on issues, but if you are interested in something, post an issue. The focus is on creating a mechanism for a complex source code generators that isolate the sections to allow folks to portion on only one part. I talked about how  thought about and built this in the [.NET Community Standup Oct 8, 2020](https://www.youtube.com/watch?v=A4479Etdx4I&list=PL1rZQsJPBU2St9-Mz1Kaa7rofciyrwWVx&index=0).

This is the source generated version of StarFruit. I expect to abandon StarFruit because I think source generation is a better tool for this problem than reflection. 

The goal is to allow users to build [System.CommandLine](https://github.com/dotnet/command-line-api) systems without having too know much about System.CommandLine. 

The programmer defines models of the data that they need users of their tool to enter. A couple of hints tell StarFruit what is an argument, option or subcommand. Additional customizations can be done with attributes. 

This is also an example of one approach to developing complex source generators. If you have evern worked with Roslyn, you know it can be a challenge. I think it valuable to separate creation of complex source generaitors into two distinct steps: 

* Evaluating the syntax tree to understand what you want to build, and creating an interim structure that represents this
* Creating a string that can be used in a source generator

Separaring these concerns lets you fully test each part. And in the case of StarFruit, it also allows alternate input that I think we will use for templates via `dotnet new`.

The current status is that thought went into the interim structure as part of StarFruit(1). Much of the effort so far on StarFruit2 was to build a testable maintainable pipeline. Much work is still needed to have this actually be useful for creating System.CommandLine classes. 

_Using .NET Interactive to test generated source code (as shown in the Community standup) is in StarFruit2.Tests.CompileAndTest.cs_
