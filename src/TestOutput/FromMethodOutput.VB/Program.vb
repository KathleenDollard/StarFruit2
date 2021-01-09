Imports StarFruit2
Imports StarFruit2.Common
Imports System
Imports System.Collections.Generic
Imports System.IO

Namespace MethodEntryPoint
    Class Program
        Public Shared Function Main(args As String()) As Integer
            Console.WriteLine("Hello World!")
            Return CommandSource.Create(NameOf(Format)).Run(args)
        End Function
        ''' <summary>
        ''' Formats source code.
        ''' </summary>
        ''' <param name="folder">Folder to treat the `project` path as a folder of files.</param>
        ''' <param name="files">A list of relative file or folder paths to include in formatting.All files are formatted if empty.</param>
        ''' <param name="exclude">A list of relative file or folder paths to exclude from formatting.</param>
        ''' <param name="check">Formats files without saving changes to disk. Terminates with a non-zero exit code if any files were formatted.</param>
        ''' <param name="report">Accepts a file path, which if provided, will produce a json report in the given directory.</param>
        ''' <param name="verbosity">Set the verbosity level.</param>
        ''' <returns></returns>
        Public Shared Function Format(<Aliases("f")> folder As DirectoryInfo,
                           <Aliases("include")> files As IEnumerable(Of FileInfo),
                           exclude As IEnumerable(Of FileInfo),
                           <Aliases("dry-run")> check As Boolean,
                           report As Boolean,
                           verbosity As VerbosityLevel) As Integer
            Console.WriteLine($"
Folder: {folder} 
Files: {files} 
Exclue: {exclude} 
Check: {check} 
Report: {report} 
Verbosity: {verbosity} 
Version:  ")
            Return 0
        End Function
    End Class
End Namespace
