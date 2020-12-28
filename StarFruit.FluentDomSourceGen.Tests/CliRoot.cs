using StarFruit2;
using StarFruit2.Common;
using System;
using System.IO;

namespace TwoLayerCli
{
    /// <summary>
    /// This is the entry point, the end user types the executable name
    /// </summary>
    public partial class CliRoot : ICliRoot
    {
        /// <summary> </summary>
        /// <param name="ctorParam">This is a constructor parameter</param>
        public CliRoot(bool ctorParam) { }

        /// <summary>
        /// This is a string property
        /// </summary>
        [Aliases("o")]
        [Required]
        public string StringProperty { get; set; } = "abc";

        /// <summary>
        /// Use this to find things
        /// </summary>
        /// <param name="stringOption">This is an string argument</param>
        /// <param name="boolOption">This is an bool argument</param>
        /// <param name="intArg">This is an integer argument</param>
        /// <returns></returns>
        public int Find(string stringOption, bool boolOption, int intArg = 42)
        {
            Console.WriteLine($"StringProperty: {StringProperty}   stringOption: {stringOption}   boolOption: {boolOption}    intArg: {intArg}");
            return  0;

        }

        /// <summary>
        /// List the elements you are interested in
        /// </summary>
        /// <param name="verbosity">The degree of detail desired</param>
        /// <returns></returns>
        public int List(VerbosityLevel verbosity)
        {
            Console.WriteLine($"Verbosity: {verbosity}");          
            return 0; }
    }
}
