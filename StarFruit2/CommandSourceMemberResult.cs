using System;
using System.Collections.Generic;
using System.CommandLine;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System.Text;

namespace StarFruit2
{
    public class CommandSourceMemberResult
    {
        public static CommandSourceMemberResult<TMember> Create<TMember>(Option option, InvocationContext context)
        {
            throw new NotImplementedException();
        }

        public static CommandSourceMemberResult<TMember> Create<TMember>(Argument argument, InvocationContext context)
        {
            throw new NotImplementedException();
        }
    }

    public class CommandSourceMemberResult<T>
    {
        private SymbolResult sysCommandLineResult;

        public T Value { get; }

        // arg or option result 
        // was it specified
    }
}
