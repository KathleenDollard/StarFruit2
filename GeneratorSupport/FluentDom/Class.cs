using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneratorSupport.FluentDom
{
    public class Class
    {
        public Class(string name)
        {
            Name = name;
        }

        public string Name { get; }
        public TypeRep? BaseStore { get; private set; }
        internal List<IClassMember> members = new List<IClassMember>();
        internal List<TypeRep> interfaces = new List<TypeRep>();
        internal List<IConstructor> constructors = new List<IConstructor>();

        public IEnumerable<IClassMember> MemberStore
            => members;

        public IEnumerable<IConstructor> ContructorStore
              => constructors;

        public IEnumerable<TypeRep> InterfaceStore
        => interfaces;

        public Class Base(string name)
        {
            BaseStore = new TypeRep(name);
            return this;
        }

        public Class Constructor(IConstructor constructor)
        {
            constructors.Add(constructor);
            return this;
        }

        public Constructor Constructor()
        {
            return new Constructor();
        }

        public Class Member(IClassMember member)
        {
            members.Add(member);
            return this;
        }

        public Class Members(params IClassMember[] members)
        {
            this.members.AddRange(members);
            return this;
        }

        public Class Members<T>(IEnumerable<T> items, params Func<T, IClassMember>[] memberMakers)
        {
            members.AddRange(items.SelectMany(
                            item => memberMakers.Select(lambda => lambda(item))));
            return this;
        }
    }
}
