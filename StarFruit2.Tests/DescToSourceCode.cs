using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace StarFruit2.Tests
{
    public class DescToSourceCode
    {
        [Theory]
        [InlineData("EmptySampleData.cs")]
        [InlineData("SingleArgumentSampleData.cs")]
        [InlineData("SingleOptionWithArgumentSampleData.cs")]
        [InlineData("SeveralOptionsAndArgumentsSampleData.cs")]
        public void GetArguments(string fileName)
        {
        
        }
    }
}
