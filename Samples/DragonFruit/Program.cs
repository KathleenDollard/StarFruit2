using System;

namespace DragonFruit
{
    class Program
    {
        /// <summary>
        /// Illustrating DragonFruit!!!
        /// </summary>
        /// <param name="intArg">Supply a magic number</param>
        /// <param name="stringArg">Who you want to say Hello to</param>
        /// <returns></returns>
        static int Main(int intArg, string stringArg)
        {
            Console.WriteLine($"Hello {stringArg}! (magic number: {intArg})");
            return 0;
        }

        // Temp: Avoid errors until generation is done
        static int Main (string[]args)
        {
            return Main(42, "World");
        }
    }
}
