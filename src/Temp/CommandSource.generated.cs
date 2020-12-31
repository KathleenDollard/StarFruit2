using StarFruit2;
using System.CommandLine;
using StarFruit2.Common;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;
using System;
using System.Collections.Generic;
using System.IO;

namespace SingleLayerCli
{
   public class DotnetFormatCommandSource : RootCommandSource<DotnetFormatCommandSource>
   {
      public DotnetFormatCommandSource()
      : this(null, null)
      {
      }
      public DotnetFormatCommandSource(RootCommandSource root, CommandSourceBase parent)
      : base(new Command("dotnet-format", ""), parent)
      {
         FolderOption = GetFolder();
         Command.Add(FolderOption);
         FilesOption = GetFiles();
         Command.Add(FilesOption);
         ExcludeOption = GetExclude();
         Command.Add(ExcludeOption);
         CheckOption = GetCheck();
         Command.Add(CheckOption);
         ReportOption = GetReport();
         Command.Add(ReportOption);
         VerbosityOption = GetVerbosity();
         Command.Add(VerbosityOption);
         InvokeCommand = new InvokeCommandSource(this, this);
         Command.AddCommand(InvokeCommand.Command);
         Command.Handler = CommandHandler.Create((InvocationContext context) =>
             {  
                CurrentCommandSource = this;
                CurrentParseResult = context.ParseResult;
                return 0;
             });
      }

      public Option<DirectoryInfo> FolderOption { get; set; }
      public Option<IEnumerable<FileInfo>> FilesOption { get; set; }
      public Option<IEnumerable<FileInfo>> ExcludeOption { get; set; }
      public Option<bool> CheckOption { get; set; }
      public Option<bool> ReportOption { get; set; }
      public Option<VerbosityLevel> VerbosityOption { get; set; }
      public InvokeCommandSource InvokeCommand { get; set; }

      public Option<DirectoryInfo> GetFolder()
      {
         Option<DirectoryInfo> option = new Option<DirectoryInfo>("--folder");
         option.Description = "";
         option.IsRequired = false;
         option.IsHidden = false;
         option.AddAlias("-f");
         return option;
      }
      public Option<IEnumerable<FileInfo>> GetFiles()
      {
         Option<IEnumerable<FileInfo>> option = new Option<IEnumerable<FileInfo>>("--files");
         option.Description = "A list of relative file or folder paths to include in formatting.All files are formatted if empty.";
         option.IsRequired = false;
         option.IsHidden = false;
         option.AddAlias("-include");
         return option;
      }
      public Option<IEnumerable<FileInfo>> GetExclude()
      {
         Option<IEnumerable<FileInfo>> option = new Option<IEnumerable<FileInfo>>("--exclude");
         option.Description = "A list of relative file or folder paths to exclude from formatting.";
         option.IsRequired = false;
         option.IsHidden = false;
         return option;
      }
      public Option<bool> GetCheck()
      {
         Option<bool> option = new Option<bool>("--check");
         option.Description = "Formats files without saving changes to disk. Terminates with a non-zero exit code if any files were formatted.";
         option.IsRequired = false;
         option.IsHidden = false;
         option.AddAlias("-dry-run");
         return option;
      }
      public Option<bool> GetReport()
      {
         Option<bool> option = new Option<bool>("--report");
         option.Description = "Accepts a file path, which if provided, will produce a json report in the given directory.";
         option.IsRequired = false;
         option.IsHidden = false;
         return option;
      }
      public Option<VerbosityLevel> GetVerbosity()
      {
         Option<VerbosityLevel> option = new Option<VerbosityLevel>("--verbosity");
         option.Description = "Set the verbosity level.";
         option.IsRequired = false;
         option.IsHidden = false;
         option.AddAlias("-v");
         return option;
      }
   }
   public class InvokeCommandSource : CommandSourceBase
   {
      public InvokeCommandSource(RootCommandSource root, CommandSourceBase parent)
      : base(new Command("invoke", ""), parent)
      {
         Command.Handler = CommandHandler.Create(() =>
             {  
                root.CurrentCommandSource = this;
                return 0;
             });
      }


   }
}
