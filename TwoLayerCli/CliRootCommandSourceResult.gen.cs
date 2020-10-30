using StarFruit2;
using StarFruit2.Common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace TwoLayerCli
{
    // Abstract because there is no invoke
    public abstract class CliRootCommandSourceResult : CommandSourceResult<CliRoot>
    {
        private CliRoot? instance;

        protected CliRootCommandSourceResult(CommandSourceMemberResult<string> stringProperty_Result,
                                             CommandSourceMemberResult<bool> ctorParam_Result)
        {
            StringProperty_Result = stringProperty_Result;
            CtorParam_Result = ctorParam_Result;
        }

        // This needs work because you get null unless you've set instance
        public CliRoot Instance
        {
            get
            {
                if (instance is null)
                {
                    instance = MakeNewInstance();
                }
                return instance;
            }
            set => instance = value;
        }

        public CommandSourceMemberResult<string> StringProperty_Result { get; set; }

        public CommandSourceMemberResult<bool> CtorParam_Result { get; set; }

        public override CliRoot MakeNewInstance()
        {
            Instance = new CliRoot(CtorParam_Result.Value)
            {
                StringProperty = StringProperty_Result.Value,
            };
            return Instance;
        }
    }

    public class FindCommandSourceResult : CliRootCommandSourceResult
    {
        public FindCommandSourceResult(CommandSourceMemberResult<int> intArg_Result,
                                       CommandSourceMemberResult<string> stringOption_Result,
                                       CommandSourceMemberResult<bool> boolOption_Result,
                                       CommandSourceMemberResult<string> stringProperty_Result,
                                       CommandSourceMemberResult<bool> ctorParam_Result)
            : base(stringProperty_Result, ctorParam_Result)
        {
            IntArg_Result = intArg_Result;
            StringOption_Result = stringOption_Result;
            BoolOption_Result = boolOption_Result;
        }

        public CommandSourceMemberResult<int> IntArg_Result { get; set; }
        public CommandSourceMemberResult<string> StringOption_Result { get; set; }
        public CommandSourceMemberResult<bool> BoolOption_Result { get; set; }

        public async override Task<int> RunAsync()
        {
            return await Instance.FindAsync(StringOption_Result.Value, BoolOption_Result.Value, IntArg_Result.Value);
        }

    }
  
    public class ListCommandSourceResult : CliRootCommandSourceResult
    {
        public ListCommandSourceResult(CommandSourceMemberResult<VerbosityLevel> verbosityOption_Result,
                                       CommandSourceMemberResult<string> stringProperty_Result,
                                       CommandSourceMemberResult<bool> ctorParam_Result)
             : base(stringProperty_Result, ctorParam_Result)
        {
            VerbosityOption_Result = verbosityOption_Result;
        }

        public CommandSourceMemberResult<VerbosityLevel> VerbosityOption_Result { get; set; }

        public async override Task<int> RunAsync()
        {
            return await Instance.ListAsync(VerbosityOption_Result.Value);
        }
    }
}
