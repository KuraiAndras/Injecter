using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NuGet;
using System;
using System.Collections.Immutable;
using System.Linq;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.NuGet.NuGetTasks;

[ShutdownDotNetAfterServerBuild]
sealed partial class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Test);

    const string DocFxJsonPath = "Documentation/docfx.json";

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter("Are we running inside a CI job?")]
    readonly bool IsCi = false;

    [Solution] readonly Solution Solution;

    Lazy<ImmutableArray<Project>> TestProjects => new(() =>
        Solution
            .AllProjects
            .Where(p => p.Name.EndsWith(".Tests"))
            .ToImmutableArray());

    Target Restore => _ => _
        .Executes(() =>
            NuGetRestore(s => s.SetTargetPath(Solution.Path)));

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() =>
            MSBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration.Release)
                .SetWarningsAsErrors()
                .SetVerbosity(MSBuildVerbosity.Minimal)));

    Target Test => _ => _
        .DependsOn(Compile)
        .Executes(() => TestProjects.Value
            .SelectMany(p =>
                DotNetTest(s => s
                    .SetProjectFile(p)
                    .EnableNoBuild()
                    .SetConfiguration(Configuration.Release)
                    .EnableCollectCoverage()
                    .SetCoverletOutputFormat(CoverletOutputFormat.opencover)))
            .ToImmutableArray());
}
