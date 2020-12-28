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

    public  class RawInfoForNull : RawInfoBase 
    {
        public RawInfoForNull()
            : base(null)
        { }
    }

    public  class RawInfoForUnknown : RawInfoBase
    {
        public RawInfoForUnknown()
            : base(null)
        { }
    }

    public abstract class RawInfoForCode : RawInfoBase
    {
        protected RawInfoForCode(object? raw,CodeElementType codeElementType)
            : base(raw)
            => CodeElementType = codeElementType;

        public CodeElementType CodeElementType { get; }

    }

    public class RawInfoForType : RawInfoForCode
    {
        public RawInfoForType(object? raw)
            : base(raw, CodeElementType.Type)
        { }
    }

    public class RawInfoForProperty : RawInfoForCode
    {
        public RawInfoForProperty(object? raw)
             : base(raw, CodeElementType.Property)
        { }
    }

    public class RawInfoForMethod : RawInfoForCode
    {
        public RawInfoForMethod(object? raw,bool isStatic)
             : base(raw, CodeElementType.Method)
        {
            IsStatic = isStatic;
        }

        public bool IsStatic { get; }
    }

    public class RawInfoForCtor : RawInfoForCode
    {
        public RawInfoForCtor(object? raw, bool isStatic)
             : base(raw, CodeElementType.Method)
        {
            IsStatic = isStatic;
        }

        public bool IsStatic { get; }
    }

    public abstract class RawInfoForParameter : RawInfoForCode
    {
        protected RawInfoForParameter(object? raw,CodeElementType codeElementType)
             : base(raw, codeElementType)
        { }
    }

    public class RawInfoForCtorParameter : RawInfoForCode
    {
        public RawInfoForCtorParameter(object? raw)
           : base(raw, CodeElementType.CtorParameter )
        { }
    }

    public class RawInfoForMethodParameter : RawInfoForCode
    {
        public RawInfoForMethodParameter(object? raw)
             : base(raw, CodeElementType.MethodParameter )
        { }
    }
}
