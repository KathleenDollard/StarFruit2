using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GeneratorSupport.FluentDom
{

    public class Code
    {
        internal List<Using> usings = new List<Using>();
        internal List<Class> classes = new List<Class>();

        public static Code Create(string @namespace)
        {
            return new Code(@namespace);
        }

        public Code(string @namespace)
        {
            Namespace = @namespace;
        }

        public string Namespace { get; }
        public IEnumerable<Using> UsingsStore
            => usings;

        public Code Usings(params string[] usings)
        {
            this.usings.AddRange(usings.Select(u => Using.Create(u)));
            return this;
        }

        public Code Usings(params Using[] usings)
        {
            this.usings.AddRange(usings);
            return this;
        }

        public Code Usings(params object[] usings)
        {
            this.usings.AddRange(usings.Select(u => Using.Create(u)));
            return this;
        }

        public Class  Class(string name)
        {
            Class newClass = new Class(name);
            classes.Add(newClass);
            return newClass;
        }

        public Code Class(Class cls)
        {
            classes.Add(cls);
            return this;
        }

        public Code Classes<T>( IEnumerable<T> items , params Func<T, Class>[] classMakers)
        {
            classes.AddRange(items.SelectMany(
                            item => classMakers.Select(lambda => lambda(item))));        
            return this;
        }

        public Code x(string name)
        {
            return this;
        }
    }
}
