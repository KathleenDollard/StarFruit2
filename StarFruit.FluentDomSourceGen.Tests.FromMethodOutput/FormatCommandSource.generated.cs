using StarFruit2;
using System.CommandLine;
using StarFruit2.Common;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System;
using System.Collections.Generic;
using System.IO;

namespace MethodEntryPoint
{
   public class FormatCommandSource : RootCommandSource<FormatCommandSource>
   {
      public FormatCommandSource()
      : this(null, null)
      {
      }
      public FormatCommandSource(RootCommandSource root, CommandSourceBase parent)
      : base(new Command("format", "Formats source code."), parent)
      {
         folderOption = Getfolder();
         Command.Add(folderOption);
         filesOption = Getfiles();
         Command.Add(filesOption);
         excludeOption = Getexclude();
         Command.Add(excludeOption);
         checkOption = Getcheck();
         Command.Add(checkOption);
         reportOption = Getreport();
         Command.Add(reportOption);
         verbosityOption = Getverbosity();
         Command.Add(verbosityOption);
         Command.Handler = CommandHandler.Create((InvocationContext context) =>
             {  
                CurrentCommandSource = this;
                CurrentParseResult = context.ParseResult;
                return 0;
             });
      }

      public Option<DirectoryInfo> folderOption { get; set; }
      public Option<IEnumerable<FileInfo>> filesOption { get; set; }
      public Option<IEnumerable<FileInfo>> excludeOption { get; set; }
      public Option<bool> checkOption { get; set; }
      public Option<bool> reportOption { get; set; }
      public Option<VerbosityLevel> verbosityOption { get; set; }

      public Option<DirectoryInfo> Getfolder()
      {
         Option<DirectoryInfo> option = new Option<DirectoryInfo>("--folder");
         option.Description = "Folder Whether to treat the `project` path as a folder of files.";
         option.IsRequired = false;
         option.IsHidden = false;
         option.AddAlias("-f");
         return option;
      }
      public Option<IEnumerable<FileInfo>> Getfiles()
      {
         Option<IEnumerable<FileInfo>> option = new Option<IEnumerable<FileInfo>>("--files");
         option.Description = "A list of relative file or folder paths to include in formatting.All files are formatted if empty.";
         option.IsRequired = false;
         option.IsHidden = false;
         option.AddAlias("-include");
         return option;
      }
      public Option<IEnumerable<FileInfo>> Getexclude()
      {
         Option<IEnumerable<FileInfo>> option = new Option<IEnumerable<FileInfo>>("--exclude");
         option.Description = "A list of relative file or folder paths to exclude from formatting.";
         option.IsRequired = false;
         option.IsHidden = false;
         return option;
      }
      public Option<bool> Getcheck()
      {
         Option<bool> option = new Option<bool>("--check");
         option.Description = "Formats files without saving changes to disk. Terminates with a non-zero exit code if any files were formatted.";
         option.IsRequired = false;
         option.IsHidden = false;
         option.AddAlias("-dry-run");
         return option;
      }
      public Option<bool> Getreport()
      {
         Option<bool> option = new Option<bool>("--report");
         option.Description = "Accepts a file path, which if provided, will produce a json report in the given directory.";
         option.IsRequired = false;
         option.IsHidden = false;
         return option;
      }
      public Option<VerbosityLevel> Getverbosity()
      {
         Option<VerbosityLevel> option = new Option<VerbosityLevel>("--verbosity");
         option.Description = "Set the verbosity level.";
         option.IsRequired = false;
         option.IsHidden = false;
         return option;
      }
   }
}
