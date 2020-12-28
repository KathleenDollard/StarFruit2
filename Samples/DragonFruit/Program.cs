using System;

namespace DragonFruit
{
    class Program
    {
        static int Main(int intArg, string stringArg)
        {
            Console.WriteLine($"Hello {stringArg}! (magic number: {intArg}");
            return 0;
        }

        // Temp: Avoid errors until generation is done
        static int Main (string[]args)
        {
            return Main(42, "World");
        }
    }
}
