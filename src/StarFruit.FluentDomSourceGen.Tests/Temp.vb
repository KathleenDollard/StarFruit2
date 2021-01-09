Option Compare Text
Option Explicit On
Option Infer On
Option Strict On
Imports StarFruit2
Imports StarFruit2.Common
Imports System
Imports System.Collections.Generic
Imports System.IO

Namespace SingleLayerCli
    Public Class DotnetFormat
        Implements  ICliRoot
        ''' <summary>
        ''' Folder Whether to treat the &lt;project&gt; path as a folder of files.
        ''' </summary>
        <Aliases("f")>
        Public Property Folder As DirectoryInfo

        ''' <summary>
        ''' A list of relative file or folder paths to include in formatting.All files are formatted if empty.
        ''' </summary>
        <Aliases("include")>
        Public Property Files As IEnumerable(Of FileInfo)


        ''' <summary>
        ''' A list of relative file or folder paths to exclude from formatting.
        ''' </summary>
        Public Property Exclude As IEnumerable(Of FileInfo)

        ''' <summary>
        ''' Formats files without saving changes to disk. Terminates with a non-zero exit code if any files were formatted.
        ''' </summary>
        <Aliases("dry-run")>
        Public Property Check As Boolean


        ''' <summary>
        ''' Accepts a file path, which if provided, will produce a json report in the given directory.
        ''' </summary>
        Public Property Report As Boolean


        ''' <summary>
        ''' Set the verbosity level.
        ''' </summary>
        <Aliases("v")>
        Public Property Verbosity As VerbosityLevel
        Public Function Invoke() As Integer
            Console.WriteLine($"Folder: {Folder} 
Files: {Files} 
Exclue: {Exclude} 
Check: {Check} 
Report: {Report} 
Verbosity: {Verbosity} 
Version:  ")
            Return 0
        End Function
    End Class
End Namespace
