using Nuke.Common;
using static Nuke.Common.Tools.DocFX.DocFXTasks;

sealed partial class Build
{
    Target CreateMetadata => _ => _
        .Executes(() => DocFX($"metadata {DocFxJsonPath}"));

    Target BuildDocs => _ => _
        .DependsOn(CreateMetadata)
        .Executes(() => DocFX($"build {DocFxJsonPath}"));

    Target ServeDocs => _ => _
        .Executes(() => DocFX($"{DocFxJsonPath} --serve"));
}
