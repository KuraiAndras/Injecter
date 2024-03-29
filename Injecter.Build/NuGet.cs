﻿using Nuke.Common;
using Nuke.Common.Tools.DotNet;
using System.Collections.Immutable;
using System.IO;
using System.Linq;
using static Nuke.Common.Tools.DotNet.DotNetTasks;
using static System.IO.Directory;

sealed partial class Build
{
    [Parameter] readonly string NugetApiUrl = "https://api.nuget.org/v3/index.json";
    [Parameter] readonly string NugetApiKey = string.Empty;

    // ReSharper disable once UnusedMember.Local
    Target PushToNuGet => _ => _
        .DependsOn(Compile)
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
}
