using StarFruit2.Common;
using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Threading.Tasks;

namespace StarFruit2
{
    /// <summary>
    /// </summary>
    /// <remarks>
    /// There are two different invocations. A StarFruit2 created method is the handler
    /// for the System.CommandLine command, in order to retrieve the data the user entered.
    /// The users method is separately invoked with this data within CommandSourceResult.
    /// This two stage approach allows authors to validate their data, for example doing
    /// validations that require two values.
    /// </remarks>
    public class CommandSourceResult
    {
        protected Func<Task<int>>? RunFunc;
        private Command command;

        public CommandSourceResult(Command command)
        {
            this.command = command;

        }

        public object? NewInstance { get; }
        public ParseResult ParseResult { get; }
        public int ExitCode { get; }

        public async Task<int> RunAsync(string[] args)
        {
            Assert.NotNull(Command);
            var exit = Command.Invoke(args);
            if (exit != 0)
            {
                // TODO: handle failure
                throw new NotImplementedException();
            }
            Assert.NotNull(RunFunc);
            return await RunFunc();
        }

        public int Run(string[] args)
        {
            Assert.NotNull(Command);
            var exit = Command.Invoke(args);
            if (exit != 0)
            {
                // TODO: handle failure
                throw new NotImplementedException();
            }
            Assert.NotNull(RunFunc);
            return RunFunc();
        }

    }
}