var target = Argument("target", "Default");
var configuration = Argument("configuration", "Release");
var version = Argument("version", "*");

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
        var settings = new DotNetCoreRestoreSettings
        {
            Verbose = false
        };

        DotNetCoreRestore(settings);
    });

Task("BuildSource")
    .IsDependentOn("Restore")
    .Does(() =>
    {
        var settingsNet45 = new DotNetCoreBuildSettings
        {
            Configuration = configuration,
            Framework = "net45"
        };

        var settingsNetStandard = new DotNetCoreBuildSettings
        {
            Configuration = configuration,
            Framework = "netstandard1.6"
        };

        foreach(var file in GetFiles("./src/*/*.csproj"))
        {
            DotNetCoreBuild(file.ToString(), settingsNet45);
            DotNetCoreBuild(file.ToString(), settingsNetStandard);
        }
    });

Task("BuildTests")
    .IsDependentOn("BuildSource")
    .Does(() =>
    {
        var settingsNet45 = new DotNetCoreBuildSettings
        {
            Configuration = "Debug",
            Framework = "net45"
        };

        var settingsNetCoreApp = new DotNetCoreBuildSettings
        {
            Configuration = "Debug",
            Framework = "netcoreapp1.1"
        };

        foreach(var file in GetFiles("./tests/*/*.csproj"))
        {
            DotNetCoreBuild(file.ToString(), settingsNet45);
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
            Framework = "netcoreapp1.1"
        };

        var settingsNet45 = new DotNetCoreTestSettings
        {
            Configuration = "Debug",
            Framework = "net45"
        };

        foreach(var file in GetFiles("./tests/*/*.csproj"))
        {
            DotNetCoreTest(file.ToString(), settingsNetCoreApp);
            DotNetCoreTest(file.ToString(), settingsNet45);
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