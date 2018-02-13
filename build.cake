#tool "nuget:?package=GitReleaseNotes";
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
var outputDir = "./artifacts";
EnsureDirectoryExists(Directory(outputDir).Path);
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
	.Does(() =>
	{
		NuGetRestore(solution, new NuGetRestoreSettings
		{
			Verbosity = MapVerbosityToNuGetVerbosity(verbosity)
		});

        var settings = new DotNetCoreRestoreSettings
         {
			 PackagesDirectory = "./packages",
             Verbosity = MapVerbosityToDotNetCoreVerbosity(verbosity)
         };

		DotNetCoreRestore(solution,settings);
	});

Task("Version")
	.Does(() => {
		GitVersion(new GitVersionSettings{
            UpdateAssemblyInfo = true,
            OutputType = GitVersionOutput.BuildServer
        });
        gitVersion = GitVersion(new GitVersionSettings{ OutputType = GitVersionOutput.Json });

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

		if (gitVersion.BranchName == "master")
		{
			gitVersion.NuGetVersionV2 = $"{gitVersion.MajorMinorPatch}-unstable";
		}

		Information($"Versioning with '{gitVersion.NuGetVersionV2}'");

		var projectFiles = GetFiles("./src/*.csproj");
		foreach(var file in projectFiles)
		{
			ReplaceTextInFiles(file.ToString(), "<VersionPrefix>1.0.0</VersionPrefix>", $"<VersionPrefix>{gitVersion.NuGetVersionV2}</VersionPrefix>");
			ReplaceTextInFiles(file.ToString(), "<VersionSuffix>local</VersionSuffix>", "");
		}
	});			

Task("Build")
	.Does(() =>
	{
        var settings = new DotNetCoreBuildSettings
        {
            Configuration = configuration,
            Verbosity = MapVerbosityToDotNetCoreVerbosity(verbosity),
			NoRestore = true
        };

		DotNetCoreBuild(solution,settings);
	});

Task("Test")
	.Does(() =>
	{
		var settings = new DotNetCoreTestSettings
        {
            Configuration = configuration,
            Verbosity = MapVerbosityToDotNetCoreVerbosity(verbosity),
            NoRestore = true,
            NoBuild = true,
            Logger = "trx"
        };

        DotNetCoreTest("./tests", settings);


	});

Task("Package")
	.Does(() =>
	{
		var settings = new DotNetCorePackSettings
		{
			OutputDirectory = outputDir,
			Verbosity = MapVerbosityToDotNetCoreVerbosity(verbosity),
			Configuration = configuration,
			NoBuild = true
		};

		var projectFiles = GetFiles("./src/*.csproj");
		foreach(var file in projectFiles)
		{
			DotNetCorePack(file.ToString(), settings);
		}
	});

Task("Generate-Release-Notes")
	.Does(() =>
	{
        var releaseNotesExitCode = StartProcess(@"tools\GitReleaseNotes.0.7.0\tools\gitreleasenotes.exe", 
        	new ProcessSettings { Arguments = ". /o artifacts/releasenotes.md" }
		);

    	if (string.IsNullOrEmpty(System.IO.File.ReadAllText("./artifacts/releasenotes.md")))
		{
        	System.IO.File.WriteAllText("./artifacts/releasenotes.md", "No issues closed since last release");
		}

    	if (releaseNotesExitCode != 0)
		{
			throw new Exception("Failed to generate release notes");
		} 
	});

Task("Upload-Artifacts")
	.WithCriteria(AppVeyor.IsRunningOnAppVeyor)
	.Does(() =>
	{
        foreach (var file in GetFiles(outputDir + "/*"))
        	AppVeyor.UploadArtifact(file.FullPath);

        foreach (var file in GetFiles("./tests/TestResults/*"))
        	AppVeyor.UploadTestResults(file.FullPath, AppVeyorTestResultsType.NUnit3);
	});

//////////////////////////////////////////////////////////////////////
// TASK TARGETS
//////////////////////////////////////////////////////////////////////

Task("BuildOnCommit")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.IsDependentOn("Build")
	.IsDependentOn("Test")
	.OnError(exception =>
	{
		PrintUsage();
	});

Task("FullBuild")
	.IsDependentOn("Clean")
	.IsDependentOn("Restore")
	.IsDependentOn("Version")
	.IsDependentOn("Build")
	.IsDependentOn("Test")
	.IsDependentOn("Package")
	.IsDependentOn("Generate-Release-Notes")
	.IsDependentOn("Upload-Artifacts")
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
