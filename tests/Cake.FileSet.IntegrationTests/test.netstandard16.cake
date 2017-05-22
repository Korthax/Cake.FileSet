#r ".\..\..\src\Cake.FileSet\bin\Debug\netstandard1.6\Cake.FileSet.dll"
#addin "nuget:?package=Microsoft.Extensions.FileSystemGlobbing&version=1.1.1"
#addin "nuget:?package=NUnit"

using NUnit.Framework;

var baseDirectory = Context.Environment.WorkingDirectory;

Setup(context =>
{
    Information(string.Format("Base Directory: {0}", baseDirectory));
});

Task("GetFileSet.BySettings.CaseSensitive")
    .Does(() =>
    {
        var settings = new FileSetSettings {
            Includes = new string[] { "./src/**/one.txt" },
            Excludes = new string[] { "./src/c/*.txt" },
            BasePath = baseDirectory,
            CaseSensitive = true
        };

        var result = GetFileSet(settings).ToList();

        foreach(var item in result)
        {
            Information(item);
        }

        Assert.That(result.Count, Is.EqualTo(1));
        Assert.That(result.Count(x => x.FullPath == string.Format("{0}/src/a/one.txt", baseDirectory)), Is.EqualTo(1));
    });

Task("GetFileSet.BySettings.CaseInsensitive")
    .Does(() =>
    {
        var settings = new FileSetSettings {
            Includes = new string[] { "/src/**/one.txt" },
            Excludes = new string[] { "/src/c/*.txt" },
            BasePath = baseDirectory,
            CaseSensitive = false
        };

        var result = GetFileSet(settings).ToList();

        foreach(var item in result)
        {
            Information(item);
        }

        Assert.That(result.Count, Is.EqualTo(2));
        Assert.That(result.Count(x => x.FullPath == string.Format("{0}/src/a/one.txt", baseDirectory)), Is.EqualTo(1));
        Assert.That(result.Count(x => x.FullPath == string.Format("{0}/src/b/ONE.txt", baseDirectory)), Is.EqualTo(1));
    });

Task("GetFileSet.ByIncludes")
    .Does(() =>
    {
        var result = GetFileSet( 
            includes: new string[] { "/src/**/*.txt" },
            excludes: new string[] { "**/two.txt" },
            caseSensitive: false,
            basePath: baseDirectory
        ).ToList();

        foreach(var item in result)
        {
            Information(item);
        }

        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.Count(x => x.FullPath == string.Format("{0}/src/a/one.txt", baseDirectory)), Is.EqualTo(1));
        Assert.That(result.Count(x => x.FullPath == string.Format("{0}/src/b/ONE.txt", baseDirectory)), Is.EqualTo(1));
        Assert.That(result.Count(x => x.FullPath == string.Format("{0}/src/c/one.txt", baseDirectory)), Is.EqualTo(1));
    });

Task("GetFileSet.ByInclude")
    .Does(() =>
    {
        var result = GetFileSet(
            include: "/src/**/*.txt",
            excludes: new string[] { "**/two.txt" },
            caseSensitive: false,
            basePath: baseDirectory
        ).ToList();

        foreach(var item in result)
        {
            Information(item);
        }

        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.Count(x => x.FullPath == string.Format("{0}/src/a/one.txt", baseDirectory)), Is.EqualTo(1));
        Assert.That(result.Count(x => x.FullPath == string.Format("{0}/src/b/ONE.txt", baseDirectory)), Is.EqualTo(1));
        Assert.That(result.Count(x => x.FullPath == string.Format("{0}/src/c/one.txt", baseDirectory)), Is.EqualTo(1));
    });

Task("GetFileSet.ByPatterns")
    .Does(() =>
    {
        var result = GetFileSet(
            patterns: new string[] { "**/one.txt", "!!**/two.txt" },
            caseSensitive: false,
            basePath: baseDirectory
        ).ToList();

        foreach(var item in result)
        {
            Information(item);
        }

        Assert.That(result.Count, Is.EqualTo(3));
        Assert.That(result.Count(x => x.FullPath == string.Format("{0}/src/a/one.txt", baseDirectory)), Is.EqualTo(1));
        Assert.That(result.Count(x => x.FullPath == string.Format("{0}/src/b/ONE.txt", baseDirectory)), Is.EqualTo(1));
        Assert.That(result.Count(x => x.FullPath == string.Format("{0}/src/c/one.txt", baseDirectory)), Is.EqualTo(1));
    });

Task("RunTests")
    .IsDependentOn("GetFileSet.BySettings.CaseSensitive")
    .IsDependentOn("GetFileSet.BySettings.CaseInsensitive")
    .IsDependentOn("GetFileSet.ByIncludes")
    .IsDependentOn("GetFileSet.ByInclude")
    .IsDependentOn("GetFileSet.ByPatterns");

RunTarget("RunTests");