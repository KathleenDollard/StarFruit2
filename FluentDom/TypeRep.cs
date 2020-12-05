using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDom
{
    public class TypeRep 
    {
        public static implicit operator TypeRep(string b)
            => new(b);

        public static TypeRep Var { get; } = new TypeRep(true) { };

        public static TypeRep Bool { get; } = new TypeRep(typeof(System.Boolean)) { };
        public static TypeRep Byte { get; } = new TypeRep(typeof(System.Byte)) { };
        public static TypeRep Sbyte { get; } = new TypeRep(typeof(System.SByte)) { };
        public static TypeRep Char { get; } = new TypeRep(typeof(System.Char)) { };
        public static TypeRep Decimal { get; } = new TypeRep(typeof(System.Decimal)) { };
        public static TypeRep Double { get; } = new TypeRep(typeof(System.Double)) { };
        public static TypeRep Float { get; } = new TypeRep(typeof(System.Single)) { };
        public static TypeRep Int { get; } = new TypeRep(typeof(System.Int32)) { };
        public static TypeRep Uint { get; } = new TypeRep(typeof(System.UInt32)) { };
        public static TypeRep Long { get; } = new TypeRep(typeof(System.Int64)) { };
        public static TypeRep Ulong { get; } = new TypeRep(typeof(System.UInt64)) { };
        public static TypeRep Short { get; } = new TypeRep(typeof(System.Int16)) { };
        public static TypeRep Ushort { get; } = new TypeRep(typeof(System.UInt16)) { };
        public static TypeRep Object { get; } = new TypeRep(typeof(System.Object)) { };
        public static TypeRep String { get; } = new TypeRep(typeof(System.String)) { };
        public static TypeRep Dynamic { get; } = new TypeRep(typeof(System.Object)) { };

        public TypeRep(string name)
        {
            Name = name switch
            {
                "bool" or "Boolean" => "System.Boolean",
                "byte" or "Byte" => "System.Byte",
                "sbyte" or "SByte" => "System.SByte",
                "char" or "Char" => "System.Char",
                "decimal" or "Decimal" => "System.Decimal",
                "double" or "Double" => "System.Double",
                "float" or "Single" => "System.Single",
                "int" or "Int32" => "System.Int32",
                "uint" or "UInt32" => "System.UInt32",
                "long" or "Int64" => "System.Int64",
                "ulong" or "UInt64" => "System.UInt64",
                "short" or "Int16" => "System.Int16",
                "ushort" or "UInt16" => "System.UInt16",
                "object" or "Object" => "System.Object",
                "string" or "String" => "System.String",
                "dynamic" or "Object" => "System.Object",
                _ => name
            };
        }
        public TypeRep(string name, params TypeRep[] genericTypeArguments)
            : this(name)
        {
            GenericTypeArguments.AddRange(genericTypeArguments);
        }
        public TypeRep(string name, params string[] genericTypeArguments)
              : this(name)
        {
            GenericTypeArguments.AddRange(genericTypeArguments.Select(x => new TypeRep(x)));
        }

        public TypeRep(Type type)
            : this(RemoveArityMark(type.Name))
        {
            GenericTypeArguments.AddRange(type.GenericTypeArguments.Select(x => new TypeRep(x)));
        }

        private static string RemoveArityMark(string name)
        {
            var pos = name.IndexOf("`");
            return pos > 0 
                ? name.Substring(0, pos)
                : name;
        }

        public string Name { get;  }
        public bool IsImplicit { get; set; }

        private TypeRep(bool isImplicit)
        {
            IsImplicit = isImplicit;
            Name = "";
        }

        public List<TypeRep> GenericTypeArguments { get; } = new List<TypeRep>();


    }
}
