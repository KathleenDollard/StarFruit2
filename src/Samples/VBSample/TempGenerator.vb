'Imports Microsoft.CodeAnalysis
'Imports Starfruit2
'Imports System
'Imports System.Collections.Generic
'Imports System.Linq
'Imports FluentDom
'Imports FluentDom.Generator

'Namespace StarFruit2.Generate
'    <Generator>
'    Public Class Generator
'        Implements ISourceGenerator

'        Public Sub Initialize(context As GeneratorInitializationContext) Implements ISourceGenerator.Initialize

'            context.RegisterForSyntaxNotifications(Function() New CommandSourceSyntaxReceiver())
'        End Sub

'        Public Sub Execute(context As GeneratorExecutionContext) Implements ISourceGenerator.Execute

'            Try

'                context.ReportDiagnostic(Diagnostic.Create(New DiagnosticDescriptor("KD0001", "Generator entered", "", "KDGenerator", DiagnosticSeverity.Info, True), _
'                    Nothing))

'                If (context.SyntaxReceiver Is Not CommandSourceSyntaxReceiver) Then
'                    Return
'                End If
'                Dim receiver = CType(context.SyntaxReceiver, CommandSourceSyntaxReceiver)
'                If (receiver Is Nothing) Then
'                    Return
'                End If

'                Dim semanticModels As Dictionary(Of ISymbol, SemanticModel) = New Dictionary(Of ISymbol, SemanticModel)()
'                ' semanticModels updated in GetSymbol
'                Dim symbols = receiver.Candidates _
'                                      .Select(Function(x) RoslynCSharpDescriptorFactory.GetSymbol(x, context.Compilation, semanticModels)) _
'                                      .Where(Function(symbol) symbol Is Not Nothing) _
'                                      .Distinct()

'                Dim cliDescriptors = symbols.Select(Function(symbol) RoslynCSharpDescriptorFactory.GetCliDescriptor(symbol!, semanticModels(symbol!))) _
'                                            .ToList()

'                For Each cliDescriptor In cliDescriptors

'                    OutputCode(New GenerateCommandSource().CreateCode(cliDescriptor, receiver.Usings), _
'                           context, _
'                           $"cliDescriptor.CommandDescriptor.OriginalNameEndCommandSource.generated.cs")
'                    OutputCode(New GenerateCommandSourceResult().CreateCode(cliDescriptor, receiver.Usings), _
'                           context, _
'                           $"cliDescriptor.CommandDescriptor.OriginalNameEndCommandSourceResult.generated.cs")
'                Next
'            Catch ex As Exception

'                context.ReportDiagnostic(Diagnostic.Create(
'                    New DiagnosticDescriptor("KD0002", "Generator failed", ex.ToString(), "KDGenerator", DiagnosticSeverity.Error, True),
'                    Nothing))
'            End Try



'        End Sub
'        Static Sub OutputCode(Code code, GeneratorExecutionContext context, String fileName)

'            Dim source = New CSharpGenerator().Generate(Code)
'            If source Is Not Nothing Then

'                context.AddSource(fileName, source)
'            End If
'        End Sub
'    End Class
'End Namespace

