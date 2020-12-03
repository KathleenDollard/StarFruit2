using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FluentDom
{
    public class Parameter 
    {
        public  Parameter(string name, TypeRep type)
        {
            Type = type;
            Name = name;
        }

        public TypeRep Type { get; }
        public string Name { get; }
    }
}
