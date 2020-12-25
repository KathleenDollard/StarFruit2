using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentDom
{

    [Flags]
    public enum ClassModifiers
    {
        Static = 0b_0001,
        Partial = 0b_0010,
        Sealed = 0b_0100,
        Abstract = 0b_1000,
    }

    public class Class
    {
        public Class(string name, Scope scope = Scope.Public)
        {
            Name = name;
            Scope = scope;
        }


        public string Name { get; }
        public Scope Scope { get; }
        public ClassModifiers Modifiers { get; }
        public TypeRep? BaseStore { get; private set; }
        private readonly List<TypeRep> interfaces = new();
        private readonly List<IClassMember> members = new();

        public IEnumerable<IClassMember> MemberStore
            => members;

        public IEnumerable<TypeRep> InterfaceStore
        => interfaces;

        public Class Base(TypeRep typeRep)
        {
            BaseStore = typeRep;
            return this;
        }

        public Class Base(string name)
            => Base(new TypeRep(name));

        public Class Constructor(Constructor constructor)
        {
            members.Add(constructor);
            return this;
        }

        public Class BlankLine()
        {
            members.Add(new BlankLine());
            return this;
        }

        public Constructor Constructor(TypeRep containingType)
        {
            return new Constructor(containingType);
        }

        public Class Member(IClassMember member)
        {
            members.Add(member);
            return this;
        }

        public Class OptionalMembers(bool condition, params Func<Class, Class>[] memberMakers)
        {
            if (condition)
            {
                foreach (var memberMaker in memberMakers)
                {
                    memberMaker(this);
                }
            }
            return this;
        }

        public Class Members(params IClassMember[] members)
        {
            this.members.AddRange(members);
            return this;
        }

        public Class Members<T>(IEnumerable<T> items, params Func<T, IClassMember>[] makers)
        {
            members.AddRange(items.SelectMany(
                            item => makers.Select(lambda => lambda(item))));
            return this;
        }

        public Class Property(string name, TypeRep typeRep, Scope scope = Scope.Public, bool readOnly = false)
        {
            members.Add(new Property(name, typeRep, scope, readOnly));
            return this;
        }

        public Class Property(Property property)
        {
            members.Add(property);
            return this;
        }

        public Class Properties<T>(IEnumerable<T> items, params Func<T, Property>[] makers)
        {
            members.AddRange(items.SelectMany(
                            item => makers.Select(lambda => lambda(item))));
            return this;
        }

        public Class Method(Method method)
        {
            members.Add(method);
            return this;
        }

        public Class Methods<T>(IEnumerable<T> items, params Func<T, Method>[] makers)
        {
            members.AddRange(items.SelectMany(
                            item => makers.Select(lambda => lambda(item))));
            return this;
        }

        public Class OptionalMember(bool condition, params IClassMember[] members)
        {
            if (condition)
            {
                Members(members);
            }
            return this;
        }

        public Class Field(string name, TypeRep typeRep, Scope scope = Scope.Public, bool readOnly = false)
        {
            members.Add(new Field(name, typeRep, scope, readOnly));
            return this;
        }

        public Class Fields<T>(IEnumerable<T> items, params Func<T, Property>[] makers)
        {
            members.AddRange(items.SelectMany(
                            item => makers.Select(lambda => lambda(item))));
            return this;
        }
    }
}
