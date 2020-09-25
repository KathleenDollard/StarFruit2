using System.Collections.Generic;

namespace StarFruit2.Tests.SampleData
{
    public class OptionExpectedData
    {
        public string Name { get; set; }
        public IEnumerable<ArgumentExpectedData> Arguments { get; set; } = new List<ArgumentExpectedData>();
    }
}
