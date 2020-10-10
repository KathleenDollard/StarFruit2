using System.CommandLine.Parsing;

namespace StarFruit2
{
    public class CommandSourceResult
    {
        public CommandSourceResult(object? newInstance, ParseResult parseResult, int exitCode)
        {
            NewInstance = newInstance;
            ParseResult = parseResult;
            ExitCode = exitCode;
        }

        public object? NewInstance { get; }
        public ParseResult ParseResult { get; }
        public int ExitCode { get; }
    }
}