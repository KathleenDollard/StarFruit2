using System.Collections.Generic;

namespace StarFruit2.Tests.SampleData
{
    public class CommandExpectedData
    {
        public string Name { get; set; }
        public IEnumerable<ArgumentSampleData> Arguments { get; set; } = new List<ArgumentSampleData>();
        public IEnumerable<OptionSampleData> Options { get; set; } = new List<OptionSampleData>();
        public IEnumerable<CommandExpectedData> SubCommands { get; set; } = new List<CommandExpectedData>();
    
    }
}
