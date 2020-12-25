using StarFruit2;
using StarFruit2.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace SingleLayerCli
{
    public class DotnetFormat
    {
        [Aliases("f")]
            // <folder>
        [Description("Folder Whether to treat the `<project>` path as a folder of files.")]
        public DirectoryInfo Folder { get; set; }

        [Aliases("include")]
        // <include>
        [Description("A list of relative file or folder paths to include in formatting.All files are formatted if empty.")]
        public IEnumerable<FileInfo> Files { get; set; }


        // <exclude>
        [Description("A list of relative file or folder paths to exclude from formatting.")]
        public IEnumerable<FileInfo> Exclude { get; set; }

        [Aliases("dry-run")]
        [Description("Formats files without saving changes to disk. Terminates with a non-zero exit code if any files were formatted.")]
        public bool Check { get; set; }


        // <report>
        [Description("Accepts a file path, which if provided, will produce a json report in the given directory.")]
        public bool Report { get; set; }


        [Aliases("v")]
        // <verbosity>
        [Description("Set the verbosity level.")]
        public VerbosityLevel Verbosity { get; set; }

        public int Invoke(int intArg, string stringOption, bool boolOption)
        {
            Console.WriteLine(@$"Folder: {Folder} 
Files: {Files} 
Exclue: {Exclude} 
Check: {Check} 
Report: {Report} 
Verbosity: {Verbosity} 
Version:  ");
            return 0; }

    }
}
