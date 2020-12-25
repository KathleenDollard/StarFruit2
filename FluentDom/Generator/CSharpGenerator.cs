using System.Runtime.CompilerServices;
using System;
using System.Collections.Generic;
using System.Linq;

[assembly: InternalsVisibleTo("FluentDom.Tests")]

// Single reponsibility: C# specific expression of FluentDom structures like Class or Statement via StringBuilderWithTabs
// Knows nothing about:
// * How to actually outpu t to a StringBulider (no AppendLine here)
// * Problem space like StarFruit
//
// Code in this class will be raised to a base class, and there will also be a VB implementation

namespace FluentDom.Generator
{
    public class CSharpGenerator : GeneratorBase
    {

        internal void OutputProperty(LocalDeclaration expression)
        {
            throw new NotImplementedException();
        }

        internal void OutputProperty(MethodCallBase expression)
        {
            throw new NotImplementedException();
        }

        protected internal override GeneratorBase OutputStatement(IExpression expression)
        {
            sb.AppendLine($"{expression.CSharpString()};");
            return this;
        }

        protected internal override GeneratorBase OutputStatements(IEnumerable<IExpression> expressions)
        {
            foreach (var expression in expressions)
            {
                OutputStatement(expression);
            }
            return this;
        }

        protected internal override GeneratorBase OutputUsings(Code code)
        {
            return
                OutputSelect(code.UsingStore, (g, x)
                             => OutputLine($"using {IncludeUsingStatic(x)}{IncludeAlias(x)}{x.UsingNamespace};"))
                .OutputLine();

            string IncludeAlias(Using usingNamespace)
                => string.IsNullOrWhiteSpace(usingNamespace.Alias) ? "" : $"{usingNamespace.Alias} = ";

            string IncludeUsingStatic(Using usingNamespace)
                => usingNamespace.UsingStatic ? "static " : "";
        }

        protected internal override GeneratorBase OpenNamespace(Code code)
        {
            sb.AppendLine($"namespace {code.Namespace}");
            OpenCurly();

            return this;
        }

        protected internal override GeneratorBase CloseNamespace(Code code)
                => CloseCurly();

        protected internal override GeneratorBase OpenClass(Class cls)
        {
            OutputLine(ClassDeclaration(cls));
            OpenCurly();
            return this;

            static string ClassDeclaration(Class cls)
            {
                var baseAndInterfaces = cls.InterfaceStore;
                if (cls.BaseStore is not null)
                {
                    baseAndInterfaces = cls.InterfaceStore.Prepend(cls.BaseStore);
                }
                string baseString = baseAndInterfaces.Any()
                                                ? $" : {string.Join(", ", baseAndInterfaces.Select(x => x.CSharpString()))}"
                                                : "";
                return $"{cls.Scope.CSharpString()}{cls.Modifiers.CSharpString()}class {cls.Name}" + baseString;
            }
        }
        protected internal override GeneratorBase CloseClass(Class cls)
            => CloseCurly();

        protected internal override GeneratorBase OpenConstructor(Constructor ctor)
        {
            OutputLine(CtorDeclaration(ctor));
            Optional(ctor.BaseOrThisCallStore is not null, () => BaseOrThisCall(ctor));
            OpenCurly();
            return this;

            static string BaseOrThisCall(Constructor ctor)
            => $": {(ctor.BaseOrThisCallStore is ThisConstructorCall ? "this" : "base")}" +
                $"({string.Join(", ", ctor.BaseOrThisCallStore.ArgumentStore.Select(x => x.CSharpString()))})";

            static string CtorDeclaration(Constructor ctor)
            => $"{ctor.Scope.CSharpString()}{ctor.Modifiers.CSharpString()}{ctor.ContainingType.Name}" +
               $"({ctor.ParameterStore.CSharpString()})";
        }
        protected internal override GeneratorBase CloseConstructor(Constructor ctor)
            => CloseCurly();

        protected internal override GeneratorBase OpenMethod(Method method)
        {
            OutputLine(MethodDeclaration(method));
            OpenCurly();
            return this;

            static string MethodDeclaration(Method method)
            => $"{method.Scope.CSharpString()}{method.Modifiers.CSharpString()}{MethodReturnType(method)} {method.Name}" +
               $"({method.ParameterStore.CSharpString()})";

            static string MethodReturnType(Method method)
                => method.ReturnTypeStore is null
                   ? "void"
                   : method.ReturnTypeStore.CSharpString();
        }
        protected internal override GeneratorBase CloseMethod(Method method)
           => CloseCurly();

        protected internal override GeneratorBase OutputProperty(Property property)
        {
            return property.ReadOnly
                        && property.GetterStatementStore.StatementStore.Count() == 1
                ? OutputExpressionBodyProperty(property)
                : property.GetterStatementStore.AnyStatements() || property.SetterStatementStore.AnyStatements()
                    ? base.OutputProperty(property)
                    : OutputAutoBodyProperty(property);
        }

        protected internal override GeneratorBase OpenProperty(Property property)
        {
            OutputLine(PropertyDeclaration(property));
            OpenCurly();
            return this;
        }

        protected internal override GeneratorBase OpenPropertyGetter(Property property)
        {
            OutputLine("get");
            OpenCurly();
            return this;
        }

        protected internal override GeneratorBase OpenPropertySetter(Property property)
        {
            OutputLine("set");
            OpenCurly();
            return this;
        }

        private static string PropertyDeclaration(Property property)
        => $"{property.Scope.CSharpString()}{property.Modifiers.CSharpString()}{property.TypeRep.CSharpString()} {property.Name}";

        protected internal GeneratorBase OutputAutoBodyProperty(Property property)
        => OutputLine($"{PropertyDeclaration(property)} {{ get;{(!property.ReadOnly ? " set;" : "")} }}");

        protected internal GeneratorBase OutputExpressionBodyProperty(Property property)
        {
            var expr = property.GetterStatementStore.StatementStore.First();
            if (expr is Return returnExpression)
            {
                expr = returnExpression.ReturnExpression;
            }
            return OutputLine(PropertyDeclaration(property))
                              .OutputLine($"=> {expr.CSharpString()};")
                              .OutputLine();
        }

        protected internal override GeneratorBase CloseProperty(Property property)
        => CloseCurly();

        protected internal override GeneratorBase ClosePropertyGetter(Property property)
        => CloseCurly();

        protected internal override GeneratorBase ClosePropertySetter(Property property)
        => CloseCurly();

        protected internal GeneratorBase OpenCurly()
        {
            sb.AppendLine("{").IncreaseTabs();
            return this;
        }

        protected internal GeneratorBase CloseCurly()
        {
            sb.DecreaseTabs().AppendLine("}");
            return this;
        }

        protected internal override GeneratorBase OutputField(Field field)
        {
            OutputLine($"{field.Scope.CSharpString()}{(field.ReadOnly ? "readonly " : "")}{field.TypeRep.CSharpString()} {field.Name};");
            return this;
        }
    }
}
