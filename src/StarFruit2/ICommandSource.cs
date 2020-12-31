using System.CommandLine;

namespace StarFruit2
{
    public interface ICommandSource
    {
        Command GetCommand();
    }
}
