# Cake.FileSet

AddIn that adds filesets to Cake using [**Microsoft.Extensions.FileSystemGlobbing**](https://github.com/aspnet/FileSystem/tree/dev/src/Microsoft.Extensions.FileSystemGlobbing) which allows for both include and exclude patterns to be specified.

[![cakebuild.net](https://img.shields.io/badge/WWW-cakebuild.net-blue.svg)](http://cakebuild.net/)
[![NuGet Version](http://img.shields.io/nuget/v/Cake.FileSet.svg?style=flat)](https://www.nuget.org/packages/Cake.FileSet/)

## Dependencies

* Cake v0.23.0
* Microsoft.Extensions.FileSystemGlobbing v1.1.1
* NETStandard.Library v1.6.1 *when using .NETStandard1.6* or NET46

## Globbing patterns

The Microsoft globbing library supports the following two wildcard characters; `*` and `**`:

**`*`**

Matches anything at the current folder level, or any filename, or any file extension. Matches are terminated by `/` and `.` characters in the file path.

<strong><code>**</code></strong>

Matches anything across multiple directory levels. Can be used to recursively match many files within a directory hierarchy.

### Globbing pattern examples

**`directory/file.txt`**

   Matches a specific file in a specific directory.

**<code>directory/*.txt</code>**

   Matches all files with `.txt` extension in a specific directory.

**`directory/*/bower.json`**

   Matches all `bower.json` files in directories exactly one level below the `directory` directory.

**<code>directory/&#42;&#42;/&#42;.txt</code>**

   Matches all files with `.txt` extension found anywhere under the `directory` directory.

### Additional usage

When finding files by patterns you can use `!` to negate a pattern (exclude it).


### Further Information

For more information on how Mircosoft's globbing works please see either their [documentation](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/file-providers#globbing-patterns) or their [source ](https://docs.microsoft.com/en-us/aspnet/core/fundamentals/file-providers#globbing-patterns).

## Including the AddIn

```csharp
#addin "Cake.FileSet"
#addin "nuget:?package=Microsoft.Extensions.FileSystemGlobbing&version=1.1.1"
```

## Addin Usage

```csharp
#addin "Cake.FileSet"
#addin "nuget:?package=Microsoft.Extensions.FileSystemGlobbing&version=1.1.1"

...

Task("GetFileSet.BySettings")
    .Does(() =>
    {
        var settings = new FileSetSettings {
            Includes = new string[] { "/src/**/one.txt" },
            Excludes = new string[] { "/src/c/*.txt" },
            BasePath = "D:/code/git/Cake.FileSet",
            CaseSensitive = false
        };

        var result = GetFileSet(settings);

        foreach(FilePath item in result)
        {
            Information(item);
        }
    });

Task("GetFileSet.ByIncludes")
    .Does(() =>
    {
        var result = GetFileSet( 
            includes: new string[] { "/src/**/*.txt" },
            excludes: new string[] { "**/two.txt" },
            caseSensitive: false,
            basePath: "D:/code/git/Cake.FileSet"
        );

        foreach(FilePath item in result)
        {
            Information(item);
        }
    });

Task("GetFileSet.ByInclude")
    .Does(() =>
    {
        var result = GetFileSet(
            include: "/src/**/*.txt",
            excludes: new string[] { "**/two.txt" },
            caseSensitive: false,
            basePath: "D:/code/git/Cake.FileSet"
        );

        foreach(FilePath item in result)
        {
            Information(item);
        }
    });

Task("GetFileSet.ByPatterns")
    .Does(() =>
    {
        var result = GetFileSet(
            patterns: new string[] { "**/one.txt", "!**/two.txt" },
            caseSensitive: false,
            basePath: baseDirectory
        );

        foreach(FilePath item in result)
        {
            Information(item);
        }
    });
```

## Direct Usage

```csharp
#addin "Cake.FileSet"
#addin "nuget:?package=Microsoft.Extensions.FileSystemGlobbing&version=1.1.1"

...

IEnumberable<FilePath> bySettings = FileSet.Find(new FileSetSettings {
    Includes = new string[] { "/src/**/one.txt" },
    Excludes = new string[] { "/src/c/*.txt" },
    BasePath = "D:/code/git/Cake.FileSet",
    CaseSensitive = false
});

IEnumberable<FilePath> byIncludes = FileSet.Find(
    includes: new string[] { "/src/**/one.txt" },
    excludes: new string[] { "/src/c/*.txt" },
    basePath: "D:/code/git/Cake.FileSet",
    caseSensitive: false
);

IEnumberable<FilePath> byInclude = FileSet.Find(
    include: "/src/**/one.txt",
    excludes: new string[] { "/src/c/*.txt" },
    basePath: "D:/code/git/Cake.FileSet",
    caseSensitive: false
);

IEnumberable<FilePath> byPatterns = FileSet.Find(
    patterns: new string[] { "**/one.txt", "!**/two.txt" },
    basePath: "D:/code/git/Cake.FileSet",
    caseSensitive: false
);

```

# General Notes
**This is an initial version and not tested thoroughly.**

**I've created this addin for use in my own scripts as I wasn't happy with the usage of the in built one therefore use at your own risk :)**