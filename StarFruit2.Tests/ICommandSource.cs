using System;
using System.Collections.Generic;
using System.CommandLine;
using System.Text;

namespace StarFruit2.Tests
{
    interface ICommandSource
    {
        Command GetCommand();
    }
}
