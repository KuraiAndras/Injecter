using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DocFX;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Tools.NuGet;
using System;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using static Nuke.Common.Tools.DocFX.DocFXTasks;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;
using static Nuke.Common.Tools.NuGet.NuGetTasks;
using static System.IO.Directory;

[ShutdownDotNetAfterServerBuild]
sealed class Build : NukeBuild
{
    public static int Main() => Execute<Build>(x => x.Test);

    const string DocFxJsonPath = "Documentation/docfx.json";

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly Configuration Configuration = IsLocalBuild ? Configuration.Debug : Configuration.Release;

    [Parameter] readonly string NugetApiUrl = "https://api.nuget.org/v3/index.json";
    [Parameter] readonly string NugetApiKey = string.Empty;

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
                    .SetNoBuild(true)
                    .SetConfiguration(Configuration.Release)
                    .SetCollectCoverage(true)
                    .SetCoverletOutputFormat(CoverletOutputFormat.opencover)))
            .ToImmutableArray());

    Target PushToNuGet => _ => _
        .DependsOn(BuildPackages)
        .Requires(() => NugetApiUrl)
        .Requires(() => NugetApiKey)
        .Requires(() => Configuration.Equals(Configuration.Release))
        .Executes(() =>
            EnumerateFiles(Solution.Directory!, "*.nupkg", SearchOption.AllDirectories)
                .Where(n => !n.EndsWith("symbols.nupkg"))
                .SelectMany(x =>
                    DotNetNuGetPush(s => s
                        .SetTargetPath(x)
                        .SetSource(NugetApiUrl)
                        .SetApiKey(NugetApiKey)))
                .ToImmutableArray());

    Target CreateMetadata => _ => _
        .Executes(() => DocFX($"metadata {DocFxJsonPath}"));

    Target BuildDocs => _ => _
        .DependsOn(CreateMetadata)
        .Executes(() => DocFX($"build {DocFxJsonPath}"));

    Target ServeDocs => _ => _
        .Executes(() => DocFX($"{DocFxJsonPath} --serve"));

    static ImmutableArray<Output> BuildWithAppropriateToolChain(ImmutableArray<Project> projects)
    {
        var uwpProjects = projects
            .Where(p => p.Name.Contains("UWP", StringComparison.InvariantCultureIgnoreCase))
            .ToImmutableArray();

        var buildUwp = uwpProjects
            .SelectMany(p =>
                MSBuild(s => s
                    .SetProjectFile(p)
                    .SetConfiguration(Configuration.Release)
                    .SetWarningsAsErrors()
                    .SetVerbosity(MSBuildVerbosity.Minimal)));

        var buildOthers = projects
            .Except(uwpProjects)
            .SelectMany(p =>
                DotNetBuild(s => s
                    .SetProjectFile(p)
                    .SetConfiguration(Configuration.Release)
                    .SetWarningsAsErrors()));

        return Enumerable.Empty<Output>()
            .Concat(buildOthers)
            .Concat(buildUwp)
            .ToImmutableArray();
    }
}
