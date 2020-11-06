﻿using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

namespace StarFruit2
{
    internal class NotInvocableCommandSourceResult : CommandSourceResult
    {
        private InvocationContext context;

        public NotInvocableCommandSourceResult(ParseResult parseResult, int exitCode)
            : base(parseResult, exitCode)
        {
            this.context = context;
        }
    }
}