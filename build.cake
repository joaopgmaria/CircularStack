#tool "nuget:?package=GitVersion.CommandLine";
#tool "nuget:?package=NUnit.ConsoleRunner";
#addin "nuget:?package=Cake.FileHelpers";

public void PrintUsage()
{
	Console.WriteLine($"Usage: build.cake [options]{Environment.NewLine}" +
								$"Options:{Environment.NewLine}" +
								$"\t-target\t\t\t\tCake build entry point.\tDefaults to 'BuildOnCommit'.{Environment.NewLine}" +
								$"\t-configuration\t\t\tBuild configuration [Debug|Release]. Defaults to 'Release'.{Environment.NewLine}" +
								$"\t-verbosity\t\t\tVerbosity [Quiet|Minimal|Normal|Verbose|Diagnostic]. Defaults to 'Minimal'.{Environment.NewLine}" +
								$"\t-branch\t\tThe branch being built. Required{Environment.NewLine}");
}

private Verbosity ParseVerbosity(string verbosity)
{
	Verbosity typedVerbosity;
	if(Enum.TryParse<Verbosity>(verbosity, out typedVerbosity)){
		return typedVerbosity;
	}
	return Verbosity.Minimal;
}

private DotNetCoreVerbosity MapVerbosityToDotNetCoreVerbosity(Verbosity verbosity)
{
	switch(verbosity)
	{
		case Verbosity.Diagnostic:
            return DotNetCoreVerbosity.Diagnostic;
		case Verbosity.Verbose:
			return DotNetCoreVerbosity.Detailed;
		case Verbosity.Quiet:
			return DotNetCoreVerbosity.Quiet;
		case Verbosity.Minimal:
			return DotNetCoreVerbosity.Minimal;
        default:
			return DotNetCoreVerbosity.Normal;
	}
}

private NuGetVerbosity MapVerbosityToNuGetVerbosity(Verbosity verbosity)
{
	switch(verbosity)
	{
		case Verbosity.Diagnostic:
		case Verbosity.Verbose:
			return NuGetVerbosity.Detailed;
		case Verbosity.Quiet:
			return NuGetVerbosity.Quiet;
		default:
			return NuGetVerbosity.Normal;
	}
}

//////////////////////////////////////////////////////////////////////
// ARGUMENTS
//////////////////////////////////////////////////////////////////////

var target = Argument("target", "BuildOnCommit");
var configuration = Argument("configuration", "Release");
var verbosity = ParseVerbosity(Argument("verbosity", "Minimal"));

//////////////////////////////////////////////////////////////////////
// PREPARATION
//////////////////////////////////////////////////////////////////////
var solution = "./CircularStack.sln";
var nugetOutputPath = "./nuget/.output";
string nugetVersion = null;
string assemblyVersion = null;
GitVersion gitVersion = null;

//////////////////////////////////////////////////////////////////////
// TASKS
//////////////////////////////////////////////////////////////////////

Task("Clean")
	.Does(() =>
	{
        var settings = new DotNetCoreCleanSettings
         {
             Configuration = configuration,
             Verbosity = MapVerbosityToDotNetCoreVerbosity(verbosity)
         };

		DotNetCoreClean(solution,settings);
	});

Task("Restore")
    .IsDependentOn("Clean")
	.Does(() =>
	{
		// NuGetRestore(solution, new NuGetRestoreSettings
		// {
		// 	Verbosity = MapVerbosityToNuGetVerbosity(verbosity)
		// });

        var settings = new DotNetCoreRestoreSettings
         {
			 PackagesDirectory = "./packages",
             Verbosity = MapVerbosityToDotNetCoreVerbosity(verbosity)
         };

		DotNetCoreRestore(solution,settings);
	});

Task("Build")
    .IsDependentOn("Restore")
	.Does(() =>
	{
        var settings = new DotNetCoreBuildSettings
        {
            Configuration = configuration,
            Verbosity = MapVerbosityToDotNetCoreVerbosity(verbosity)
        };

		DotNetCoreBuild(solution,settings);
	});

Task("Run-Tests")
	.IsDependentOn("Build")
	.Does(() =>
	{
		// NUnit3($"./src/*/bin/{configuration}/*.Tests.dll", new NUnit3Settings
		// {
		// 	Configuration = configuration,
		// 	TeamCity = runningOnBuildServer
		// });
		Information("Bypassing test execution");
	});

Task("Pack-NuGet-Packages")
    //.IsDependentOn("Run-Tests")
	.IsDependentOn("Get-GitVersion")
	.Does(() =>
	{
		if(string.IsNullOrWhiteSpace(gitVersion.PreReleaseTagWithDash) && gitVersion.BranchName != "master")
		{
			Information("No Pre-Release tag found. Versioning as a Release...");
			nugetVersion = $"{gitVersion.MajorMinorPatch}.{gitVersion.PreReleaseNumber}";
		}
		else
		{
			Information($"Pre-Release tag '{gitVersion.PreReleaseLabel}' found. Versioning as a Pre-Release...");
			nugetVersion = gitVersion.NuGetVersionV2;
		}

		assemblyVersion = gitVersion.AssemblySemVer;

        Information($"Using version {nugetVersion} for nuget packages");

		var settings = new DotNetCorePackSettings
		{
			OutputDirectory = nugetOutputPath,
			Verbosity = MapVerbosityToDotNetCoreVerbosity(verbosity),
			Configuration = configuration,
            NoBuild = true
		};

		EnsureDirectoryExists(Directory(nugetOutputPath).Path);

        var projectFiles = GetFiles("./src/*.csproj");
        foreach(var file in projectFiles)
        {
            ReplaceTextInFiles(file.ToString(), "<VersionPrefix>1.0.0.0</VersionPrefix>", $"<VersionPrefix>{nugetVersion}</VersionPrefix>");
            DotNetCorePack(file.ToString(), settings);
        }
	});

Task("Get-GitVersion")
		.Does(() => {
			gitVersion = GitVersion(new GitVersionSettings
			{
				UpdateAssemblyInfo = false,
				NoFetch = true,
				WorkingDirectory = "./"
			});

			Information($"AssemblySemVer: {gitVersion.AssemblySemVer}{Environment.NewLine}"+
									$"SemVer: {gitVersion.AssemblySemVer}{Environment.NewLine}" +
									$"FullSemVer: {gitVersion.FullSemVer}{Environment.NewLine}" +
									$"MajorMinorPatch: {gitVersion.MajorMinorPatch}{Environment.NewLine}" +
									$"NuGetVersionV2: {gitVersion.NuGetVersionV2}{Environment.NewLine}" +
									$"NuGetVersion: {gitVersion.NuGetVersion}{Environment.NewLine}" +
									$"BranchName: {gitVersion.BranchName}{Environment.NewLine}" +
									$"Sha: {gitVersion.Sha}{Environment.NewLine}" +
									$"Pre-Release Label: {gitVersion.PreReleaseLabel}{Environment.NewLine}" +
									$"Pre-Release Number: {gitVersion.PreReleaseNumber}{Environment.NewLine}" +
									$"Pre-Release Tag: {gitVersion.PreReleaseTag}{Environment.NewLine}" +
									$"Pre-Release Tag with dash: {gitVersion.PreReleaseTagWithDash}{Environment.NewLine}" +
									$"Build MetaData: {gitVersion.BuildMetaData}{Environment.NewLine}" +
									$"Build MetaData Padded: {gitVersion.BuildMetaDataPadded}{Environment.NewLine}" +
									$"Full Build MetaData: {gitVersion.FullBuildMetaData}{Environment.NewLine}");
		});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("BuildOnCommit")
	.IsDependentOn("Pack-NuGet-Packages")
	.OnError(exception =>
	{
		PrintUsage();
	});

//////////////////////////////////////////////////////////////////////
// EXECUTION
//////////////////////////////////////////////////////////////////////
if (target=="Help")
{
	PrintUsage();
}
else
{
	RunTarget(target);
}
