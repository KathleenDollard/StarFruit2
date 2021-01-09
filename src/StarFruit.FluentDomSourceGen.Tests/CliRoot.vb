' These using choices are deliberate.
' * StarFruit2.Common is redundant with default, 
' * System.Threading.Tasks may or may not be in the defualt generation,
' * System.IO is not in the generation

Option Compare Text
Option Explicit On
Option Infer On
Option Strict On

Imports StarFruit2
Imports StarFruit2.Common
Imports System

Namespace TwoLayerCli

    ''' <summary>
    ''' This is the entry point, the end user types the executable name
    ''' </summary>
    Partial Public Class CliRoot
        Implements ICliRoot

        ''' <summary> </summary>                    
        ''' <param name="ctorParam">This is a constructor parameter</param>
        Public Sub New(ctorParam As Boolean)
        End Sub

        ''' <summary>
        ''' This is a string property
        ''' </summary>
        <Aliases("o")>
        <Required>
        Public Property StringProperty As String = "abc"

        ''' <summary>
        ''' Use this to find things
        ''' </summary>
        ''' <param name="stringOption">This is an string argument</param>
        ''' <param name="boolOption">This is an bool argument</param>
        ''' <param name="intArg">This is an integer argument</param>
        ''' <returns></returns>
        Public Function Find(stringOption As String, boolOption As Boolean, Optional intArg As Integer = 42) As Integer
            Console.WriteLine($"StringProperty: {StringProperty}   stringOption: {stringOption}   boolOption: {boolOption}    intArg: {intArg}")
            Return 0
        End Function

        ''' <summary>
        ''' List the elements you are interested in
        ''' </summary>
        ''' <param name="verbosity">The degree of detail desired</param>
        ''' <returns></returns>
        Public Function List(verbosity As VerbosityLevel) As Integer
            Console.WriteLine($"Verbosity: {verbosity}")
            Return 0
        End Function
    End Class
End Namespace
