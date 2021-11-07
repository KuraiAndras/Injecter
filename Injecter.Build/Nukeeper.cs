﻿using Nuke.Common;
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
    Target UpdatePackages => _ => _
        .Requires(() => NukeeperToken)
        .Executes(() => Nukeeper(
            "repo "
            + $"{Repository.HttpsUrl} "
            + $"{NukeeperToken} "
            + "-a 0 "
            + "--targetBranch develop "
            + "--maxpackageupdates 100 "
            + "--consolidate "
            + "--exclude Microsoft.Extensions.DependencyInjection.Abstractions "
            + "--exclude Unity3D.SDK "
            + "--exclude Xamarin.Forms"));
}