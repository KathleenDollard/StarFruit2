using StarFruit2;
using StarFruit2.Common;
using System;
using System.Collections.Generic;
using System.IO;

namespace SingleLayerCli
{
    public class DotnetFormat : ICliRoot
    {
        /// <summary>
        /// "Folder Whether to treat the `<project>` path as a folder of files."
        /// </summary>
        [Aliases("f")]
        // <folder>
        public DirectoryInfo Folder { get; set; }

        /// <summary>
        /// "A list of relative file or folder paths to include in formatting.All files are formatted if empty."
        /// </summary>
        [Aliases("include")]
        // <include>
        public IEnumerable<FileInfo> Files { get; set; }


        // <exclude>
        /// <summary>
        /// "A list of relative file or folder paths to exclude from formatting."
        /// </summary>
        public IEnumerable<FileInfo> Exclude { get; set; }

        /// <summary>
        /// "Formats files without saving changes to disk. Terminates with a non-zero exit code if any files were formatted."
        /// </summary>
        [Aliases("dry-run")]
        public bool Check { get; set; }


        // <report>
        /// <summary>
        /// "Accepts a file path, which if provided, will produce a json report in the given directory."
        /// </summary>
        public bool Report { get; set; }


        /// <summary>
        /// "Set the verbosity level."
        /// </summary>
        [Aliases("v")]
        // <verbosity>
        public VerbosityLevel Verbosity { get; set; }

        public int Invoke()
        {
            Console.WriteLine(@$"Folder: {Folder} 
Files: {Files} 
Exclue: {Exclude} 
Check: {Check} 
Report: {Report} 
Verbosity: {Verbosity} 
Version:  ");
            return 0;
        }

    }
}
