using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDom
{
    public class BlankLine : MemberBase
    {
        public BlankLine() 
            : base(Scope.Public, MemberModifiers.None)
        {
        }
    }
}
