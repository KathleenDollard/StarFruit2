using StarFruit2;
using StarFruit2.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace StarFruit.FromMethod
{
    class Program
    {
        static int Main(string[] args)
        {
            Console.WriteLine("Hello World!");
            return CommandSource.Create(nameof(Format)).Run(args);
        }

        /// <summary>
        /// Formats source code.
        /// </summary>
        /// <param name="folder">Folder to treat the `project` path as a folder of files.</param>
        /// <param name="files">A list of relative file or folder paths to include in formatting.All files are formatted if empty.</param>
        /// <param name="exclude">A list of relative file or folder paths to exclude from formatting.</param>
        /// <param name="check">Formats files without saving changes to disk. Terminates with a non-zero exit code if any files were formatted.</param>
        /// <param name="report">Accepts a file path, which if provided, will produce a json report in the given directory.</param>
        /// <param name="verbosity">Set the verbosity level.</param>
        /// <returns></returns>
       internal static int Format([Aliases("f")] DirectoryInfo folder,
                           [Aliases("include")] IEnumerable<FileInfo> files,
                           IEnumerable<FileInfo> exclude,
                           [Aliases("dry-run")] bool check,
                           bool report,
                           VerbosityLevel verbosity)
        {
            Console.WriteLine(@$"
Folder: {folder} 
Files: {files} 
Exclue: {exclude} 
Check: {check} 
Report: {report} 
Verbosity: {verbosity} 
Version:  ");
            return 0;
        }
    }
}
