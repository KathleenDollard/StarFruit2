using StarFruit2;
using StarFruit2.Common;
using System.CommandLine.Parsing;
using System.Threading.Tasks;

namespace TwoLayerCli
{
    // Abstract because there is no invoke
    public abstract class CliRootCommandSourceResult : CommandSourceResult<CliRoot>
    {

        protected CliRootCommandSourceResult(ParseResult parseResult,
                                             CommandSourceMemberResult<string> stringProperty_Result,
                                             CommandSourceMemberResult<bool> ctorParam_Result,
                                             int exitCode)
            : base(parseResult, exitCode)
        {
            StringProperty_Result = stringProperty_Result;
            CtorParam_Result = ctorParam_Result;
        }


        public CommandSourceMemberResult<string> StringProperty_Result { get; set; }

        public CommandSourceMemberResult<bool> CtorParam_Result { get; set; }

        public override CliRoot CreateInstance()
        {
            return new CliRoot(CtorParam_Result.Value)
            {
                StringProperty = StringProperty_Result.Value,
            };
        }
    }

    public class FindCommandSourceResult : CliRootCommandSourceResult
    {
        public FindCommandSourceResult(ParseResult parseResult,
                                       FindCommandSource findCommandSource,
                                       int exitCode)
            : base(parseResult,
                   CommandSourceMemberResult.Create(findCommandSource.StringProperty, parseResult),
                   CommandSourceMemberResult.Create(findCommandSource.CtorParam, parseResult),
                   exitCode)
        {
            IntArg_Result = CommandSourceMemberResult.Create(findCommandSource.IntArg, parseResult);
            StringOption_Result = CommandSourceMemberResult.Create(findCommandSource.StringOption, parseResult);
            BoolOption_Result = CommandSourceMemberResult.Create(findCommandSource.BoolOption, parseResult);
        }

        public CommandSourceMemberResult<int> IntArg_Result { get; set; }
        public CommandSourceMemberResult<string> StringOption_Result { get; set; }
        public CommandSourceMemberResult<bool> BoolOption_Result { get; set; }

        public async override Task<int> RunAsync()
        {
            return await CreateInstance().FindAsync(StringOption_Result.Value, BoolOption_Result.Value, IntArg_Result.Value);
        }

    }

    public class ListCommandSourceResult : CliRootCommandSourceResult
    {
        public ListCommandSourceResult(ParseResult parseResult,
                                       ListCommandSource listCommandSource,
                                       int exitCode)
            : base(parseResult,
                   CommandSourceMemberResult.Create(listCommandSource.StringProperty, parseResult),
                   CommandSourceMemberResult.Create(listCommandSource.CtorParam, parseResult),
                   exitCode)
        {
            VerbosityOption_Result = CommandSourceMemberResult.Create(listCommandSource.VerbosityOption, parseResult);
        }

        public CommandSourceMemberResult<VerbosityLevel> VerbosityOption_Result { get; set; }

        public async override Task<int> RunAsync()
        {
            return await CreateInstance().ListAsync(VerbosityOption_Result.Value);
        }
    }
}
