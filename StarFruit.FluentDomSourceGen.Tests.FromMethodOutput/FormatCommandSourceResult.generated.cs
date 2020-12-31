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
   public class FormatCommandSourceResult : CommandSourceResult
   {
      public FormatCommandSourceResult(ParseResult parseResult, FormatCommandSource commandSource, int exitCode)
      : base(parseResult, (commandSource.ParentCommandSource as CommandSourceBase), exitCode)
      {
         folderOption_Result = CommandSourceMemberResult.Create(commandSource.folderOption, parseResult);
         filesOption_Result = CommandSourceMemberResult.Create(commandSource.filesOption, parseResult);
         excludeOption_Result = CommandSourceMemberResult.Create(commandSource.excludeOption, parseResult);
         checkOption_Result = CommandSourceMemberResult.Create(commandSource.checkOption, parseResult);
         reportOption_Result = CommandSourceMemberResult.Create(commandSource.reportOption, parseResult);
         verbosityOption_Result = CommandSourceMemberResult.Create(commandSource.verbosityOption, parseResult);
      }
      public CommandSourceMemberResult<DirectoryInfo> folderOption_Result { get; set; }
      public CommandSourceMemberResult<IEnumerable<FileInfo>> filesOption_Result { get; set; }
      public CommandSourceMemberResult<IEnumerable<FileInfo>> excludeOption_Result { get; set; }
      public CommandSourceMemberResult<bool> checkOption_Result { get; set; }
      public CommandSourceMemberResult<bool> reportOption_Result { get; set; }
      public CommandSourceMemberResult<VerbosityLevel> verbosityOption_Result { get; set; }

      public override int Run()
      {
         return Program.Format(folderOption_Result.Value, filesOption_Result.Value, excludeOption_Result.Value, checkOption_Result.Value, reportOption_Result.Value, verbosityOption_Result.Value);
      }
   }
}
