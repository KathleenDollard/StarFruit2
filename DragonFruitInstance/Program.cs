using System;

Console.WriteLine($"Temp until code below works");


//var myArgs = CommandSource.CreateInstance<MyArgs>();
//Console.WriteLine($"Hello {myArgs.stringArg}! (magic number: {myArgs.intArg}");

class MyArgs
{
    /// <summary>
    /// StringArg description
    /// </summary>
    string StringArg { get; set; }

    /// <summary>
    /// IntArg description
    /// </summary>
    string IntArg { get; set; }
}
