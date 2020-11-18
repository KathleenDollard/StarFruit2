using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorSupport.FluentDom
{
    public abstract class MemberBase : IClassMember 
    {
        public Scope Scope { get; init; }
        public bool IsStatic { get; init; }
    }
}
