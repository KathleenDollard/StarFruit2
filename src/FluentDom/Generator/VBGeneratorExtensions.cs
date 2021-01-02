using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentDom.Generator
{
    public static class VBGeneratorExtensions
    {
        public static string VBString(this Scope scope)
            => scope switch
            {
                Scope.None => "",
                Scope.ProtectedInternal => "Protected Friend ",
                Scope.PrivateProtected => "Private Protected ",
                _ => scope.ToString() + " "
            };

        public static string VBString(this ClassModifiers modifiers)
        {
            var s = "";
            if (modifiers.HasFlag(ClassModifiers.Partial))
            { s += "Partial "; }
            if (modifiers.HasFlag(ClassModifiers.Static))
            { s += "Shared "; }
            if (modifiers.HasFlag(ClassModifiers.Abstract))
            { s += "Abstract "; }
            if (modifiers.HasFlag(ClassModifiers.Sealed))
            { s += "NotInheritable  "; }
            return s;
        }

        public static string VBString(this MemberModifiers modifiers)
        {
            var s = "";
            if (modifiers.HasFlag(MemberModifiers.Partial))
            { s += "Partial "; }
            if (modifiers.HasFlag(MemberModifiers.Static))
            { s += "Shared "; }
            if (modifiers.HasFlag(MemberModifiers.Abstract))
            { s += "Abstract "; }
            if (modifiers.HasFlag(MemberModifiers.Sealed))
            { s += "NotOverridable  "; }
            if (modifiers.HasFlag(MemberModifiers.New))
            { s += "Shadows "; }
            if (modifiers.HasFlag(MemberModifiers.Virtual))
            { s += "Overridable  "; }
            if (modifiers.HasFlag(MemberModifiers.Override))
            { s += "Overrides  "; }
            if (modifiers.HasFlag(MemberModifiers.Async))
            { s += "Async "; }
            return s;
        }

        public static string VBString(this BlockType blockType)
             => blockType switch
             {
                 BlockType.Namespace => "Namespace",
                 BlockType.Class => "Class",
                 BlockType.Sub => "Sub",
                 BlockType.Constructor => "Sub",
                 BlockType.Function => "Function",
                 BlockType.Property => "Property",
                 BlockType.Getter => "Get",
                 BlockType.Setter => "Set",
                 _ => throw new NotImplementedException(),
             };

        public static string VBString(this IExpression expression)
        => expression switch
        {
            Assign x => $"{x.LeftHand.VBString()} = {x.Expression.VBString()}",
            LocalDeclaration x => $"Dim {x.LocalName} As {x.TypeRep.VBString()} = {x.RightHand.VBString()}",
            MethodCall x => $"{x.Name}({x.ArgumentStore.VBString()})",
            NewObject x => $"New {x.TypeRep.VBString()}({x.ArgumentStore.VBString()})",
            This => "Me",
            Base => "MyBase",
            Value x => x.VBString(),
            Return x => x.VBString(),
            MultilineLambda x => x.VBString(),
            Null => "Nothing",
            VariableReference x => x.ValueStore,                              // NOTE: This differs from Value because it does not have quotes for strings
            Dot x => $"{x.Left.VBString()}.{x.Right}",                        // NOTE: Maybe we should drop this
            As x => $"TryCast({x.Expression.VBString()}, {x.TypeRep.VBString()})", // NOTE: This TryString in VB, although nullable value types are handled differently
            _ => throw new NotImplementedException($"Not implemented expression type in {nameof(VBString)}(IExpression) in {nameof(VBGeneratorExtensions)}"),
        };

        public static string VBString(this Return expression)
          => $"Return {expression.ReturnExpression.VBString()}";

        public static string VBString(this Value expression)
            => expression.ValueStore switch
            {
                null => "Nothing",
                bool x => x ? "True" : "False",
                string s => @$"""{s}""",
                _ => expression.ValueStore.ToString()!
            };


        public static string VBString(this TypeRep typeRep)
        {
            return typeRep is null
                       ? ""
                       : typeRep.GenericTypeArguments.Any()
                           ? $"{typeRep.Name}(Of {string.Join(", ", typeRep.GenericTypeArguments.Select(x => x.VBString()))})"
                           : typeRep.Name.StartsWith("@")
                               ? typeRep.Name
                               : typeRep.Name switch
                               {
                                   "System.Boolean" => "Boolean",
                                   "System.Byte" => "Byte",
                                   "System.SByte" => "SByte",
                                   "System.Char" => "Char",
                                   "System.Decimal" => "Decimal",
                                   "System.Double" => "Double",
                                   "System.Single" => "Single",
                                   "System.Int32" => "Integer",
                                   "System.UInt32" => "UInteger",
                                   "System.Int64" => "Long",
                                   "System.UInt64" => "ULong",
                                   "System.Int16" => "Short",
                                   "System.UInt16" => "UShort",
                                   "System.Object" => "Object",
                                   "System.String" => "String",
                                   _ => typeRep.Name
                               };
        }

        public static string VBString(this ParameterStore parameterStore)
        {
            return string.Join(", ",
                    parameterStore.Parameters.Select(x => $"{x.Name} As {x.Type.VBString()}"));
        }

        public static string VBString(this IEnumerable<IExpression> argumentStore)
        {
            return string.Join(", ",
                    argumentStore.Select(x => x.VBString()));
        }

        public static string VBString(this MultilineLambda lambda)
        {
            // We do not have the generator here, so we just fake the spacing
            return $@"({lambda.ParameterStore.VBString()}) =>
             {{  
                {string.Join("\n                ", lambda.StatementStore.Select(x => x.VBString() + ";"))}
             }}";

        }
    }
}