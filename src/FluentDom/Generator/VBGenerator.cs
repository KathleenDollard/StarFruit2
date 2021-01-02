using System;
using System.Collections.Generic;
using System.Linq;

namespace FluentDom.Generator
{
    public class VBGenerator : GeneratorBase
    {
        protected internal override GeneratorBase OutputStatement(IExpression expression)
        {
            sb.AppendLine($"{expression.VBString()}");
            return this;
        }

        protected internal override GeneratorBase OutputUsings(Code code)
        {
            return OutputSelect(code.UsingStore.Distinct(new Using.NameEqualComparer()),
                                (g, x) => OutputLine($"Imports {IncludeAlias(x)}{x.UsingNamespace}"))
                .OutputLine();

            string IncludeAlias(Using usingNamespace)
                => string.IsNullOrWhiteSpace(usingNamespace.Alias) ? "" : $"{usingNamespace.Alias} = ";
        }

        protected internal override GeneratorBase OpenNamespace(Code code)
        {
            sb.AppendLine($"Namespace {code.Namespace}");
            sb.IncreaseTabs();
            return this;
        }

        protected internal override GeneratorBase OpenClass(Class cls)
        {
            OutputLine(ClassDeclaration(cls));
            sb.IncreaseTabs();
            if (cls.BaseStore is not null)
            {
                sb.AppendLine($"Inherits {cls.BaseStore.VBString()}");
            }
            foreach (var item in cls.InterfaceStore)
            {
                sb.AppendLine(item.VBString());
            }
            return this;

            static string ClassDeclaration(Class cls)
                => $"{cls.Scope.VBString()}{cls.Modifiers.VBString()}Class {cls.Name}";
        }

        protected internal override GeneratorBase OpenConstructor(Constructor ctor)
        {
            OutputLine(CtorDeclaration(ctor));
            sb.IncreaseTabs();
            Optional(ctor.BaseOrThisCallStore is not null, () => BaseOrThisCall(ctor));
            sb.IncreaseTabs();
            return this;

            static string BaseOrThisCall(Constructor ctor)
            => $"{(ctor.BaseOrThisCallStore is ThisConstructorCall ? "Me.New" : "MyBase.New")}" +
                $"({string.Join(", ", ctor.BaseOrThisCallStore.ArgumentStore.Select(x => x.VBString()))})";

            static string CtorDeclaration(Constructor ctor)
            => $"{ctor.Scope.VBString()}{ctor.Modifiers.VBString()}Sub New" +
               $"({ctor.ParameterStore.VBString()})";
        }

        protected internal override GeneratorBase OpenMethod(Method method)
        {
            OutputLine(MethodDeclaration(method));
            sb.IncreaseTabs();
            return this;

            static string MethodDeclaration(Method method)
               => method.ReturnTypeStore is null
                   ? $"{method.Scope.VBString()}{method.Modifiers.VBString()} Sub " +
                          $"{method.Name}({method.ParameterStore.VBString()})"
                   : $"{method.Scope.VBString()}{method.Modifiers.VBString()} Function " +
                          $"{method.Name}({method.ParameterStore.VBString()}) As {MethodReturnType(method)}";

            static string MethodReturnType(Method method)
                => method.ReturnTypeStore is null
                   ? "Nothing"
                   : method.ReturnTypeStore.VBString();
        }

        protected internal override GeneratorBase OutputProperty(Property property)
        {
            return property.GetterStatementStore.AnyStatements() || property.SetterStatementStore.AnyStatements()
                    ? base.OutputProperty(property)
                    : OutputAutoBodyProperty(property);
        }

        protected internal override GeneratorBase OpenProperty(Property property)
        {
            OutputLine(PropertyDeclaration(property));
            sb.IncreaseTabs();
            return this;
        }

        protected internal override GeneratorBase OpenPropertyGetter(Property property)
        {
            OutputLine("Get");
            sb.IncreaseTabs();
            return this;
        }

        protected internal override GeneratorBase OpenPropertySetter(Property property)
        {
            OutputLine("Set(Value As String)");
            sb.IncreaseTabs();
            return this;
        }

        private static string PropertyDeclaration(Property property)
        {
            return $"{ScopeAndModifiers(property)}Property {property.Name} As {property.TypeRep.VBString()}";

            static string ScopeAndModifiers(Property property)
                => $"{property.Scope.VBString()}{property.Modifiers.VBString()}{Readonly(property)}";
            static string Readonly(Property property)
                => property.ReadOnly
                    ? "ReadOnly "
                    : "";
        }



        protected internal GeneratorBase OutputAutoBodyProperty(Property property)
        => OutputLine($"{PropertyDeclaration(property)}");

        protected internal GeneratorBase OutputExpressionBodyProperty(Property property)
        {
            var expr = property.GetterStatementStore.StatementStore.First();
            if (expr is Return returnExpression)
            {
                expr = returnExpression.ReturnExpression;
            }
            return OutputLine(PropertyDeclaration(property))
                              .OutputLine($"=> {expr.VBString()};")
                              .OutputLine();
        }


        protected internal override GeneratorBase OutputField(Field field)
        {
            OutputLine($"{field.Scope.VBString()}{(field.ReadOnly ? "readonly " : "")}{field.TypeRep.VBString()} {field.Name};");
            return this;
        }

        protected internal override string BlockClose(BlockType blockType)
            => "End " + blockType.VBString();


    }
}
