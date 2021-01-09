Imports StarFruit2
Imports System.CommandLine
Imports StarFruit2.Common
Imports System.CommandLine.Invocation
Imports System.CommandLine.Parsing
Imports System
Imports System.Collections.Generic
Imports System.IO

Namespace SingleLayerCli
    Public Class DotnetFormatCommandSource
        Inherits RootCommandSource(Of DotnetFormatCommandSource)
        Public Sub New()
            Me.New(Nothing, Nothing)
        End Sub
        Public Sub New(root As RootCommandSource, parent As CommandSourceBase)
            MyBase.New(New Command("dotnet-format", ""), parent)
            FolderOption = GetFolder()
            Command.Add(FolderOption)
            FilesOption = GetFiles()
            Command.Add(FilesOption)
            ExcludeOption = GetExclude()
            Command.Add(ExcludeOption)
            CheckOption = GetCheck()
            Command.Add(CheckOption)
            ReportOption = GetReport()
            Command.Add(ReportOption)
            VerbosityOption = GetVerbosity()
            Command.Add(VerbosityOption)
            InvokeCommand = New InvokeCommandSource(Me, Me)
            Command.AddCommand(InvokeCommand.Command)
            Command.Handler = CommandHandler.Create(Function(context As InvocationContext) As Integer
                                                        CurrentCommandSource = Me
                                                        CurrentParseResult = context.ParseResult
                                                        Return 0
                                                    End Function)
        End Sub

        Public Property FolderOption As [Option](Of DirectoryInfo)
        Public Property FilesOption As [Option](Of IEnumerable(Of FileInfo))
        Public Property ExcludeOption As [Option](Of IEnumerable(Of FileInfo))
        Public Property CheckOption As [Option](Of Boolean)
        Public Property ReportOption As [Option](Of Boolean)
        Public Property VerbosityOption As [Option](Of VerbosityLevel)
        Public Property InvokeCommand As InvokeCommandSource

        Public Function GetFolder() As [Option](Of DirectoryInfo)
            Dim opt As [Option](Of DirectoryInfo) = New [Option](Of DirectoryInfo)("--folder")
            opt.Description = "Folder Whether to treat the <project> path as a folder of files."
            opt.IsRequired = False
            opt.IsHidden = False
            opt.AddAlias("-f")
            Return opt
        End Function
        Public Function GetFiles() As [Option](Of IEnumerable(Of FileInfo))
            Dim opt As [Option](Of IEnumerable(Of FileInfo)) = New [Option](Of IEnumerable(Of FileInfo))("--files")
            opt.Description = "A list of relative file or folder paths to include in formatting.All files are formatted if empty."
            opt.IsRequired = False
            opt.IsHidden = False
            opt.AddAlias("-include")
            Return opt
        End Function
        Public Function GetExclude() As [Option](Of IEnumerable(Of FileInfo))
            Dim opt As [Option](Of IEnumerable(Of FileInfo)) = New [Option](Of IEnumerable(Of FileInfo))("--exclude")
            opt.Description = "A list of relative file or folder paths to exclude from formatting."
            opt.IsRequired = False
            opt.IsHidden = False
            Return opt
        End Function
        Public Function GetCheck() As [Option](Of Boolean)
            Dim opt As [Option](Of Boolean) = New [Option](Of Boolean)("--check")
            opt.Description = "Formats files without saving changes to disk. Terminates with a non-zero exit code if any files were formatted."
            opt.IsRequired = False
            opt.IsHidden = False
            opt.AddAlias("-dry-run")
            Return opt
        End Function
        Public Function GetReport() As [Option](Of Boolean)
            Dim opt As [Option](Of Boolean) = New [Option](Of Boolean)("--report")
            opt.Description = "Accepts a file path, which if provided, will produce a json report in the given directory."
            opt.IsRequired = False
            opt.IsHidden = False
            Return opt
        End Function
        Public Function GetVerbosity() As [Option](Of VerbosityLevel)
            Dim opt As [Option](Of VerbosityLevel) = New [Option](Of VerbosityLevel)("--verbosity")
            opt.Description = "Set the verbosity level."
            opt.IsRequired = False
            opt.IsHidden = False
            opt.AddAlias("-v")
            Return opt
        End Function
    End Class
    Public Class InvokeCommandSource
        Inherits CommandSourceBase
        Public Sub New(root As RootCommandSource, parent As CommandSourceBase)
            MyBase.New(New Command("invoke", ""), parent)
            Command.Handler = CommandHandler.Create(Function() As Integer
                                                        root.CurrentCommandSource = Me
                                                        Return 0
                                                    End Function)
        End Sub


    End Class
End Namespace
