﻿using Nuke.Common;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.SonarScanner;
using static Nuke.Common.Tools.SonarScanner.SonarScannerTasks;

sealed partial class Build
{
    [Parameter] readonly string SonarProjectKey = string.Empty;
    [Parameter] readonly string SonarToken = string.Empty;
    [Parameter] readonly string SonarHostUrl = string.Empty;
    [Parameter] readonly string SonarOrganization = string.Empty;

    Target SonarBegin => _ => _
        .Before(Restore)
        .OnlyWhenDynamic(() => IsCi)
        .Requires(() => SonarProjectKey)
        .Requires(() => SonarToken)
        .Requires(() => SonarHostUrl)
        .Requires(() => SonarOrganization)
        .Executes(() => SonarScannerBegin(s => s
            .SetFramework("net5.0")
            .SetProjectKey(SonarProjectKey)
            .SetLogin(SonarToken)
            .SetServer(SonarHostUrl)
            .SetProcessArgumentConfigurator(a => a.Add($"/o:\"{SonarOrganization}\""))
            .SetOpenCoverPaths("**/*.opencover.xml")
            .SetCoverageExclusions("**/*Samples*/**")));

    Target SonarEnd => _ => _
        .OnlyWhenDynamic(() => IsCi)
        .DependsOn(SonarBegin)
        .DependsOn(Test)
        .Executes(() => SonarScannerEnd(s => s
            .SetFramework("net5.0")
            .SetLogin(SonarToken)));

    // ReSharper disable once UnusedMember.Local
    Target RunSonar => _ => _
        .OnlyWhenDynamic(() => IsCi)
        .DependsOn(SonarEnd)
        .Executes(() => { });
}
