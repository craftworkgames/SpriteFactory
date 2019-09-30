#tool nuget:?package=GitVersion.CommandLine&prerelease

var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var projectName = "SpriteFactory";
var solution = $"./{projectName}.sln";

var gitVersion = GitVersion();

Information($"##teamcity[buildNumber '{gitVersion.FullSemVer}']");

TaskSetup(context => Information($"##teamcity[blockOpened name='{context.Task.Name}']"));
TaskTeardown(context => Information($"##teamcity[blockClosed name='{context.Task.Name}']"));

Task("Publish")
   .Does(() =>
   {
      Information("##teamcity[progressMessage 'Publishing...']");    

      var artifactsDirectory = "./artifacts";
      var outputDirectory = $"{artifactsDirectory}/{projectName}-v{gitVersion.SemVer}";
      var settings = new DotNetCorePublishSettings
      {
         NoRestore = true,
         Configuration = configuration,
         OutputDirectory = outputDirectory,
         ArgumentCustomization = args => args.Append($"/p:Version={gitVersion.AssemblySemVer}")
      };

      CleanDirectory(artifactsDirectory);
      DotNetCorePublish($"./{projectName}/{projectName}.csproj", settings);

      Zip(outputDirectory, $"{outputDirectory}.zip");
   });

Task("Default")
    .IsDependentOn("Publish");

RunTarget(target);
