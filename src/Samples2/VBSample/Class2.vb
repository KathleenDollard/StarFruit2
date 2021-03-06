﻿Imports StarFruit2
Imports System.CommandLine
Imports StarFruit2.Common
Imports System.CommandLine.Invocation
Imports System.CommandLine.Parsing
Imports System.Threading.Tasks
Imports System.IO

Namespace Tests
    Public Class CliRootCommandSourceResult
        Inherits CommandSourceResult(Of CliRoot)
        Public Sub New(parseResult As ParseResult, commandSource As CliRootCommandSource, exitCode As Integer)
            MyBase.New(parseResult, TryCast(commandSource.ParentCommandSource, CommandSourceBase), exitCode)
            StringPropertyOption_Result = CommandSourceMemberResult.Create(commandSource.StringPropertyOption, parseResult)
            ctorParamOption_Result = CommandSourceMemberResult.Create(commandSource.ctorParamOption, parseResult)
        End Sub
        Public Property StringPropertyOption_Result As CommandSourceMemberResult(Of String)
        Public Property ctorParamOption_Result As CommandSourceMemberResult(Of Boolean)

        Public Overrides Function CreateInstance() As CliRoot
            Dim newItem As var = New CliRoot(ctorParamOption_Result.Value)
            newItem.StringProperty = StringPropertyOption_Result.Value
            Return newItem
        End Function
    End Class
    Public Class InvokeAsyncCommandSourceResult
        Inherits CliRootCommandSourceResult
        Public Sub New(parseResult As ParseResult, commandSource As InvokeAsyncCommandSource, exitCode As Integer)
            MyBase.New(parseResult, TryCast(commandSource.ParentCommandSource, CliRootCommandSource), exitCode)
            intArgArgument_Result = CommandSourceMemberResult.Create(commandSource.intArgArgument, parseResult)
            stringOptionOption_Result = CommandSourceMemberResult.Create(commandSource.stringOptionOption, parseResult)
            boolOptionOption_Result = CommandSourceMemberResult.Create(commandSource.boolOptionOption, parseResult)
        End Sub
        Public Property intArgArgument_Result As CommandSourceMemberResult(Of Integer)
        Public Property stringOptionOption_Result As CommandSourceMemberResult(Of String)
        Public Property boolOptionOption_Result As CommandSourceMemberResult(Of Boolean)

    End Class
End Namespace
