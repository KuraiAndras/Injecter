using Nuke.Common;
using Nuke.Common.Git;
using Nuke.Common.Tooling;

sealed partial class Build
{
    [Parameter] readonly string NukeeperToken = string.Empty;
    [GitRepository] readonly GitRepository Repository = default!;

    [PackageExecutable(
        "nukeeper",
        "nukeeper.dll",
        Framework = "netcoreapp3.1")]
    readonly Tool Nukeeper = default!;

    // ReSharper disable once UnusedMember.Local
    Target UpdateNuGetPackages => _ => _
        .Requires(() => NukeeperToken)
        .Executes(() => Nukeeper(
            "repo "
            + $"{Repository.HttpsUrl} "
            + $"{NukeeperToken} "
            + "-a 0 "
            + "--targetBranch develop "
            + "--maxpackageupdates 100 "
            + "--consolidate "
            + @"--exclude (Microsoft\.Extensions\.DependencyInjection\.Abstractions)|(Unity3D\.SDK)|(Xamarin\.Forms)"));
}
