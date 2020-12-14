using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDom
{
    public class Method : MethodBaseWithReturn <Method>
    {

        public string Name { get; }
 
        public Method(string name, Scope scope = Scope.Public, MemberModifiers modifiers = MemberModifiers.None)
            : base(scope)
        {
            Name = name;
        }
    }
}
