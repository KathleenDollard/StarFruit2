Imports StarFruit2
Imports System.CommandLine
Imports StarFruit2.Common
Imports System.CommandLine.Invocation
Imports System.CommandLine.Parsing
Imports System
Imports System.Collections.Generic
Imports System.IO

Namespace SingleLayerCli
   Public Class DotnetFormatCommandSourceResult
      Inherits CommandSourceResult(Of DotnetFormat)
      Public Sub New(parseResult As ParseResult, commandSource As DotnetFormatCommandSource, exitCode As Integer)
         MyBase.New(parseResult, TryCast(commandSource.ParentCommandSource, CommandSourceBase), exitCode)
            FolderOption_Result = CommandSourceMemberResult.Create(commandSource.FolderOption, parseResult)
            FilesOption_Result = CommandSourceMemberResult.Create(commandSource.FilesOption, parseResult)
            ExcludeOption_Result = CommandSourceMemberResult.Create(commandSource.ExcludeOption, parseResult)
            CheckOption_Result = CommandSourceMemberResult.Create(commandSource.CheckOption, parseResult)
            ReportOption_Result = CommandSourceMemberResult.Create(commandSource.ReportOption, parseResult)
            VerbosityOption_Result = CommandSourceMemberResult.Create(commandSource.VerbosityOption, parseResult)
      End Sub
      Public Property FolderOption_Result As CommandSourceMemberResult(Of DirectoryInfo)
      Public Property FilesOption_Result As CommandSourceMemberResult(Of IEnumerable(Of FileInfo))
      Public Property ExcludeOption_Result As CommandSourceMemberResult(Of IEnumerable(Of FileInfo))
      Public Property CheckOption_Result As CommandSourceMemberResult(Of Boolean)
      Public Property ReportOption_Result As CommandSourceMemberResult(Of Boolean)
      Public Property VerbosityOption_Result As CommandSourceMemberResult(Of VerbosityLevel)

      Public Overrides   Function CreateInstance() As DotnetFormat
         Dim newItem = New DotnetFormat()
         newItem.Folder = FolderOption_Result.Value
         newItem.Files = FilesOption_Result.Value
         newItem.Exclude = ExcludeOption_Result.Value
         newItem.Check = CheckOption_Result.Value
         newItem.Report = ReportOption_Result.Value
         newItem.Verbosity = VerbosityOption_Result.Value
         Return newItem
   End Function
End Class
Public Class InvokeCommandSourceResult
   Inherits DotnetFormatCommandSourceResult
   Public Sub New(parseResult As ParseResult, commandSource As InvokeCommandSource, exitCode As Integer)
      MyBase.New(parseResult, TryCast(commandSource.ParentCommandSource, DotnetFormatCommandSource), exitCode)
   End Sub

End Class
End Namespace
