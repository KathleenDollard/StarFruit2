﻿using System;

namespace StarFruit.Common
{
    public static class CommonExtensions
    {
        public static string NewLineWithTabs(int tabsCount)
        {
            return Environment.NewLine + new string(' ', tabsCount) + new string(' ', tabsCount) + new string(' ', tabsCount);
        }

    }
}
