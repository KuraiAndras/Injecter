using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
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

    [Parameter] readonly bool DeterministicSourcePaths;

    [Solution] readonly Solution Solution;

    Lazy<ImmutableArray<Project>> TestProjects => new(() =>
        Solution
            .AllProjects
            .Where(p => p.Name.EndsWith(".Tests"))
            .ToImmutableArray());

    Lazy<ImmutableArray<Project>> SampleProjects => new(() =>
        Solution
            .AllProjects
            .Where(p => p.Path.ToString().Contains("Samples"))
            .Except(TestProjects.Value)
            .ToImmutableArray());

    Lazy<ImmutableArray<Project>> PackageProjects => new(() =>
        Solution
            .AllProjects
            .Except(TestProjects.Value)
            .Except(SampleProjects.Value)
            .ToImmutableArray());

    Target Restore => _ => _
        .Executes(() =>
            NuGetRestore(s => s.SetTargetPath(Solution.Path)));

    Target BuildTests => _ => _
        .DependsOn(Restore)
        .Executes(() => BuildWithAppropriateToolChain(TestProjects.Value));

    // ReSharper disable once UnusedMember.Local
    Target BuildSamples => _ => _
        .DependsOn(Restore)
        .DependentFor(Test)
        .Executes(() => BuildWithAppropriateToolChain(SampleProjects.Value));

    Target BuildPackages => _ => _
        .DependsOn(Restore)
        .DependentFor(Test)
        .Executes(() => BuildWithAppropriateToolChain(PackageProjects.Value));

    Target Test => _ => _
        .DependsOn(BuildTests)
        .Executes(() => TestProjects.Value
            .SelectMany(p =>
                DotNetTest(s => s
                    .SetProjectFile(p)
                    .EnableNoBuild()
                    .SetConfiguration(Configuration.Release)
                    .EnableCollectCoverage()
                    .SetCoverletOutputFormat(CoverletOutputFormat.opencover)))
            .ToImmutableArray());

    ImmutableArray<Output> BuildWithAppropriateToolChain(ImmutableArray<Project> projects)
    {
        var deterministicSourcePaths = $"/p:DeterministicSourcePaths={DeterministicSourcePaths.ToString().ToLower()}";

        var uwpProjects = projects
            .Where(p =>
                p.Name.Contains("UWP", StringComparison.InvariantCultureIgnoreCase)
                || p.Name.Contains("Droid", StringComparison.InvariantCultureIgnoreCase))
            .ToImmutableArray();

        var msbuildProjects = uwpProjects
            .SelectMany(p =>
                MSBuild(s => s
                    .SetProjectFile(p)
                    .SetConfiguration(Configuration.Release)
                    .SetWarningsAsErrors()
                    .SetVerbosity(MSBuildVerbosity.Minimal)
                    .SetProcessArgumentConfigurator(a => a.Add(deterministicSourcePaths))));

        var dotnetProjects = projects
            .Except(uwpProjects)
            .SelectMany(p =>
                DotNetBuild(s => s
                    .SetProjectFile(p)
                    .SetConfiguration(Configuration.Release)
                    .SetWarningsAsErrors()
                    .SetProcessArgumentConfigurator(a => a.Add(deterministicSourcePaths))));

        return Enumerable.Empty<Output>()
            .Concat(dotnetProjects)
            .Concat(msbuildProjects)
            .ToImmutableArray();
    }
}
