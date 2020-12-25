using StarFruit2;
using System.CommandLine;
using StarFruit2.Common;
using System.CommandLine.Invocation;
using System.CommandLine.Parsing;

namespace TwoLayerCli
{
   public class CliRootCommandSourceResult : CommandSourceResult<CliRoot>
   {
      public CliRootCommandSourceResult(ParseResult parseResult, CliRootCommandSource commandSource, int exitCode)
      : base(parseResult, (commandSource.ParentCommandSource as CommandSource), exitCode)
      {
         StringPropertyOption_Result = CommandSourceMemberResult.Create(commandSource.StringPropertyOption, parseResult);
         ctorParamOption_Result = CommandSourceMemberResult.Create(commandSource.ctorParamOption, parseResult);
      }
      public CommandSourceMemberResult<string> StringPropertyOption_Result { get; set; }
      public CommandSourceMemberResult<bool> ctorParamOption_Result { get; set; }

      public override CliRoot CreateInstance()
      {
         var newItem = new CliRoot(ctorParamOption_Result.Value);
         newItem.StringProperty = StringPropertyOption_Result.Value;
         return newItem;
      }
   }
   public class FindCommandSourceResult : CliRootCommandSourceResult
   {
      public FindCommandSourceResult(ParseResult parseResult, FindCommandSource commandSource, int exitCode)
      : base(parseResult, (commandSource.ParentCommandSource as CliRootCommandSource), exitCode)
      {
         intArgArgument_Result = CommandSourceMemberResult.Create(commandSource.intArgArgument, parseResult);
         stringOptionOption_Result = CommandSourceMemberResult.Create(commandSource.stringOptionOption, parseResult);
         boolOptionOption_Result = CommandSourceMemberResult.Create(commandSource.boolOptionOption, parseResult);
      }
      public CommandSourceMemberResult<int> intArgArgument_Result { get; set; }
      public CommandSourceMemberResult<string> stringOptionOption_Result { get; set; }
      public CommandSourceMemberResult<bool> boolOptionOption_Result { get; set; }

   }
   public class ListCommandSourceResult : CliRootCommandSourceResult
   {
      public ListCommandSourceResult(ParseResult parseResult, ListCommandSource commandSource, int exitCode)
      : base(parseResult, (commandSource.ParentCommandSource as CliRootCommandSource), exitCode)
      {
         verbosityOption_Result = CommandSourceMemberResult.Create(commandSource.verbosityOption, parseResult);
      }
      public CommandSourceMemberResult<VerbosityLevel> verbosityOption_Result { get; set; }

   }
}
