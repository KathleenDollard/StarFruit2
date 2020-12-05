using FluentDom;
using StarFruit2.Common.Descriptors;
using System;
using System.Linq;
using static FluentDom.Expression;

// KAD: Extensibility
// * Use a base/derived overlad extensibility mechanism.Overriding methods may need to navigate DOM
// * Use a two phase appraoch to evaluation, maintaining Select methods as IENumerable and Funcs for extension
// * Have a separate calculate values method for each method so it can be called at any point
namespace GeneratorSupport.Tests
{
    public class GenerateCommandSourceResult
    {

        public void Can_create_code()
        {
            var cli = new CliDescriptor();
            cli.CommandDescriptor = new CommandDescriptor(null, "Fizz", null);
            var code = CreateCode(cli);
        }

        public virtual Code CreateCode(CliDescriptor cli)
        {
            throw new NotImplementedException();
        }
 
    }
}

