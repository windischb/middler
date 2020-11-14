using System;
using System.Linq;
using Nuke.Common;
using Nuke.Common.CI.GitHubActions;
using Nuke.Common.Execution;
using Nuke.Common.Git;
using Nuke.Common.IO;
using Nuke.Common.ProjectModel;
using Nuke.Common.Tooling;
using Nuke.Common.Tools.DotNet;
using Nuke.Common.Tools.GitVersion;
using Nuke.Common.Utilities.Collections;
using static Nuke.Common.EnvironmentInfo;
using static Nuke.Common.IO.FileSystemTasks;
using static Nuke.Common.IO.PathConstruction;
using static Nuke.Common.Tools.DotNet.DotNetTasks;

[CheckBuildProjectConfigurations]
[UnsetVisualStudioEnvironmentVariables]
[GitHubActions("beta",
    GitHubActionsImage.UbuntuLatest,
    AutoGenerate = true,
    OnPushBranches = new[] { "develop" },
    InvokedTargets = new[] { nameof(Push) },
    ImportSecrets = new [] { "GITHUB_TOKEN" }
)]
class Build : NukeBuild
{
    /// Support plugins are available for:
    ///   - JetBrains ReSharper        https://nuke.build/resharper
    ///   - JetBrains Rider            https://nuke.build/rider
    ///   - Microsoft VisualStudio     https://nuke.build/visualstudio
    ///   - Microsoft VSCode           https://nuke.build/vscode

    public static int Main () => Execute<Build>(x => x.Compile);

    [Parameter("Configuration to build - Default is 'Debug' (local) or 'Release' (server)")]
    readonly string Configuration = IsLocalBuild ? "Debug" : "Release";

    [Solution] readonly Solution Solution;
    [GitRepository] readonly GitRepository GitRepository;
    [GitVersion] readonly GitVersion GitVersion;

    [Parameter("ApiKey", Name = "GITHUB_TOKEN")]
    readonly string GITHUB_TOKEN;

    AbsolutePath SourceDirectory => RootDirectory / "source";
    AbsolutePath TestsDirectory => RootDirectory / "tests";
    AbsolutePath OutputDirectory => RootDirectory / "output";

    Target Clean => _ => _
        .Before(Restore)
        .Executes(() => {
            SourceDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            TestsDirectory.GlobDirectories("**/bin", "**/obj").ForEach(DeleteDirectory);
            EnsureCleanDirectory(OutputDirectory);
        });

    Target Restore => _ => _
        .Executes(() => {
            DotNetRestore(s => s
                .SetProjectFile(Solution));
        });

    Target Compile => _ => _
        .DependsOn(Restore)
        .Executes(() => {

            var libraries = Solution.AllProjects.Where(p => p.Name.StartsWith("middler.") && !p.Name.Contains("Test", StringComparison.OrdinalIgnoreCase));

            foreach (var library in libraries)
            {
                var frameworks = library.GetTargetFrameworks().Where(fr => fr.StartsWith("netstandard") || fr.StartsWith("netcoreapp") || fr.StartsWith("net5"));

                if (Platform == PlatformFamily.Windows)
                {

                    DotNetBuild(s => s
                        .SetConfiguration("release")
                        .SetAssemblyVersion(GitVersion.AssemblySemVer)
                        .SetFileVersion(GitVersion.AssemblySemFileVer)
                        .SetInformationalVersion(GitVersion.InformationalVersion)
                        .EnableNoRestore()
                        .SetProjectFile(library)
                    );

                }
                else
                {
                    foreach (var framework in frameworks)
                    {
                        DotNetBuild(s => s
                            .SetConfiguration("release")
                            .SetAssemblyVersion(GitVersion.AssemblySemVer)
                            .SetFileVersion(GitVersion.AssemblySemFileVer)
                            .SetInformationalVersion(GitVersion.InformationalVersion)
                            .EnableNoRestore()
                            .SetProjectFile(library)
                            .SetFramework(framework)
                        );
                    }
                }
            }

        });
    
    Target Pack => _ => _
        .DependsOn(Restore)
        .Executes(() => {

            var libraries = Solution.AllProjects.Where(p => p.Name.StartsWith("middler.") && !p.Name.Contains("Test", StringComparison.OrdinalIgnoreCase));

            DotNetPack(s => s
                .SetVersion(GitVersion.NuGetVersionV2)
                .SetConfiguration("release")
                .SetAssemblyVersion(GitVersion.AssemblySemVer)
                .SetFileVersion(GitVersion.AssemblySemFileVer)
                .SetInformationalVersion(GitVersion.InformationalVersion)
                .EnableNoRestore()
                .SetOutputDirectory(OutputDirectory)
                .CombineWith(libraries, (settings, project) => settings.SetProject(project))
            );
        });

    Target Push => _ => _
        .DependsOn(Pack)
        .Executes(() =>
        {

            DotNetNuGetPush(o => o
                .SetSource("https://nuget.pkg.github.com/windischb/index.json")
                .SetApiKey(GITHUB_TOKEN)
                .SetTargetPath(OutputDirectory / "*.nupkg")
            );
           
        });
}
