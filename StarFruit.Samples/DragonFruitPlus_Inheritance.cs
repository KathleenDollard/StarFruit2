namespace StarFruit.Samples.DragonFruitPlus_Inheritance
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

    public abstract class ToolCommand : DotnetCommand
    {
    }

    public abstract class InstallCommand : ToolCommand
    {
        public override void Invoke() { }

    }
}

namespace StarFruit
{
    public abstract class CommandLineCommand
    {
        public abstract void Invoke();
    }
}
