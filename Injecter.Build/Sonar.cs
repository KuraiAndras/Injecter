using Nuke.Common;
using Nuke.Common.Tools.SonarScanner;
using static Nuke.Common.Tools.SonarScanner.SonarScannerTasks;


sealed partial class Build
{
    [Parameter] readonly string SonarProjectKey = string.Empty;
    [Parameter] readonly string SonarToken = string.Empty;
    [Parameter] readonly string SonarHostUrl = string.Empty;

    Target SonarBegin => _ => _
        .Before(Restore)
        .Requires(() => SonarProjectKey)
        .Requires(() => SonarToken)
        .Requires(() => SonarHostUrl)
        .Executes(() => SonarScannerBegin(s => s
            .SetProjectKey(SonarProjectKey)
            .SetLogin(SonarToken)
            .SetServer(SonarHostUrl)
            .SetOpenCoverPaths("**/*.opencover.xml")));

    Target SonarEnd => _ => _
        .DependsOn(SonarBegin)
        .DependsOn(Test)
        .Executes(() => SonarScannerEnd(s => s.SetLogin(SonarToken)));

    Target RunSonar => _ => _
        .DependsOn(SonarEnd)
        .Executes(() => { });
}
