using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NuGet;
using Nuke.Common.Utilities.Collections;
using System;
using System.IO;
using System.Linq;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.NuGet.NuGetTasks;
using static System.IO.Directory;

[CheckBuildProjectConfigurations]
[ShutdownDotNetAfterServerBuild]
sealed class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Test);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter] readonly string NugetApiUrl = "https://api.nuget.org/v3/index.json";
    [Parameter] readonly string NugetApiKey = string.Empty;

    [Solution] readonly Solution Solution;

    Target Restore => _ => _
        .Executes(() =>
            NuGetRestore(s => s.SetTargetPath(Solution.Path)));

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
        {
            var projectsToBuild = Solution.AllProjects
                .Where(p => !p.Path.ToString().Contains("Samples"))
                .ToArray();

            var uwpProjects = projectsToBuild
                .Where(p => p.Name.Contains("UWP", StringComparison.InvariantCultureIgnoreCase))
                .ToArray();

            var buildUwp = uwpProjects
                .Select(p =>
                    MSBuild(s => s
                        .SetProjectFile(p)
                        .SetConfiguration(Configuration.Release)
                        .SetWarningsAsErrors()
                        .SetVerbosity(MSBuildVerbosity.Minimal)))
                .ToList();

            var buildOthers = projectsToBuild
                .Except(uwpProjects)
                .Select(p =>
                    DotNetBuild(s => s
                        .SetProjectFile(p)
                        .SetConfiguration(Configuration.Release)
                        .SetWarningsAsErrors()))
                .ToList();

            return Enumerable.Empty<Output>()
                .Concat(buildUwp)
                .Concat(buildOthers)
                .ToArray();
        });

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() =>
            EnumerateFiles(Solution.Directory!, "*Tests.csproj", SearchOption.AllDirectories)
                .Select(t =>
                    DotNetTest(s => s
                        .SetProjectFile(t)
                        .SetNoBuild(true)
                        .SetConfiguration(Configuration.Release)
                        .SetCollectCoverage(true)
                        .SetCoverletOutputFormat(CoverletOutputFormat.opencover)))
                .ToArray());

    Target PushToNuGet => _ => _
        .DependsOn(Compile)
        .Requires(() => NugetApiUrl)
        .Requires(() => NugetApiKey)
        .Requires(() => Configuration.Equals(Configuration.Release))
        .Executes(() =>
            EnumerateFiles(Solution.Directory!, "*.nupkg", SearchOption.AllDirectories)
                .Where(n => !n.EndsWith("symbols.nupkg"))
                .Select(x =>
                    DotNetNuGetPush(s => s
                        .SetTargetPath(x)
                        .SetSource(NugetApiUrl)
                        .SetApiKey(NugetApiKey)))
                .ToArray());
}
