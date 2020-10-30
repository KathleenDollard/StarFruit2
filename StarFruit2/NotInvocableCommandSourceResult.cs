using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

namespace StarFruit2
{
    internal class NotInvocableCommandSourceResult : CommandSourceResult
    {
        private InvocationContext context;

        public NotInvocableCommandSourceResult(ParseResult parseResult)
            : base(parseResult)
        {
            this.context = context;
        }
    }
}