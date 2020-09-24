using System;

namespace StarFruit.Samples.DragonFruitPlus_ComplexCtor
{
    public class Program
    {
        public void Main(DotnetCommand cmd)
        {
            cmd.Invoke();
        }
    }


    public abstract class DotnetCommand : CommandLineCommand
    {
    }

    public abstract class ToolCommand : CommandLineCommand
    {
        private DotnetCommand  _dotnetCommand;
        public ToolCommand(DotnetCommand parentCommand)
        {
            _dotnetCommand = parentCommand;
        }
    }


    public class InstallCommand : CommandLineCommand
    {
        private ToolCommand _toolCommand;
        public InstallCommand(ToolCommand parentCommand)
        {
            _toolCommand = parentCommand;
        }
        public override void Invoke()
        {  
            // Do work
        }

    }
}
namespace StarFruit
{
    internal class CommandLineParentAttribute : Attribute
    {
        public CommandLineParentAttribute(Type parentType)
        {

        }
    }
}
