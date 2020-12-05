using System;
using System.Collections.Generic;

namespace FluentDom.Generator
{
    public abstract class GeneratorBase
    {
        protected StringBuilderWithTabs sb = new();

        public virtual string Generate(Code code)
        {

            if (string.IsNullOrWhiteSpace(code.Namespace))
            {
                this
                   .OutputUsings(code)
                   .OutputSelect(code.ClassStore, (generator, x) => generator.OutputClass(x));
            }
            else
            {
                this
                   .OutputUsings(code)
                   .OpenNamespace(code)
                   .OutputSelect(code.ClassStore, (generator, x) => generator.OutputClass(x))
                   .CloseNamespace(code);
            }

            return sb.ToString();
        }

        public override string ToString()
        {
            return sb.ToString();
        }

        protected internal abstract GeneratorBase OutputUsings(Code code);
        protected internal abstract GeneratorBase OpenNamespace(Code code);
        protected internal abstract GeneratorBase CloseNamespace(Code code);
        protected internal abstract GeneratorBase OpenClass(Class cls);
        protected internal abstract GeneratorBase CloseClass(Class cls);
        protected internal abstract GeneratorBase OutputField(Field field);
        protected internal abstract GeneratorBase OpenConstructor(Constructor ctor);
        protected internal abstract GeneratorBase CloseConstructor(Constructor ctor);
        protected internal abstract GeneratorBase OpenMethod(Method method);
        protected internal abstract GeneratorBase CloseMethod(Method method);
        protected internal abstract GeneratorBase OpenProperty(Property property);
        protected internal abstract GeneratorBase CloseProperty(Property property);
        protected internal abstract GeneratorBase OpenPropertyGetter(Property property);
        protected internal abstract GeneratorBase ClosePropertyGetter(Property property);
        protected internal abstract GeneratorBase OpenPropertySetter(Property property);
        protected internal abstract GeneratorBase ClosePropertySetter(Property property);

        protected internal abstract GeneratorBase OutputStatement(IExpression expression);
        protected internal abstract GeneratorBase OutputStatements(IEnumerable<IExpression> expressions);


        // This will often force a closure. Consider if heavily used.
        protected internal GeneratorBase Optional(bool shouldOutput, Func<string> lineMaker)
        {
            if (shouldOutput)
            {
                sb.AppendLine(lineMaker());
            }
            return this;
        }

        protected internal GeneratorBase OutputLine()
        {
            sb.AppendBlankLine();
            return this;
        }

        protected internal GeneratorBase OutputLine(string line = "")
        {
            sb.AppendLine(line);
            return this;
        }

        // The generator is passed to avoid a lambda closure. Since this runs in VS, attention to memory where its this easy seems a good idea
        // The generator is returned so the same methods can be used in OutputSelect and fluent calls
        protected internal GeneratorBase OutputSelect<T>(IEnumerable<T> items, params Func<GeneratorBase, T, GeneratorBase>[] makers)
        {
            foreach (var item in items)
            {
                foreach (var maker in makers)
                {
                    maker(this, item);
                }
            }
            return this;
        }

        protected internal virtual GeneratorBase OutputClass(Class cls)
        {
            return OpenClass(cls)
                   .OutputSelect(cls.MemberStore, (g, x) => g.OutputMember(x))
                   .CloseClass(cls);
        }

        protected internal virtual GeneratorBase OutputMember(IClassMember member)
        {
            return member switch
            {
                Property p => OutputProperty(p),
                Constructor c => OutputConstructor(c),
                Method m => OutputMethod(m),
                BlankLine => OutputLine(),
                Field f => OutputField(f),
                _ => throw new NotImplementedException()
            };
        }

        protected internal virtual GeneratorBase OutputConstructor(Constructor ctor)
        {
            return OpenConstructor(ctor)
                   .OutputStatements(ctor.StatementStore)
                   .CloseConstructor(ctor);
        }
        protected internal virtual GeneratorBase OutputMethod(Method method)
        {
            return OpenMethod(method)
                   .OutputStatements(method.StatementStore)
                   .CloseMethod(method);
        }

        protected internal virtual GeneratorBase OutputProperty(Property property)
        {
            // C# needs to override this for auto properties
            OpenProperty(property);
            if (property.GetterStatementStore.AnyStatements())
            {
                OpenPropertyGetter(property);
                OutputStatements(property.GetterStatementStore.StatementStore);
                ClosePropertyGetter(property);
            }
            if (property.SetterStatementStore.AnyStatements())
            {
                OpenPropertySetter(property);
                OutputStatements(property.SetterStatementStore.StatementStore);
                ClosePropertyGetter(property);
            }
            CloseProperty(property);
            return this;
        }
    }
}