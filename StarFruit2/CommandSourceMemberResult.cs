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
            // KAD! problem here is that string-property isn't present. Need to figure out
            // whether default value needs to be set here, or would already have been set.
            // And when there isn't a result how this should work (also save the Symbol in 
            // the CommandSourceMemberResult, etc
            var commmandResult = parseResult.CommandResult;
            SymbolResult sysCommandLineResult = null;
            while (!(commmandResult is null) && sysCommandLineResult is null)
            {
                sysCommandLineResult = parseResult.RootCommandResult.FindResultFor(option);
                commmandResult = commmandResult.Parent as CommandResult;
            }
            return new CommandSourceOptionMemberResult<T>(sysCommandLineResult as OptionResult);
        }

        public static CommandSourceMemberResult<T> Create<T>(Argument<T> argument, ParseResult parseResult)
        {
            var commmandResult = parseResult.CommandResult;
            SymbolResult sysCommandLineResult = null;
            while (!(commmandResult is null) && sysCommandLineResult is null)
            {
                sysCommandLineResult = parseResult.RootCommandResult.FindResultFor(argument);
                commmandResult = commmandResult.Parent as CommandResult;
            }
            return new CommandSourceArgumentMemberResult<T>(sysCommandLineResult as ArgumentResult);
        }
    }

    public abstract class CommandSourceMemberResult<T>
    {
        private SymbolResult sysCommandLineResult;

        public CommandSourceMemberResult(T value)
        {
            Value = value;
        }

        // This is writeable so changes can be made during validation stage
        public T Value { get; set; }

    }

    public class CommandSourceArgumentMemberResult<T> : CommandSourceMemberResult<T>
    {
        private ArgumentResult sysCommandLineResult;

        public CommandSourceArgumentMemberResult(ArgumentResult sysCommandLineResult)
            : base(sysCommandLineResult.GetValueOrDefault<T>())
        {
            this.sysCommandLineResult = sysCommandLineResult;
        }

        // public bool UserSpecified =>    
    }

    public class CommandSourceOptionMemberResult<T> : CommandSourceMemberResult<T>
    {
        private OptionResult sysCommandLineResult;
        public CommandSourceOptionMemberResult(OptionResult sysCommandLineResult)
             : base(sysCommandLineResult.GetValueOrDefault<T>())
        {
            this.sysCommandLineResult = sysCommandLineResult;
        }
        // public bool UserSpecified =>    

    }
}
