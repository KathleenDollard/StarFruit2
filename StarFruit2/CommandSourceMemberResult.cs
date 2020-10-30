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
        public static CommandSourceMemberResult<T> Create<T>(Option<T> option, ParseResult parseResult)
        {
            throw new NotImplementedException();
        }

        public static CommandSourceMemberResult<T> Create<T>(Argument<T> argument, ParseResult parseResult)
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
