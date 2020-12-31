using System;
using System.Collections.Generic;
using System.Text;

namespace StarFruit.Common
{
    public enum CodeElementType
    {
        Unknown,
        Null,
        Property,
        MethodParameter,
        CtorParameter,
        Type,
        Method,
    }

    public abstract class RawInfoBase
    {
        protected RawInfoBase(object? raw)
           => Raw = raw;

        public object? Raw { get; set; }
    }

    public class RawInfoForNull : RawInfoBase
    {
        public RawInfoForNull()
            : base(null)
        { }
    }

    public class RawInfoForUnknown : RawInfoBase
    {
        public RawInfoForUnknown()
            : base(null)
        { }
    }

    public abstract class RawInfoForCode : RawInfoBase
    {
        protected RawInfoForCode(object? raw,
                                 CodeElementType codeElementType,
                                 string namespaceName,
                                 string? containingTypeName)
            : base(raw)
        {
            CodeElementType = codeElementType;
            NamespaceName = namespaceName;
            ContainingTypeName = containingTypeName;
        }

        public CodeElementType CodeElementType { get; }
        public string NamespaceName { get; }
        public string? ContainingTypeName { get; }
    }

    public class RawInfoForType : RawInfoForCode
    {
        public RawInfoForType(object? raw,
                              string namespaceName,
                              string? containingTypeName)
            : base(raw, CodeElementType.Type, namespaceName, containingTypeName)
        { }
    }

    public class RawInfoForProperty : RawInfoForCode
    {
        public RawInfoForProperty(object? raw,
                                  string namespaceName,
                                  string containingTypeName)
             : base(raw, CodeElementType.Property, namespaceName, containingTypeName)
        { }
    }

    public class RawInfoForMethod : RawInfoForCode
    {
        public RawInfoForMethod(object? raw,
                                bool isStatic,
                                string namespaceName,
                                string containingTypeName)
             : base(raw, CodeElementType.Method, namespaceName, containingTypeName)
        {
            IsStatic = isStatic;
        }

        public bool IsStatic { get; }
    }

    public class RawInfoForCtor : RawInfoForCode
    {
        public RawInfoForCtor(object? raw,
                              bool isStatic,
                              string namespaceName,
                              string containingTypeName)
             : base(raw, CodeElementType.Method, namespaceName, containingTypeName)
        {
            IsStatic = isStatic;
        }

        public bool IsStatic { get; }
    }

    public abstract class RawInfoForParameter : RawInfoForCode
    {
        protected RawInfoForParameter(object? raw,
                                      CodeElementType codeElementType,
                                      string namespaceName,
                                      string containingTypeName)
             : base(raw, codeElementType, namespaceName, containingTypeName)
        { }
    }

    public class RawInfoForCtorParameter : RawInfoForParameter
    {
        public RawInfoForCtorParameter(object? raw,
                                       string namespaceName,
                                       string containingTypeName)
           : base(raw, CodeElementType.CtorParameter, namespaceName, containingTypeName)
        { }
    }

    public class RawInfoForMethodParameter : RawInfoForParameter
    {
        public RawInfoForMethodParameter(object? raw,
                                         string namespaceName,
                                         string containingTypeName,
                                         string methodName)
             : base(raw, CodeElementType.MethodParameter, namespaceName, containingTypeName)
        {
            MethodName = methodName;
        }

        public string MethodName { get; }

    }
}
