using Nuke.Common;
using Nuke.Common.CI;
using Nuke.Common.Execution;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tools.Coverlet;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.MSBuild;
using Nuke.Common.Utilities.Collections;
using System;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml.Serialization;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static Nuke.Common.Tools.Git.GitTasks;
using static Nuke.Common.Tools.MSBuild.MSBuildTasks;

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

    bool UpToDate { get; set; } = false;

    Target CheckVersion => _ => _
        .OnlyWhenStatic(() => ExecutingTargets.SingleOrDefault(t => t.Name == nameof(PublishNuget)) != null)
        .DependentFor(Restore)
        .Executes(() =>
        {
            var tagRegex = new Regex(@"\d+\.d+\.\d+", RegexOptions.Compiled, TimeSpan.FromMilliseconds(100d));
            var tags = Git("tag").Where(t => tagRegex.IsMatch(t.Text)).Select(t => t.Text).ToArray();

            var version = Props.Value.PropertyGroup.Version;

            UpToDate = tags.Contains(Props.Value.PropertyGroup.Version);
        });

    Target Clean => _ => _
        .DependentFor(Restore)
        .Executes(() =>
            MSBuild(s => s
                .SetTargets("clean")
                .SetVerbosity(MSBuildVerbosity.Minimal)));

    Target Restore => _ => _
        .OnlyWhenDynamic(() => !UpToDate || ExecutingTargets.SingleOrDefault(t => t.Name == nameof(PublishNuget)) != null)
        .Executes(() => DotNetRestore(s => s.SetProjectFile(Solution)));

    Target Compile => _ => _
        .OnlyWhenDynamic(() => !UpToDate || ExecutingTargets.SingleOrDefault(t => t.Name == nameof(PublishNuget)) != null)
        .DependsOn(Restore)
        .Executes(() =>
            MSBuild(s => s
                .SetProjectFile(Solution)
                .SetConfiguration(Configuration.Release)
                .SetWarningsAsErrors()
                .SetVerbosity(MSBuildVerbosity.Minimal)));

    Target Test => _ => _
        .OnlyWhenDynamic(() => !UpToDate || ExecutingTargets.SingleOrDefault(t => t.Name == nameof(PublishNuget)) != null)
        .DependsOn(Compile)
        .Executes(() =>
            DotNetTest(s => s
                .SetProjectFile(Solution)
                .SetNoBuild(true)
                .SetConfiguration(Configuration.Release)
                .SetCollectCoverage(true)
                .SetCoverletOutputFormat(CoverletOutputFormat.opencover)));

    Target PublishNuget => _ => _
        .OnlyWhenDynamic(() => !UpToDate)
        .DependsOn(Test)
        .Requires(() => NugetApiUrl)
        .Requires(() => NugetApiKey)
        .Requires(() => Configuration.Equals(Configuration.Release))
        .Executes(() =>
            GlobFiles(Solution.Directory, "*.nupkg")
                .NotEmpty()
                .Where(x => !x.EndsWith("symbols.nupkg"))
                .ForEach(x =>
                    DotNetNuGetPush(s => s
                        .SetTargetPath(x)
                        .SetSource(NugetApiUrl)
                        .SetApiKey(NugetApiKey))));

    Target PushNewTag => _ => _
        .OnlyWhenDynamic(() => !UpToDate)
        .DependsOn(PublishNuget)
        .Executes(() =>
        {
            Git($"git tag {Props.Value.PropertyGroup.Version}");
            Git($"push origin {Props.Value.PropertyGroup.Version}");
        });

    Target PublishNugetPackages => _ => _
        .OnlyWhenDynamic(() => !UpToDate)
        .DependsOn(PushNewTag)
        .Executes(() => { });

    Lazy<Project> Props { get; } = new(() =>
    {
        static Project ReadProject(string s)
        {
            using var reader = new FileStream(s, FileMode.Open);
            var serializer = new XmlSerializer(typeof(Project));
            return serializer.Deserialize(reader) as Project ?? throw new InvalidOperationException("Could not deserialize Project");
        }

        var solutionDirectory = Directory.GetCurrentDirectory();
        var propsPath = Path.Combine(solutionDirectory, "MainProject", "Directory.Build.props");
        var project = ReadProject(propsPath);

        return project;
    });
}

public sealed class Project
{
    public PropertyGroup PropertyGroup { get; set; } = new();
}

public sealed class PropertyGroup
{
    public string Version { get; set; } = string.Empty;
}
