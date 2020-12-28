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
   public class DotnetFormatCommandSourceResult : CommandSourceResult<DotnetFormat>
   {
      public DotnetFormatCommandSourceResult(ParseResult parseResult, DotnetFormatCommandSource commandSource, int exitCode)
      : base(parseResult, (commandSource.ParentCommandSource as CommandSource), exitCode)
      {
         FolderOption_Result = CommandSourceMemberResult.Create(commandSource.FolderOption, parseResult);
         FilesOption_Result = CommandSourceMemberResult.Create(commandSource.FilesOption, parseResult);
         ExcludeOption_Result = CommandSourceMemberResult.Create(commandSource.ExcludeOption, parseResult);
         CheckOption_Result = CommandSourceMemberResult.Create(commandSource.CheckOption, parseResult);
         ReportOption_Result = CommandSourceMemberResult.Create(commandSource.ReportOption, parseResult);
         VerbosityOption_Result = CommandSourceMemberResult.Create(commandSource.VerbosityOption, parseResult);
      }
      public CommandSourceMemberResult<DirectoryInfo> FolderOption_Result { get; set; }
      public CommandSourceMemberResult<IEnumerable<FileInfo>> FilesOption_Result { get; set; }
      public CommandSourceMemberResult<IEnumerable<FileInfo>> ExcludeOption_Result { get; set; }
      public CommandSourceMemberResult<bool> CheckOption_Result { get; set; }
      public CommandSourceMemberResult<bool> ReportOption_Result { get; set; }
      public CommandSourceMemberResult<VerbosityLevel> VerbosityOption_Result { get; set; }

      public override DotnetFormat CreateInstance()
      {
         var newItem = new DotnetFormat();
         newItem.Folder = FolderOption_Result.Value;
         newItem.Files = FilesOption_Result.Value;
         newItem.Exclude = ExcludeOption_Result.Value;
         newItem.Check = CheckOption_Result.Value;
         newItem.Report = ReportOption_Result.Value;
         newItem.Verbosity = VerbosityOption_Result.Value;
         return newItem;
      }
   }
   public class InvokeCommandSourceResult : DotnetFormatCommandSourceResult
   {
      public InvokeCommandSourceResult(ParseResult parseResult, InvokeCommandSource commandSource, int exitCode)
      : base(parseResult, (commandSource.ParentCommandSource as DotnetFormatCommandSource), exitCode)
      {
      }

   }
}
