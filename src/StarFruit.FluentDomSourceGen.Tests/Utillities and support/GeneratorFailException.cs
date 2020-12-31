using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Text;

namespace StarFruit.FluentDomSourceGen.Tests
{
    public class GeneratorFailException : InvalidOperationException
    {
        // Standard constructors are not included because if you are not carryiing a set of diagnostics, don't use this exception

        public GeneratorFailException(string message, IEnumerable<Diagnostic> diagnostics)
            : base(message)
        {
            Diagnostics = diagnostics;
        }

        public IEnumerable<Diagnostic> Diagnostics { get; }
    }
}
