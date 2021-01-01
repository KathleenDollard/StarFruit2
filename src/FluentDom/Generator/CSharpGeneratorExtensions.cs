using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDom.Generator
{
    public static class CSharpGeneratorExtensions
    {
        public static string CSharpString(this Scope scope)
        => scope switch
        {
            Scope.None => "",
            Scope.ProtectedInternal => "protected internal ",
            Scope.PrivateProtected => "private protected ",
            _ => scope.ToString().ToLower() + " "
        };

        public static string CSharpString(this ClassModifiers modifiers)
        {
            var s = "";
            if (modifiers.HasFlag(ClassModifiers.Partial))
            { s += "partial "; }
            if (modifiers.HasFlag(ClassModifiers.Static))
            { s += "static "; }
            if (modifiers.HasFlag(ClassModifiers.Abstract))
            { s += "abstract "; }
            if (modifiers.HasFlag(ClassModifiers.Sealed))
            { s += "sealed "; }
            return s;
        }

        public static string CSharpString(this MemberModifiers modifiers)
        {
            var s = "";
            if (modifiers.HasFlag(MemberModifiers.Partial))
            { s += "partial "; }
            if (modifiers.HasFlag(MemberModifiers.Static))
            { s += "static "; }
            if (modifiers.HasFlag(MemberModifiers.Abstract))
            { s += "abstract "; }
            if (modifiers.HasFlag(MemberModifiers.Sealed))
            { s += "sealed "; }
            if (modifiers.HasFlag(MemberModifiers.New))
            { s += "new "; }
            if (modifiers.HasFlag(MemberModifiers.Virtual))
            { s += "virtual "; }
            if (modifiers.HasFlag(MemberModifiers.Override))
            { s += "override "; }
            if (modifiers.HasFlag(MemberModifiers.Async))
            { s += "async "; }
            return s;
        }

        public static string CSharpString(this IExpression expression)
        => expression switch
        {
            Assign x => $"{x.LeftHand.CSharpString()} = {x.Expression.CSharpString()}",
            LocalDeclaration x => $"{x.TypeRep.CSharpString()} {x.LocalName} = {x.RightHand.CSharpString()}",
            MethodCall x => $"{x.Name}({x.ArgumentStore.CSharpString()})",
            NewObject x => $"new {x.TypeRep.CSharpString()}({x.ArgumentStore.CSharpString()})",
            This => "this",
            Base => "base",
            Value x => x.CSharpString(),
            Return x => x.CSharpString(),
            MultilineLambda x => x.CSharpString(),
            Null => "null",
            VariableReference x => x.ValueStore,                      // NOTE: This differs from Value because it does not have quotes for strings
            Dot x=> $"{x.Left.CSharpString()}.{x.Right}",             // NOTE: Maybe we should drop this
            As x=> $"({x.Expression.CSharpString()} as {x.TypeRep.CSharpString()})", // NOTE: This TryString in VB, although nullable value types are handled differently
            _ => throw new NotImplementedException($"Not implemented expression type in {nameof(CSharpString)}(IExpression) in {nameof(CSharpGeneratorExtensions)}"),
        };

        public static string CSharpString(this Return expression)
          => $"return {expression.ReturnExpression.CSharpString()}";

        public static string CSharpString(this Value expression)
            => expression.ValueStore switch
            {
                null => "null",
                bool x => x ? "true" : "false",
                string s => @$"""{s}""",
                _ => expression.ValueStore.ToString()!
            };


        public static string CSharpString(this TypeRep typeRep)
        {
            return typeRep is null
                       ? ""
                       : typeRep.GenericTypeArguments.Any()
                           ? $"{typeRep.Name}<{string.Join(", ", typeRep.GenericTypeArguments.Select(x => x.CSharpString()))}>"
                           : typeRep.Name.StartsWith("@")
                               ? typeRep.Name
                               : typeRep.Name switch
                               {
                                   "System.Boolean" => "bool",
                                   "System.Byte" => "byte",
                                   "System.SByte" => "sbyte",
                                   "System.Char" => "char",
                                   "System.Decimal" => "decimal",
                                   "System.Double" => "double",
                                   "System.Single" => "float",
                                   "System.Int32" => "int",
                                   "System.UInt32" => "uint",
                                   "System.Int64" => "long",
                                   "System.UInt64" => "ulong",
                                   "System.Int16" => "short",
                                   "System.UInt16" => "ushort",
                                   "System.Object" => "object",
                                   "System.String" => "string",
                                   _ => typeRep.Name
                               };
        }

        public static string CSharpString(this ParameterStore parameterStore)
        {
            return string.Join(", ",
                    parameterStore.Parameters.Select(x => $"{x.Type.CSharpString()} {x.Name}"));
        }

        public static string CSharpString(this IEnumerable<IExpression> argumentStore)
        {
            return string.Join(", ",
                    argumentStore.Select(x => x.CSharpString()));
        }

        public static string CSharpString(this MultilineLambda lambda)
        {
            // We do not have the generator here, so we just fake the spacing
            return $@"({lambda.ParameterStore.CSharpString()}) =>
             {{  
                {string.Join("\n                ", lambda.StatementStore.Select(x => x.CSharpString() + ";"))}
             }}";

        }



    }
}
