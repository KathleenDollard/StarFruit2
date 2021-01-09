Imports StarFruit2
Imports System.CommandLine
Imports StarFruit2.Common
Imports System.CommandLine.Invocation
Imports System.CommandLine.Parsing
Imports System
Imports System.Collections.Generic
Imports System.IO

Namespace MethodEntryPoint
   Public Class FormatCommandSource
      Inherits RootCommandSource(Of FormatCommandSource)
      Public Sub New()
         Me.New(Nothing, Nothing)
      End Sub
      Public Sub New(root As RootCommandSource, parent As CommandSourceBase)
         MyBase.New(New Command("format", "Formats source code."), parent)
            folderOption = Getfolder()
            Command.Add(folderOption)
            filesOption = Getfiles()
            Command.Add(filesOption)
            excludeOption = Getexclude()
            Command.Add(excludeOption)
            checkOption = Getcheck()
            Command.Add(checkOption)
            reportOption = Getreport()
            Command.Add(reportOption)
            verbosityOption = Getverbosity()
            Command.Add(verbosityOption)
            Command.Handler = CommandHandler.Create(Function (context As InvocationContext) As Integer
                CurrentCommandSource = Me
                CurrentParseResult = context.ParseResult
                Return 0
                   End Function)
      End Sub

      Public Property folderOption As [Option](Of DirectoryInfo)
      Public Property filesOption As [Option](Of IEnumerable(Of FileInfo))
      Public Property excludeOption As [Option](Of IEnumerable(Of FileInfo))
      Public Property checkOption As [Option](Of Boolean)
      Public Property reportOption As [Option](Of Boolean)
      Public Property verbosityOption As [Option](Of VerbosityLevel)

      Public  Function Getfolder() As [Option](Of DirectoryInfo)
         Dim opt As [Option](Of DirectoryInfo) = New [Option](Of DirectoryInfo)("--folder")
         opt.Description = "Folder to treat the `project` path as a folder of files."
         opt.IsRequired = False
         opt.IsHidden = False
         opt.AddAlias("-f")
         Return opt
   End Function
   Public  Function Getfiles() As [Option](Of IEnumerable(Of FileInfo))
      Dim opt As [Option](Of IEnumerable(Of FileInfo)) = New [Option](Of IEnumerable(Of FileInfo))("--files")
      opt.Description = "A list of relative file or folder paths to include in formatting.All files are formatted if empty."
      opt.IsRequired = False
      opt.IsHidden = False
      opt.AddAlias("-include")
      Return opt
End Function
Public  Function Getexclude() As [Option](Of IEnumerable(Of FileInfo))
   Dim opt As [Option](Of IEnumerable(Of FileInfo)) = New [Option](Of IEnumerable(Of FileInfo))("--exclude")
   opt.Description = "A list of relative file or folder paths to exclude from formatting."
   opt.IsRequired = False
   opt.IsHidden = False
   Return opt
End Function
Public  Function Getcheck() As [Option](Of Boolean)
   Dim opt As [Option](Of Boolean) = New [Option](Of Boolean)("--check")
   opt.Description = "Formats files without saving changes to disk. Terminates with a non-zero exit code if any files were formatted."
   opt.IsRequired = False
   opt.IsHidden = False
   opt.AddAlias("-dry-run")
   Return opt
End Function
Public  Function Getreport() As [Option](Of Boolean)
   Dim opt As [Option](Of Boolean) = New [Option](Of Boolean)("--report")
   opt.Description = "Accepts a file path, which if provided, will produce a json report in the given directory."
   opt.IsRequired = False
   opt.IsHidden = False
   Return opt
End Function
Public  Function Getverbosity() As [Option](Of VerbosityLevel)
   Dim opt As [Option](Of VerbosityLevel) = New [Option](Of VerbosityLevel)("--verbosity")
   opt.Description = "Set the verbosity level."
   opt.IsRequired = False
   opt.IsHidden = False
   Return opt
End Function
End Class
End Namespace
