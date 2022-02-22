﻿using Nuke.Common;
using static Nuke.Common.Tools.DocFX.DocFXTasks;

#pragma warning disable CA1822 // Mark members as static
sealed partial class Build
{
    Target CreateMetadata => _ => _
        .DependsOn(Compile)
        .Executes(() => DocFX($"metadata {DocFxJsonPath}"));

    // ReSharper disable once UnusedMember.Local
    Target BuildDocs => _ => _
        .DependsOn(CreateMetadata)
        .Executes(() => DocFX($"build {DocFxJsonPath}"));

    // ReSharper disable once UnusedMember.Local
    Target ServeDocs => _ => _
        .DependsOn(BuildDocs)
        .Executes(() => DocFX($"{DocFxJsonPath} --serve"));
}
#pragma warning restore CA1822 // Mark members as static
