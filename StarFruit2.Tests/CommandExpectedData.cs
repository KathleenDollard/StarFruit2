using System.Collections.Generic;

namespace StarFruit2.Tests.SampleData
{
    public class CommandExpectedData
    {
        public string FileName { get; set; }
        public string Name { get; set; }
        public IEnumerable<ArgumentExpectedData> Arguments { get; set; } = new List<ArgumentExpectedData>();
        public IEnumerable<OptionExpectedData> Options { get; set; } = new List<OptionExpectedData>();
        public IEnumerable<CommandExpectedData> SubCommands { get; set; } = new List<CommandExpectedData>();

    }
}
