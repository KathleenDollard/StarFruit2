using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorSupport.FluentDom
{
    public class Value : Expression
    {
        public Value(string value)
        {
            ValueStore = value;
        }

        public string ValueStore { get; }
    }
}
