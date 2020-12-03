using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDom
{
    public enum MemberModifiers
    {
        Static = 0b_0000_0001,
        Partial = 0b_0000_0010,
        Sealed = 0b_0000_0100,
        Abstract = 0b_0000_1000,
        New = 0b_0001_0000,
        Virtual = 0b_0001_0000,
        Override = 0b_0001_0000,
        Async = 0b_0001_0000,
    }

    public abstract class MemberBase : IClassMember 
    {
        public Scope Scope { get; init; }
        public MemberModifiers Modifiers { get; init; }
    }
}
