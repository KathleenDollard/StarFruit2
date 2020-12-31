using System.Collections.Generic;
using System.Linq;

namespace FluentDom
{
    public abstract class MethodCallBase : IExpression
    {
        private IExpression[] arguments;

        public IEnumerable<IExpression> ArgumentStore
             => arguments;

        public MethodCallBase(IExpression[] arguments)
        {
            this.arguments = arguments;
        }

        public MethodCallBase(string[] arguments)
        {
            this.arguments = arguments.Select(x => Expression.Value(x)).ToArray();
        }
    }

    public class MethodCall : MethodCallBase
    {
        public MethodCall(string name,  params IExpression[] arguments)
         : base(arguments)
        {
            Name = name;
        }

        public string Name { get; }
    }

    public class BaseConstructorCall : MethodCallBase
    {
        public BaseConstructorCall()
             : base(new string[] { }) { }
        public BaseConstructorCall(params IExpression[] arguments)
            : base(arguments) { }
        public BaseConstructorCall(params string[] arguments)
            : base(arguments) { }
    }

    public class ThisConstructorCall : MethodCallBase
    {
        public ThisConstructorCall()
             : base(new string[] { }) { }
        public ThisConstructorCall(params IExpression[] arguments)
            : base(arguments) { }
        public ThisConstructorCall(params string[] arguments)
           : base(arguments) { }
    }

    public class Base : IExpression
    {  }

    public class This : IExpression
    {  }


    public class Null : IExpression
    { }
}