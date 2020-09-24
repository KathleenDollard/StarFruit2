using System.Collections.Generic;

namespace StarFruit2.Tests.SampleData
{
    public class OptionSampleData
    {
        public string Name { get; set; }
        public IEnumerable<ArgumentSampleData> Arguments { get; set; } = new List<ArgumentSampleData>();
    }
}
