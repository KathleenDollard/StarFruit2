using System.CommandLine.Parsing;
using System.Threading.Tasks;

namespace StarFruit2
{
    public abstract class CommandSourceResult
    {
        public CommandSourceResult(ParseResult parseResult,int exitCode, bool earlyReturn =false)
        {
            ParseResult = parseResult;
            ExitCode = exitCode;
        }

        public bool EarlyReturn
            => this is EmptyCommandSourceResult;
        public ParseResult ParseResult { get; }
        public int ExitCode { get; }

        public virtual int Run() => 0;
        public virtual async Task<int> RunAsync() => await Task.FromResult(0);

    }

    public class EmptyCommandSourceResult : CommandSourceResult
    {
        public EmptyCommandSourceResult(ParseResult parseResult, int exitCode)
            : base(parseResult, exitCode, earlyReturn:true )
        { }
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
        protected CommandSourceResult(ParseResult parseResult, CommandSource commandSource, int exitCode)
            : base(parseResult, exitCode)
        {
            CommandSource = commandSource;
        }

        public abstract T CreateInstance();
        public CommandSource CommandSource { get; }
    }
}