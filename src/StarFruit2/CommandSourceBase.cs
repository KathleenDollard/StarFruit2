using System.CommandLine;
using System.CommandLine.Parsing;

namespace StarFruit2
{

    public abstract class CommandSourceBase
    {
        protected CommandSourceBase(Command command, CommandSourceBase? parentCommandSource)
        {
            Command = command;
            ParentCommandSource = parentCommandSource;
        }

        public CommandSourceBase? ParentCommandSource { get; }
        public Command Command { get; }

        public virtual CommandSourceResult GetCommandSourceResult(ParseResult parseResult, int exitCode)
        {
            return new NotInvocableCommandSourceResult(parseResult, this, exitCode);
        }
    }

 
}
