using System.Text;

// Single reponsibility: Writing to a wrapped string builder
// Knows nothing about:
// * FluentDom structures like Class or Statement
// * Language like C# or VB
// * Problem space like StarFruit

namespace FluentDom
{
    public class StringBuilderWithTabs
    {
        private readonly StringBuilder sb = new();
        private int tabs = 0;
        private int tabWidth = 3;
        private const string newLine = "\n";

        public StringBuilderWithTabs AppendLine()
        {
            sb.Append(newLine);
            return this;
        }

        public StringBuilderWithTabs AppendLine(string line)
        {
            tabs = tabs < 0 ? 0 : tabs;
            sb.Append(new string(' ', tabs * tabWidth));
            sb.Append(line + newLine);
            return this;
        }

        internal void AppendBlankLine()
        {
            sb.AppendLine();
        }


        public StringBuilderWithTabs Append(string part)
        {
            sb.Append(part);
            return this;
        }

        public StringBuilderWithTabs AppendJoin(string separator, params string[] values)
        {
            sb.Append(string.Join(separator, values));
            return this;
        }

        // Stack of integers
        public StringBuilderWithTabs IncreaseTabs(int extraTabs = 0)
        {
            tabs += 1 + extraTabs;
            return this;
        }

        public StringBuilderWithTabs DecreaseTabs(int extraTabs = 0)
        {
            tabs -= 1 - extraTabs;
            return this;
        }

        public override string ToString()
        {
            return sb.ToString();
        }
    }
}
