using System;
using System.CommandLine;

namespace StarFruit2.StarFruitSurface
{
    public partial class SampleOneLayerCli : CliBase
    {
        public int Invoke(bool greeting, int count)
        {
            Console.WriteLine(greeting);
            return 0;
        }
    }

    public partial class SampleOneLayerCli
    {
        protected override Command CreateCommand(string methodName)
        {
            throw new NotImplementedException();
        }

        protected override Command CreateCommand(Type commandType)
        {
            throw new NotImplementedException();
        }
    }



}
