using StarFruit2.Common;
using System;
using System.Collections.Generic;
using System.CommandLine;
using System.IO;
using System.Text;

namespace StarFruit2.StarFruitSurface
{

    public partial class SampleTwoLayerCli : CliBase
    {
        public DirectoryInfo StartPathArg { get; set; }

        public VerbosityLevel Verbosity { get; set; }

        public int Find()
                => ManageGlobalJsonImplementation.Find(StartPathArg, Verbosity);

        public int List(FileInfo Output)
            => ManageGlobalJsonImplementation.List(StartPathArg, Verbosity, Output);

        public int Update(FileInfo FilePathArg, string OldVersion, string NewVersion, bool AllowPrerelease, RollForward RollForward)
            => ManageGlobalJsonImplementation.Update(StartPathArg, Verbosity, FilePathArg, OldVersion, NewVersion, AllowPrerelease, RollForward);

        public int Check(bool SdkOnly)
            => ManageGlobalJsonImplementation.Check(StartPathArg, Verbosity);

    }

    public enum RollForward
    {
        Patch,   // Uses the specified version. If not found, rolls forward to the latest patch level. If not found, fails. This value is the legacy behavior from the earlier versions of the SDK.
        Feature, // Uses the latest patch level for the specified major, minor, and feature band. If not found, rolls forward to the next higher feature band within the same major/minor and uses the latest patch level for that feature band. If not found, fails.
        Minor,  // Uses the latest patch level for the specified major, minor, and feature band. If not found, rolls forward to the next higher feature band within the same major/minor version and uses the latest patch level for that feature band. If not found, rolls forward to the next higher minor and feature band within the same major and uses the latest patch level for that feature band. If not found, fails.
        Major,  // Uses the latest patch level for the specified major, minor, and feature band. If not found, rolls forward to the next higher feature band within the same major/minor version and uses the latest patch level for that feature band. If not found, rolls forward to the next higher minor and feature band within the same major and uses the latest patch level for that feature band. If not found, rolls forward to the next higher major, minor, and feature band and uses the latest patch level for that feature band. If not found, fails.
        LatestPatch, // Uses the latest installed patch level that matches the requested major, minor, and feature band with a patch level and that is greater or equal than the specified value. If not found, fails.
        LatestFeature,//  Uses the highest installed feature band and patch level that matches the requested major and minor with a feature band that is greater or equal than the specified value. If not found, fails.
        LatestMinor, // Uses the highest installed minor, feature band, and patch level that matches the requested major with a minor that is greater or equal than the specified value. If not found, fails.
        LatestMajor, // Uses the highest installed .NET Core SDK with a major that is greater or equal than the specified value. If not found, fail.
        Disable, // Doesn't roll forward. Exact match required.
    }

    public class ManageGlobalJsonImplementation
    {
        public static int Find(DirectoryInfo startPathArg, VerbosityLevel verbosity)
        {
            Console.WriteLine($"Run Find from {startPathArg} with verbosity {verbosity}");
            return 0;
        }

        public static int List(DirectoryInfo startPathArg, VerbosityLevel verbosity, FileInfo output)
        {
            Console.WriteLine($"Run List from {startPathArg} with verbosity {verbosity}");
            return 0;
        }

        public static int Update(DirectoryInfo startPathArg,
                                 VerbosityLevel verbosity,
                                 FileInfo filePath,
                                 string oldVersion,
                                 string newVersion,
                                 bool allowPrerelease,
                                 RollForward rollForward)
        {
            var prereleaseText = allowPrerelease ? "allowing prerelease" : "disallowing prerelease";
            Console.WriteLine(@$"Run Update from {startPathArg} for {filePath} changing '{oldVersion}' to '{newVersion}' " +
                                    $"{prereleaseText} and rollforward to {rollForward} with verbosity {verbosity} ");
            return 7;
        }

        public static int Check(DirectoryInfo startPathArg, VerbosityLevel verbosity)
        {
            Console.WriteLine($"Run Check from {startPathArg} with verbosity {verbosity}");
            return 0;
        }
    }



    public partial class SampleTwoLayerCli
    {
        protected override Command CreateCommand(string methodName)
        {
            throw new NotImplementedException();
        }

        protected override Command CreateCommand(Type commandType)
        {
            throw new NotImplementedException();
        }
    }

}
