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
         Option<DirectoryInfo> opt = new Option<DirectoryInfo>("--folder");
         opt.Description = "";
         opt.IsRequired = false;
         opt.IsHidden = false;
         opt.AddAlias("-f");
         return opt;
      }
      public Option<IEnumerable<FileInfo>> GetFiles()
      {
         Option<IEnumerable<FileInfo>> opt = new Option<IEnumerable<FileInfo>>("--files");
         opt.Description = "A list of relative file or folder paths to include in formatting.All files are formatted if empty.";
         opt.IsRequired = false;
         opt.IsHidden = false;
         opt.AddAlias("-include");
         return opt;
      }
      public Option<IEnumerable<FileInfo>> GetExclude()
      {
         Option<IEnumerable<FileInfo>> opt = new Option<IEnumerable<FileInfo>>("--exclude");
         opt.Description = "A list of relative file or folder paths to exclude from formatting.";
         opt.IsRequired = false;
         opt.IsHidden = false;
         return opt;
      }
      public Option<bool> GetCheck()
      {
         Option<bool> opt = new Option<bool>("--check");
         opt.Description = "Formats files without saving changes to disk. Terminates with a non-zero exit code if any files were formatted.";
         opt.IsRequired = false;
         opt.IsHidden = false;
         opt.AddAlias("-dry-run");
         return opt;
      }
      public Option<bool> GetReport()
      {
         Option<bool> opt = new Option<bool>("--report");
         opt.Description = "Accepts a file path, which if provided, will produce a json report in the given directory.";
         opt.IsRequired = false;
         opt.IsHidden = false;
         return opt;
      }
      public Option<VerbosityLevel> GetVerbosity()
      {
         Option<VerbosityLevel> opt = new Option<VerbosityLevel>("--verbosity");
         opt.Description = "Set the verbosity level.";
         opt.IsRequired = false;
         opt.IsHidden = false;
         opt.AddAlias("-v");
         return opt;
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
