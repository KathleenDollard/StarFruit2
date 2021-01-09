Imports StarFruit2
Imports System.CommandLine
Imports StarFruit2.Common
Imports System.CommandLine.Invocation
Imports System.CommandLine.Parsing
Imports System

Namespace TwoLayerCli
   Public Class CliRootCommandSource
      Inherits RootCommandSource(Of CliRootCommandSource)
      Public Sub New()
         Me.New(Nothing, Nothing)
      End Sub
      Public Sub New(root As RootCommandSource, parent As CommandSourceBase)
         MyBase.New(New Command("cli-root", "This is the entry point, the end user types the executable name"), parent)
            StringPropertyOption = GetStringProperty()
            Command.Add(StringPropertyOption)
            ctorParamOption = GetctorParam()
            Command.Add(ctorParamOption)
            FindCommand = New FindCommandSource(Me, Me)
            Command.AddCommand(FindCommand.Command)
            ListCommand = New ListCommandSource(Me, Me)
            Command.AddCommand(ListCommand.Command)
            Command.Handler = CommandHandler.Create(Function (context As InvocationContext) As Integer
                CurrentCommandSource = Me
                CurrentParseResult = context.ParseResult
                Return 0
                   End Function)
      End Sub

      Public Property StringPropertyOption As [Option](Of String)
      Public Property ctorParamOption As [Option](Of Boolean)
      Public Property FindCommand As FindCommandSource
      Public Property ListCommand As ListCommandSource

      Public  Function GetStringProperty() As [Option](Of String)
         Dim opt As [Option](Of String) = New [Option](Of String)("--string-property")
         opt.Description = "This is a string property"
         opt.IsRequired = False
         opt.IsHidden = False
         Dim optionArg = opt.Argument
         optionArg.SetDefaultValue("abc")
         opt.AddAlias("-o")
         Return opt
   End Function
   Public  Function GetctorParam() As [Option](Of Boolean)
      Dim opt As [Option](Of Boolean) = New [Option](Of Boolean)("--ctor-param")
      opt.Description = "This is a constructor parameter"
      opt.IsRequired = False
      opt.IsHidden = False
      Return opt
End Function
End Class
Public Class FindCommandSource
   Inherits CommandSourceBase
   Public Sub New(root As RootCommandSource, parent As CommandSourceBase)
      MyBase.New(New Command("find", "Use this to find things"), parent)
         intArgArgument = GetintArg()
         Command.Add(intArgArgument)
         stringOptionOption = GetstringOption()
         Command.Add(stringOptionOption)
         boolOptionOption = GetboolOption()
         Command.Add(boolOptionOption)
         Command.Handler = CommandHandler.Create(Function () As Integer
                root.CurrentCommandSource = Me
                Return 0
                   End Function)
   End Sub

   Public Property intArgArgument As Argument(Of Integer)
   Public Property stringOptionOption As [Option](Of String)
   Public Property boolOptionOption As [Option](Of Boolean)

   Public  Function GetintArg() As Argument(Of Integer)
      Dim argument As Argument(Of Integer) = New Argument(Of Integer)("int")
      argument.Description = "This is an integer argument"
      argument.IsHidden = False
      argument.SetDefaultValue(42)
      Return argument
End Function
Public  Function GetstringOption() As [Option](Of String)
   Dim opt As [Option](Of String) = New [Option](Of String)("--string")
   opt.Description = "This is an string argument"
   opt.IsRequired = False
   opt.IsHidden = False
   Return opt
End Function
Public  Function GetboolOption() As [Option](Of Boolean)
   Dim opt As [Option](Of Boolean) = New [Option](Of Boolean)("--bool")
   opt.Description = "This is an bool argument"
   opt.IsRequired = False
   opt.IsHidden = False
   Return opt
End Function
End Class
Public Class ListCommandSource
   Inherits CommandSourceBase
   Public Sub New(root As RootCommandSource, parent As CommandSourceBase)
      MyBase.New(New Command("list", "List the elements you are interested in"), parent)
         verbosityOption = Getverbosity()
         Command.Add(verbosityOption)
         Command.Handler = CommandHandler.Create(Function () As Integer
                root.CurrentCommandSource = Me
                Return 0
                   End Function)
   End Sub

   Public Property verbosityOption As [Option](Of VerbosityLevel)

   Public  Function Getverbosity() As [Option](Of VerbosityLevel)
      Dim opt As [Option](Of VerbosityLevel) = New [Option](Of VerbosityLevel)("--verbosity")
      opt.Description = "The degree of detail desired"
      opt.IsRequired = False
      opt.IsHidden = False
      Return opt
End Function
End Class
End Namespace
