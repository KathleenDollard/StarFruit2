Imports StarFruit2
Imports System
Imports System.Threading.Tasks

Namespace TwoLayerCli
    NotInheritable Class Program
        Public Shared Function Main(args As String()) As Integer
            'var x = TwoLayerCli.
            RepeatMain(args)
            Return 0
            'Main_Simplest(args);
            'Main_with_CliModifications(args);
            ''' Main_with_direct_execution(args);
            'return await Main_what_really_happens(args);
        End Function
        Private Shared Sub RepeatMain(args As String())
            Dim input As String = String.Join("", args)
            While input IsNot Nothing
                CommandSource.Run(Of CliRoot)(input)
                Console.WriteLine("Enter something:")
                input = Console.ReadLine()
            End While
        End Sub
    End Class
End Namespace
