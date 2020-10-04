using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace StarFruit2
{
    public abstract class CliBase
    {
        private readonly string? nspace;

        public string Namespace 
            => nspace ?? GetType().Namespace ?? "";

        public virtual Command GetCommand()
        {
            // return a Root Command based on derived class
            return CreateCommand(GetType());
        }
        public virtual Command GetCommand<TCommand>()
        {
            // return a Command based on specified class
            return GetCommand(typeof(TCommand));
        }
        public virtual Command GetCommand(Type commandType)
        {
            // return a Command based on specified class
            return CreateCommand(commandType);
        }
        public virtual Command GetCommand(string methodName)
        {
            // return a Command based on specified method in derived class
            return CreateCommand(methodName);
        }

        public void Register<TCommand>()
        {
            // Register a class as a subcommand of root
        }
        public void Register<TCommand, TBase>()
        {
            // Register a class as a subcommand of specified base
        }

        // This method is generally generated, but if it is explicitly overridden, we skip generation
        protected abstract Command CreateCommand(Type commandType);
        protected abstract Command CreateCommand(string methodName);
    }
}
