using System;
using System.Collections.Generic;
using System.Linq;
using Cake.Core;
using Cake.Core.Annotations;
using Cake.Core.IO;

namespace Cake.FileSet
{
    /// <summary>
    /// Contains functionality for creating filesets based on includes and excludes.
    /// </summary>
    /// <remarks>
    /// The Microsoft globbing library supports the following two wildcard characters; `*` and `**` (See https://docs.microsoft.com/en-us/aspnet/core/fundamentals/file-providers#globbing-patterns for more information).
    ///
    /// * - Matches anything at the current folder level, or any filename, or any file extension.Matches are terminated by / and . characters in the file path.
    /// ** - Matches anything across multiple directory levels.Can be used to recursively match many files within a directory hierarchy.
    /// 
    /// Globbing pattern examples:
    /// directory/file.txt - Matches a specific file in a specific directory.
    /// directory/*.txt - Matches all files with .txt extension in a specific directory.
    /// directory/*/bower.json - Matches all bower.json files in directories exactly one level below the directory directory.
    /// directory/**/*.txt - Matches all files with .txt extension found anywhere under the directory directory.
    /// </remarks>
    [CakeNamespaceImport("Microsoft.Extensions.FileSystemGlobbing")]
    [CakeAliasCategory("FileSet")]
    public static class FileSetAliases
    {
        /// <summary>
        /// Gets a fileset by FileSetSettings
        /// </summary>
        /// <example>
        /// <code>
        /// Task("GetFileSet.BySettings")
        ///   .Does(() => 
        ///   {
        ///      var settings = new FileSetSettings {
        ///         Includes = new string[] { "/src/**/*.csproj" },
        ///         Excludes = new string[] { "/src/**/*.Test.csproj" },
        ///         BasePath = "D:\code\git\Cake.FileSet",
        ///         CaseSensitive = true
        ///      };
        ///
        ///      GetFileSet(settings);
        ///   });
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="settings">Settings for fileset.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <c>ICakeContext</c> is null.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when the <c>FileSetSettings</c> is null.</exception>
        [CakeMethodAlias]
        public static List<FilePath> GetFileSet(this ICakeContext context, FileSetSettings settings)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (settings == null)
                throw new ArgumentNullException(nameof(settings));

            if(settings.BasePath == null)
                settings.BasePath = context.Environment.WorkingDirectory;

            return FileSet
                .Find(settings)
                .ToList();
        }

        /// <summary>
        /// Gets a fileset by includes and excludes
        /// </summary>
        /// <example>
        /// <code>
        /// Task("GetFileSet.ByIncludes")
        ///   .Does(() => 
        ///   {
        ///      GetFileSet(
        ///         includes: new string[] { "/src/**/*.csproj" }, 
        ///         excludes: new string[] { "/src/**/*.Test.csproj" }, 
        ///         caseSensitive: false, 
        ///         basePath: "D:\code\git\Cake.FileSet"
        ///     );
        ///   });
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="includes">Patterns to include.</param>
        /// <param name="excludes">Patterns to exclude. Optional.</param>
        /// <param name="caseSensitive">Whether the pattern match is case senstive. Optional. Defaults to false.</param>
        /// <param name="basePath">Base path to use for the fileset. Optional. The working directory is used if null.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <c>ICakeContext</c>.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when the <c>includes</c> are null.</exception>
        [CakeMethodAlias]
        public static List<FilePath> GetFileSet(this ICakeContext context, IEnumerable<string> includes, IEnumerable<string> excludes = null, bool caseSensitive = false, DirectoryPath basePath = null)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (includes == null)
                throw new ArgumentNullException(nameof(includes));

            return FileSet
                .Find(basePath?.FullPath ?? context.Environment.WorkingDirectory.FullPath, includes, excludes, caseSensitive)
                .ToList();
        }

        /// <summary>
        /// Gets a fileset by single include and excludes
        /// </summary>
        /// <example>
        /// <code>
        /// Task("GetFileSet.ByInclude")
        ///   .Does(() => 
        ///   {
        ///      GetFileSet(
        ///         include: "/src/**/*.csproj", 
        ///         excludes: new string[] { "/src/**/*.Test.csproj" }, 
        ///         caseSensitive: false, 
        ///         basePath: "D:\code\git\Cake.FileSet"
        ///     );
        ///   });
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="include">The pattern to include.</param>
        /// <param name="excludes">Patterns to exclude. Optional.</param>
        /// <param name="caseSensitive">Whether the pattern match is case senstive. Optional. Defaults to false.</param>
        /// <param name="basePath">Base path to use for the fileset. Optional. The working directory is used if null.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <c>ICakeContext</c>.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when the <c>include</c> is null or whitespace.</exception>
        [CakeMethodAlias]
        public static List<FilePath> GetFileSet(this ICakeContext context, string include, IEnumerable<string> excludes = null, bool caseSensitive = false, DirectoryPath basePath = null)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (string.IsNullOrWhiteSpace(include))
                throw new ArgumentNullException(nameof(include));

            return FileSet
                .Find(basePath?.FullPath ?? context.Environment.WorkingDirectory.FullPath, new []{ include }, excludes, caseSensitive)
                .ToList();
        }


        /// <summary>
        /// Gets a fileset by patterns
        /// </summary>
        /// <example>
        /// <code>
        /// Task("GetFileSet.ByPatterns")
        ///   .Does(() => 
        ///   {
        ///      GetFileSet(
        ///         patterns: new string[] {
        ///             "/src/**/*.csproj",
        ///             "!!/src/**/*.Test.csproj"
        ///         }, 
        ///         caseSensitive: false,
        ///         basePath: "D:\code\git\Cake.FileSet"
        ///     );
        ///   });
        /// </code>
        /// </example>
        /// <param name="context">The context.</param>
        /// <param name="patterns">Patterns to match against.</param>
        /// <param name="caseSensitive">Whether the pattern match is case senstive. Optional. Defaults to false.</param>
        /// <param name="basePath">Base path to use for the fileset. Optional. The working directory is used if null.</param>
        /// <exception cref="System.ArgumentNullException">Thrown when the <c>ICakeContext</c>.</exception>
        /// <exception cref="System.ArgumentNullException">Thrown when the <c>patterns</c> are null.</exception>
        [CakeMethodAlias]
        public static List<FilePath> GetFileSet(this ICakeContext context, IEnumerable<string> patterns, bool caseSensitive = false, DirectoryPath basePath = null)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            if (patterns == null)
                throw new ArgumentNullException(nameof(patterns));

            return FileSet
                .Find(basePath?.FullPath ?? context.Environment.WorkingDirectory.FullPath, patterns, caseSensitive)
                .ToList();
        }
    }
}