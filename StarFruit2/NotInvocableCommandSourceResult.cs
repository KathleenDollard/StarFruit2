using System.CommandLine.Invocation;

namespace StarFruit2
{
    internal class NotInvocableCommandSourceResult : CommandSourceResult
    {
        private InvocationContext context;

        public NotInvocableCommandSourceResult(InvocationContext context)
        {
            this.context = context;
        }
    }
}