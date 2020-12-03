using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDom
{
    public class Method : MethodBaseWithReturn <Method>
    {

        public string Name { get; init; }
 
        public Method(string name, Scope scope = Scope.Public)
        {
            Name = name;
            Scope = scope;
        }
    }
}
