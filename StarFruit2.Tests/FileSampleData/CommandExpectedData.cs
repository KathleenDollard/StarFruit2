using System.Collections.Generic;

namespace StarFruit2.Tests.SampleData
{
    public class FileCommandSampleData
    {
        public FileCommandSampleData(string fileName, CommandExpectedData commandExpectedData)
        {
            FileName = fileName;
            CommandExpectedData = commandExpectedData;
        }

        public string FileName { get; set; }
        public CommandExpectedData CommandExpectedData { get; set; }

    }
}
