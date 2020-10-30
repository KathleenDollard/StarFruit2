using StarFruit2.Common;
using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Threading.Tasks;

namespace StarFruit2
{
    public abstract class CommandSourceResult
    {
        public CommandSourceResult(ParseResult parseResult)
        {
            ParseResult = parseResult;
        }

        public bool EarlyReturn { get; }
        public ParseResult ParseResult { get; }
        public int ExitCode { get; }

        public virtual int Run() => 0;
        public virtual async Task<int> RunAsync() => await Task.FromResult(0);

    }

    /// <summary>
    /// </summary>
    /// <remarks>
    /// There are two different invocations. A StarFruit2 created method is the handler
    /// for the System.CommandLine command, in order to retrieve the data the user entered.
    /// The users method is separately invoked with this data within CommandSourceResult.
    /// This two stage approach allows authors to validate their data, for example doing
    /// validations that require two values.
    /// </remarks>
    public abstract class CommandSourceResult<T> : CommandSourceResult
    {

        public abstract T MakeNewInstance();



        //public CommandSourceResult(Command command)
        //{
        //    this.command = command;

        //}

        public T? NewInstance { get; }



    }
}