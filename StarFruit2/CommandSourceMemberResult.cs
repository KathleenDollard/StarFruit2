using StarFruit2.Common;
using System;
using System.CommandLine;
using System.CommandLine.Parsing;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace StarFruit2
{
    [Flags]
    public enum MemberResultValueFrom
    {
        UserEntry = 1,
        Default = 0b0000_0010,
        DefaultFromCli = 0b0000_0110,
        DefaultFromType = 0b0000_1010,
        ChangedDuringValidation = 0b0001_0000,
    }

    public class CommandSourceMemberResult
    {
        public static CommandSourceMemberResult<T> Create<T>(Option<T> option, ParseResult parseResult)
        {
            Assert.NotNull(parseResult);
            Assert.NotNull(parseResult.CommandResult);
            var symbolResult = GetSymbolResult(option, parseResult, parseResult.CommandResult);
            var optionResult = symbolResult as OptionResult;
            return optionResult is null
                ? GetDefaultSymbolResult(option)
                : new CommandSourceMemberResult<T>(optionResult.GetValueOrDefault<T>(), GetValueFrom(optionResult), option, optionResult);
        }


        public static CommandSourceMemberResult<T> Create<T>(Argument<T> argument, ParseResult parseResult)
        {
            Assert.NotNull(parseResult);
            Assert.NotNull(parseResult.CommandResult);
            var symbolResult = GetSymbolResult(argument, parseResult, parseResult.CommandResult);
            var argumentResult = symbolResult as ArgumentResult;
            return argumentResult is null
                ? GetDefaultSymbolResult(argument)
                : new CommandSourceMemberResult<T>(argumentResult.GetValueOrDefault<T>(), GetValueFrom(argumentResult), argument, argumentResult);
        }

        private static CommandSourceMemberResult<T> GetDefaultSymbolResult<T>(Option<T> option)
        {
            return option.Argument.HasDefaultValue
                         ? new CommandSourceMemberResult<T>((T)option.Argument.GetDefaultValue(), MemberResultValueFrom.DefaultFromCli, option, null)
                         : new CommandSourceMemberResult<T>(default(T), MemberResultValueFrom.DefaultFromType, option, null);
        }

        private static CommandSourceMemberResult<T> GetDefaultSymbolResult<T>(Argument<T> argument)
        {
            return argument.HasDefaultValue
                         ? new CommandSourceMemberResult<T>((T)argument.GetDefaultValue(), MemberResultValueFrom.DefaultFromCli, argument, null)
                         : new CommandSourceMemberResult<T>(default(T), MemberResultValueFrom.DefaultFromType, argument, null);
        }

        private static MemberResultValueFrom GetValueFrom(OptionResult optionResult)
        {
            return optionResult is null
                ? MemberResultValueFrom.DefaultFromType
                : optionResult.Token is null
                    ? MemberResultValueFrom.DefaultFromCli
                    : MemberResultValueFrom.UserEntry;
        }


        private static MemberResultValueFrom GetValueFrom(ArgumentResult argumentResult)
        {
            return argumentResult is null
                ? MemberResultValueFrom.DefaultFromType
                : !argumentResult.Tokens.Any()
                    ? MemberResultValueFrom.DefaultFromCli
                    : MemberResultValueFrom.UserEntry;
        }

        private static SymbolResult GetSymbolResult<T>(Option<T> option, ParseResult parseResult, CommandResult? commmandResult)
        {
            SymbolResult? symbolResult = null;
            while (!(commmandResult is null) && symbolResult is null)
            {
                symbolResult = parseResult.RootCommandResult.FindResultFor(option);
                commmandResult = commmandResult.Parent as CommandResult;
            }

            return symbolResult;
        }

        private static SymbolResult GetSymbolResult<T>(Argument<T> argumeent, ParseResult parseResult, CommandResult? commmandResult)
        {
            SymbolResult? symbolResult = null;
            while (!(commmandResult is null) && symbolResult is null)
            {
                symbolResult = parseResult.RootCommandResult.FindResultFor(argumeent);
                commmandResult = commmandResult.Parent as CommandResult;
            }

            return symbolResult;
        }
    }

    /// <summary>
    /// </summary>
    /// <remarks>
    /// Note on default values:
    /// There are three states:
    ///  * The user entered a value for the option/argument
    ///    * A System.CommandLine Option/Argument result exists
    ///  * The user did not enter a value, but the option/argument has a default that is used
    ///    * A System.CommandLine Option/Argument result exists
    ///  * The user did not enter a value, and the option/argument does not have a default
    ///    * No System.CommandLine Option/Argument result exists
    ///    
    /// Three states seem confusing for the programmer. Which question is more likely:
    ///  * Did the user enter a value? (required for templates, but I do not mind more work there)
    ///  * Is there a System.CommandLine Option/Argument result available?
    /// To accomodate both, it is currently a Flags enum with bool accessing properties
    /// </remarks>
    /// <typeparam name="T"></typeparam>
    public class CommandSourceMemberResult<T> : CommandSourceMemberResult
    {

        public CommandSourceMemberResult([MaybeNull] T value, MemberResultValueFrom valueFrom, Symbol symbol, SymbolResult? symbolResult)
        {
            this.value = value;
            Symbol = symbol;
            SymbolResult = symbolResult;
            From = valueFrom;
        }

        // This is writeable so changes can be made during validation stage
        private T value;
        public T Value
        {
            get => value;
            set
            {
                From = MemberResultValueFrom.ChangedDuringValidation;
                this.value = value;
            }
        }
        public Symbol Symbol { get; }
        public MemberResultValueFrom From { get; private set; }
        public SymbolResult? SymbolResult { get; }

        public bool IsUserEntered => From == MemberResultValueFrom.UserEntry;
        public bool IsDefault => From.HasFlag(MemberResultValueFrom.Default);
        public bool IsTypeDefault => From == MemberResultValueFrom.DefaultFromType;
        public bool IsChangedDuringValidation => From == MemberResultValueFrom.DefaultFromType;

    }




    //public class CommandSourceMemberEnteredResult<T> : CommandSourceMemberResult<T>
    //{
    //    public CommandSourceMemberEnteredResult(T value, SymbolResult symbolResult)
    //        : base(value)
    //    {
    //        SymbolResult = symbolResult;
    //    }

    //}

    //public class CommandSourceMemberDefaultResult<T> : CommandSourceMemberResult<T>
    //{
    //    public CommandSourceMemberDefaultResult(T value)
    //          : base(value)
    //    { }
    //}
}
