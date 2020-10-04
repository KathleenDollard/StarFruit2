//using Microsoft.CodeAnalysis.CSharp.Syntax;
//using StarFruit2.Common.Descriptors;
//using StarFruit2.Common;
//using System;
//using System.Linq;
//using System.CommandLine.Parsing;
//using System.Collections.Generic;

//namespace StarFruit2
//{
//    public class SyntaxTreeDescriptorMaker
//        : DescriptorMakerBase<EmptyClass, SyntaxTreeSource>
//    {

//        public override CommandDescriptor GetCommandDescriptor<T>(T commandSource)
//        => commandSource switch
//        {
//            ClassDeclarationSyntax classDeclaration => GetCommandDescriptor(null, classDeclaration),
//            MethodDeclarationSyntax methodDeclaration => GetCommandDescriptor(null, methodDeclaration),
//            _ => throw new ArgumentException("The command source is an unexpected type", nameof(commandSource))
//        };

//        public override OptionDescriptor GetOptionDescriptor<T>(ISymbolDescriptor? parent, T optionSource)
//        => optionSource switch
//        {
//            ParameterSyntax parameterDeclaration => GetOptionDescriptor(parent, parameterDeclaration),
//            PropertyDeclarationSyntax propertyDeclaration => GetOptionDescriptor(parent, propertyDeclaration),
//            _ => throw new ArgumentException("The option source is an unexpected type", nameof(optionSource))
//        };

//        public override ArgumentDescriptor GetArgumentDescriptor<T>(ISymbolDescriptor? parent, T argumentSource)
//         => argumentSource switch
//         {
//             ParameterSyntax parameterDeclaration => GetArgumentDescriptor(parent, parameterDeclaration),
//             PropertyDeclarationSyntax propertyDeclaration => GetArgumentDescriptor(parent, propertyDeclaration),
//             _ => throw new ArgumentException("The option source is an unexpected type", nameof(argumentSource))
//         };

//        public override bool IsArgument<T>(T argumentSource)
//        => argumentSource switch
//        {
//            ParameterSyntax parameterDeclaration => IsArgument(parameterDeclaration),
//            PropertyDeclarationSyntax propertyDeclaration => IsArgument(propertyDeclaration),
//            _ => throw new ArgumentException("The option source is an unexpected type", nameof(argumentSource))
//        };

//        public override bool IsOption<T>(T argumentSource)
//            => !IsArgument(argumentSource);

//        public override IEnumerable<CommandDescriptor> GetSubCommandSources<T>(ISymbolDescriptor? parent, T commandSource)
//        => commandSource switch
//        {
//            //ClassDeclarationSyntax classDeclaration => GetSubCommandDescriptors(null, classDeclaration),
//            _ => throw new ArgumentException("The command source is an unexpected type", nameof(commandSource))
//        };

//        public override string SourceToOptionName(string sourceName)
//            => $"--{sourceName.ToKebabCase()}";

//        public override string SourceToArgumentName(string sourceName)
//        {
//            if (sourceName.EndsWith("Arg"))
//            {
//                sourceName = sourceName[..^3];
//            }
//            return sourceName.ToKebabCase();
//        }

//        public override string SourceToCommandName(string sourceName)
//            => sourceName.ToKebabCase();







//        private ArgumentDescriptor GetArgumentDescriptor(ISymbolDescriptor? parent, PropertyDeclarationSyntax propertyDeclaration)
//        => new ArgumentDescriptor(new ArgTypeInfo(propertyDeclaration.Type), parent, propertyDeclaration.Identifier.ToString(), propertyDeclaration)
//        {
//            Name = SourceToArgumentName(propertyDeclaration.Identifier.ToString())
//        };

//        private ArgumentDescriptor GetArgumentDescriptor(ISymbolDescriptor? parent, ParameterSyntax parameterDeclaration)
//        => new ArgumentDescriptor(new ArgTypeInfo(parameterDeclaration.Type), parent, parameterDeclaration.Identifier.ToString(), parameterDeclaration)
//        {
//            Name = SourceToArgumentName(parameterDeclaration.Identifier.ToString())
//        };

//        private ArgumentDescriptor GetOptionArgumentDescriptor(ISymbolDescriptor? parent, PropertyDeclarationSyntax propertyDeclaration)
//         => new ArgumentDescriptor(new ArgTypeInfo(propertyDeclaration.Type), parent, propertyDeclaration.Identifier.ToString(), propertyDeclaration)
//         {
//             Name = SourceToArgumentName(propertyDeclaration.Identifier.ToString())
//         };

//        private ArgumentDescriptor GetOptionArgumentDescriptor(ISymbolDescriptor? parent, ParameterSyntax parameterDeclaration)
//          => new ArgumentDescriptor(new ArgTypeInfo(parameterDeclaration.Type), parent, parameterDeclaration.Identifier.ToString(), parameterDeclaration)
//          {
//              Name = SourceToArgumentName(parameterDeclaration.Identifier.ToString())
//          };

//        private OptionDescriptor GetOptionDescriptor(ISymbolDescriptor? parent, PropertyDeclarationSyntax propertyDeclaration)
//        {
//            var option = new OptionDescriptor(parent, propertyDeclaration.Identifier.ToString(), propertyDeclaration)
//            {
//                Name = SourceToOptionName(propertyDeclaration.Identifier.ToString())
//            };
//            option.Arguments.Add(GetOptionArgumentDescriptor(parent, propertyDeclaration));
//            return option;
//        }


//        private OptionDescriptor GetOptionDescriptor(ISymbolDescriptor? parent, ParameterSyntax parameterDeclaration)
//        {
//            var option = new OptionDescriptor(parent, parameterDeclaration.Identifier.ToString(), parameterDeclaration)
//            {
//                Name = SourceToOptionName(parameterDeclaration.Identifier.ToString())
//            };
//            option.Arguments.Add(GetOptionArgumentDescriptor(parent, parameterDeclaration));
//            return option;
//        }

//        private CommandDescriptor GetCommandDescriptor(ISymbolDescriptor? parent, MethodDeclarationSyntax methodDeclaration)
//        {
//            var command = new CommandDescriptor(parent, methodDeclaration.Identifier.ToString(), methodDeclaration)
//            {
//                Name = SourceToCommandName(methodDeclaration.Identifier.ToString()),
//            };
//            command.AddArguments(methodDeclaration.ParameterList.Parameters.Select(p => GetArgumentDescriptor(parent, p)));
//            command.AddOptions(methodDeclaration.ParameterList.Parameters.Select(p => GetOptionDescriptor(parent, p)));
//            return command;
//        }

//        private CommandDescriptor GetCommandDescriptor(ISymbolDescriptor? parent, ClassDeclarationSyntax classDeclaration)
//        {
//            var command = new CommandDescriptor(parent, classDeclaration.Identifier.ToString(), classDeclaration)
//            {
//                Name = SourceToCommandName(classDeclaration.Identifier.ToString()),
//                // SubCommands = GetSubCommandDescriptorSet(classDeclaration)
//            };
//            command.AddArguments(classDeclaration.ChildNodes()
//                                  .OfType<PropertyDeclarationSyntax>()
//                                  .Where(p => IsArgument(p))
//                                  .Select(p => GetArgumentDescriptor(parent, p)));
//            command.AddOptions(classDeclaration.ChildNodes()
//                                       .OfType<PropertyDeclarationSyntax>()
//                                       .Where(p => !IsArgument(p))
//                                       .Select(p => GetOptionDescriptor(parent, p)));
//            return command;
//        }

//        private bool IsArgument(PropertyDeclarationSyntax property)
//               => property.Identifier.ToString().EndsWith("Arg")
//                   || property.HasAttribute(typeof(ArgumentAttribute));

//    }
//}
