Imports StarFruit2
Imports System.CommandLine
Imports StarFruit2.Common
Imports System.CommandLine.Invocation
Imports System.CommandLine.Parsing
Imports System
Imports System.Collections.Generic
Imports System.IO

Namespace MethodEntryPoint
   Public Class FormatCommandSourceResult
      Inherits CommandSourceResult
      Public Sub New(parseResult As ParseResult, commandSource As FormatCommandSource, exitCode As Integer)
         MyBase.New(parseResult, TryCast(commandSource.ParentCommandSource, CommandSourceBase), exitCode)
            folderOption_Result = CommandSourceMemberResult.Create(commandSource.folderOption, parseResult)
            filesOption_Result = CommandSourceMemberResult.Create(commandSource.filesOption, parseResult)
            excludeOption_Result = CommandSourceMemberResult.Create(commandSource.excludeOption, parseResult)
            checkOption_Result = CommandSourceMemberResult.Create(commandSource.checkOption, parseResult)
            reportOption_Result = CommandSourceMemberResult.Create(commandSource.reportOption, parseResult)
            verbosityOption_Result = CommandSourceMemberResult.Create(commandSource.verbosityOption, parseResult)
      End Sub
      Public Property folderOption_Result As CommandSourceMemberResult(Of DirectoryInfo)
      Public Property filesOption_Result As CommandSourceMemberResult(Of IEnumerable(Of FileInfo))
      Public Property excludeOption_Result As CommandSourceMemberResult(Of IEnumerable(Of FileInfo))
      Public Property checkOption_Result As CommandSourceMemberResult(Of Boolean)
      Public Property reportOption_Result As CommandSourceMemberResult(Of Boolean)
      Public Property verbosityOption_Result As CommandSourceMemberResult(Of VerbosityLevel)

      Public Overrides   Function Run() As Integer
         Return Program.Format(folderOption_Result.Value, filesOption_Result.Value, excludeOption_Result.Value, checkOption_Result.Value, reportOption_Result.Value, verbosityOption_Result.Value)
   End Function
End Class
End Namespace
