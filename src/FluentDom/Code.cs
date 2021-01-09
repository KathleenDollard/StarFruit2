using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentDom
{

    public class Code
    {
        internal List<Using> usings = new List<Using>();
        internal List<Class> classes = new List<Class>();

        public static Code Create(string? @namespace)
        {
            return new Code(@namespace);
        }

        public Code(string? @namespace)
        {
            Namespace = @namespace;
        }

        public string? Namespace { get; }
        public IEnumerable<Using> UsingStore
            => usings;
        public IEnumerable<Class> ClassStore
            => classes;

        public Code Usings(params Using[] usings)
        {
            this.usings.AddRange(usings.Where(x => x is not null));
            return this;
        }

        public Code Usings(IEnumerable<Using>? usings)
        {
            if (usings is not null)
            {
                this.usings.AddRange(usings.Where(x => x is not null));
            }
            return this;
        }

        public Code Usings(IEnumerable<string>? usings)
        {
            if (usings is not null)
            {
                this.usings.AddRange(usings.Where(x => x is not null).Select(x=>(Using)x));
            }
            return this;
        }

        public Code Class(Class cls)
        {
            classes.Add(cls);
            return this;
        }

        public Code Classes<T>(IEnumerable<T> items, params Func<T, Class>[] classMakers)
        {
            classes.AddRange(items.SelectMany(
                            item => classMakers.Select(lambda => lambda(item))));
            return this;
        }

        public Code Classes<T>(IEnumerable<T> items, params Func<T, IEnumerable<Class>>[] classMakers)
        {
            classes.AddRange(items.SelectMany(
                            item => classMakers.SelectMany(lambda => lambda(item))));
            return this;
        }
    }
}
