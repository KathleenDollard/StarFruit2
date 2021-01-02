using System;
using System.Collections.Generic;

namespace FluentDom.Generator
{
    public abstract class GeneratorBase
    {
        protected StringBuilderWithTabs sb = new();

        public virtual string Generate(Code code)
            => this
                .OutputUsings(code)
                .Optional(!string.IsNullOrWhiteSpace(code.Namespace), () => OpenNamespace(code))
                .OutputSelect(code.ClassStore, (generator, x) => generator.OutputClass(x))
                .Optional(!string.IsNullOrWhiteSpace(code.Namespace), () => CloseNamespace(code))
                .GetOutput();

        public string GetOutput()
        {
            return sb.ToString();
        }

        protected internal abstract GeneratorBase OutputUsings(Code code);
        protected internal abstract GeneratorBase OpenNamespace(Code code);
        protected internal abstract GeneratorBase OpenClass(Class cls);
        protected internal abstract GeneratorBase OutputField(Field field);
        protected internal abstract GeneratorBase OpenConstructor(Constructor ctor);
        protected internal abstract GeneratorBase OpenMethod(Method method);
        protected internal abstract GeneratorBase OpenProperty(Property property);
        protected internal abstract GeneratorBase OpenPropertyGetter(Property property);
        protected internal abstract GeneratorBase OpenPropertySetter(Property property);
        protected internal abstract GeneratorBase OutputStatement(IExpression expression);
        protected internal abstract string BlockClose(BlockType blockType);

       protected internal virtual GeneratorBase CloseNamespace(Code code)
            => Close(BlockType.Namespace);
        protected internal virtual GeneratorBase CloseClass(Class cls)
             => Close(BlockType.Class);
        protected internal virtual GeneratorBase CloseConstructor(Constructor ctor)
              => Close(BlockType.Constructor);
        protected internal virtual GeneratorBase CloseMethod(Method method)
              => Close(method.ReturnTypeStore is null
                ? BlockType.Sub
                : BlockType.Function);
        protected internal virtual GeneratorBase CloseProperty(Property property)
              => Close(BlockType.Property);
        protected internal virtual GeneratorBase ClosePropertyGetter(Property property)
              => Close(BlockType.Getter);
        protected internal virtual GeneratorBase ClosePropertySetter(Property property)
              => Close(BlockType.Setter);

        protected internal virtual GeneratorBase Close(BlockType blockType)
        {
            sb.DecreaseTabs();
            OutputLine(BlockClose(blockType));
            return this;
        }


        protected internal virtual GeneratorBase OutputStatements(IEnumerable<IExpression> expressions)
        {
            foreach (var expression in expressions)
            {
                OutputStatement(expression);
            }
            return this;
        }


        // This will often force a closure. Consider if heavily used.
        protected internal GeneratorBase Optional(bool shouldOutput, Func<string> lineMaker)
        {
            if (shouldOutput)
            {
                sb.AppendLine(lineMaker());
            }
            return this;
        }

        // This will often force a closure. Consider if heavily used.
        protected internal GeneratorBase Optional(bool shouldOutput, Func<GeneratorBase> lineMaker)
        {
            if (shouldOutput)
            {
                lineMaker();
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
                _ => throw new NotImplementedException($"Not implemented {nameof(IClassMember)} in {nameof(GeneratorBase)}.{nameof(OutputMember)}")
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
                ClosePropertySetter(property);
            }
            CloseProperty(property);
            return this;
        }
    }
}