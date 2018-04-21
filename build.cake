var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var version = Argument("version", "*");

var netStandardVersion = "netstandard2.0";
var netFrameworkVersion = "net471";

Task("Clean")
    .Does(() =>
    {
        CleanDirectories("./artifacts");
        CleanDirectories("./src/**/bin");
        CleanDirectories("./src/**/obj");
        CleanDirectories("./tests/**/bin");
        CleanDirectories("./tests/**/obj");
    });

Task("Restore")
    .IsDependentOn("Clean")
    .Does(() =>
    {
        var settings = new DotNetCoreRestoreSettings();
        DotNetCoreRestore(settings);
    });

Task("BuildSource")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        var settingsNetFramework = new DotNetCoreBuildSettings
        {
            Configuration = configuration,
            Framework = netFrameworkVersion
        };

        var settingsNetStandard = new DotNetCoreBuildSettings
        {
            Configuration = configuration,
            Framework = netStandardVersion
        };

        foreach(var file in GetFiles("./src/*/*.csproj"))
        {
            DotNetCoreBuild(file.ToString(), settingsNetFramework);
            DotNetCoreBuild(file.ToString(), settingsNetStandard);
        }
    });

Task("BuildTests")
    .IsDependentOn("BuildSource")
    .Does(() =>
    {
        var settingsNetFramework = new DotNetCoreBuildSettings
        {
            Configuration = "Debug",
            Framework = netFrameworkVersion
        };

        var settingsNetCoreApp = new DotNetCoreBuildSettings
        {
            Configuration = "Debug",
            Framework = "netStandardVersion
        };

        foreach(var file in GetFiles("./tests/*/*.csproj"))
        {
            DotNetCoreBuild(file.ToString(), settingsNetFramework);
            DotNetCoreBuild(file.ToString(), settingsNetCoreApp);
        }
    });

Task("Test")
    .IsDependentOn("BuildTests")
    .Does(() =>
    {
        var settingsNetCoreApp = new DotNetCoreTestSettings
        {
            Configuration = "Debug",
            Framework = netStandardVersion
        };

        var settingsNetFramework = new DotNetCoreTestSettings
        {
            Configuration = "Debug",
            Framework = netFrameworkVersion
        };

        foreach(var file in GetFiles("./tests/*/*.csproj"))
        {
            DotNetCoreTest(file.ToString(), settingsNetCoreApp);
            DotNetCoreTest(file.ToString(), settingsNetFramework);
        }
    });

Task("Pack")
    .IsDependentOn("Test")
    .Does(() =>
    {
        var settings = new DotNetCorePackSettings
        {
            Configuration = "Release",
            OutputDirectory = "./artifacts/Cake.FileSet"
        };

        DotNetCorePack("./src/Cake.FileSet/Cake.FileSet.csproj", settings);
    });

Task("Default")
    .IsDependentOn("Pack");

RunTarget(target);