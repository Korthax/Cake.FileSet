using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Cake.Core.IO;
using Microsoft.Extensions.FileSystemGlobbing;
using Microsoft.Extensions.FileSystemGlobbing.Abstractions;

using Path = System.IO.Path;

namespace Cake.FileSet
{
    /// <summary>
    /// Finds a set of <c>FilePath</c> based on input patterns.
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
    public class FileSet
    {
        private readonly DirectoryInfoBase _directoryInfoWrapper;
        private readonly IEnumerable<string> _includes;
        private readonly IEnumerable<string> _excludes;
        private readonly bool _caseSensitive;

        /// <summary>
        /// Gets a fileset by FileSetSettings
        /// </summary>
        /// <example>
        /// <code>
        /// IEnumerable&gt;FilePath&lt; filePaths = FileSet.Find(new FileSetSettings());
        /// </code>
        /// </example>
        /// <returns>Returns an IEnumberable of <c>FilePath</c> that match the input patterns.</returns>
        /// <param name="settings">Settings for fileset.</param>
        public static IEnumerable<FilePath> Find(FileSetSettings settings)
        {
            return Find(settings.BasePath.FullPath, settings.Includes, settings.Excludes, settings.CaseSensitive);
        }

        /// <summary>
        /// Gets a fileset by FileSetSettings
        /// </summary>
        /// <example>
        /// <code>
        /// IEnumerable&gt;FilePath&lt; filePaths = FileSet.Find("D:\code\git\Cake.FileSet", new string[] { "/src/**/*.csproj", "!!/src/**/*.Test.csproj" }, false);
        /// </code>
        /// </example>
        /// <returns>Returns an IEnumberable of <c>FilePath</c> that match the input patterns.</returns>
        /// <param name="patterns">Patterns to match against.</param>
        /// <param name="caseSensitive">Whether the pattern match is case senstive. Defaults to false.</param>
        /// <param name="basePath">Base directory to use for the fileset. The working directory is used if null.</param>
        public static IEnumerable<FilePath> Find(string basePath, IEnumerable<string> patterns, bool caseSensitive = false)
        {
            var includes = new List<string>();
            var excludes = new List<string>();

            foreach(var pattern in patterns ?? new string[0])
            {
                if(pattern.StartsWith("!"))
                    excludes.Add(pattern.Substring(1));
                else
                    includes.Add(pattern);
            }

            return Find(basePath, includes, excludes, caseSensitive);
        }

        /// <summary>
        /// Gets a fileset by FileSetSettings
        /// </summary>
        /// <example>
        /// <code>
        /// IEnumerable&gt;FilePath&lt; filePaths = FileSet.Find("D:\code\git\Cake.FileSet", new string[] { "/src/**/*.csproj" } , new string[] { "/src/**/*.Test.csproj" }, false);
        /// </code>
        /// </example>
        /// <returns>Returns an IEnumberable of <c>FilePath</c> that match the input patterns.</returns>
        /// <param name="include">Pattern to include.</param>
        /// <param name="excludes">Patterns to exclude.</param>
        /// <param name="caseSensitive">Whether the pattern match is case senstive. Defaults to false.</param>
        /// <param name="basePath">Base directory to use for the fileset. The working directory is used if null.</param>
        public static IEnumerable<FilePath> Find(string basePath, string include, IEnumerable<string> excludes = null, bool caseSensitive = false)
        {
            return Find(basePath, new[] { include }, excludes, caseSensitive);
        }

        /// <summary>
        /// Gets a fileset by FileSetSettings
        /// </summary>
        /// <example>
        /// <code>
        /// IEnumerable&gt;FilePath&lt; filePaths = FileSet.Find("D:\code\git\Cake.FileSet", new string[] { "/src/**/*.csproj" } , new string[] { "/src/**/*.Test.csproj" }, false);
        /// </code>
        /// </example>
        /// <returns>Returns an IEnumberable of <c>FilePath</c> that match the input patterns.</returns>
        /// <param name="includes">Patterns to include.</param>
        /// <param name="excludes">Patterns to exclude.</param>
        /// <param name="caseSensitive">Whether the pattern match is case senstive. Defaults to false.</param>
        /// <param name="basePath">Base directory to use for the fileset. The working directory is used if null.</param>
        public static IEnumerable<FilePath> Find(string basePath, IEnumerable<string> includes, IEnumerable<string> excludes = null, bool caseSensitive = false)
        {
            var directoryInfoWrapper = new DirectoryInfoWrapper(new DirectoryInfo(basePath));
            return new FileSet(directoryInfoWrapper, includes ?? new string[0], excludes ?? new string[0], caseSensitive)
                .GetFiles();
        }

        internal FileSet(DirectoryInfoBase baseDirectory, IEnumerable<string> includes, IEnumerable<string> excludes, bool caseSensitive)
        {
            _includes = includes;
            _excludes = excludes;
            _caseSensitive = caseSensitive;
            _directoryInfoWrapper = baseDirectory;
        }

        internal IEnumerable<FilePath> GetFiles()
        {
            if(!_includes.Any())
                return new FilePath[0];

            var matcher = new Matcher(_caseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase);
            matcher.AddIncludePatterns(_includes);

            if(_excludes.Any())
                matcher.AddExcludePatterns(_excludes);

            var result = matcher.Execute(_directoryInfoWrapper);

            return result.HasMatches
                ? result.Files.Select(x => new FilePath(Path.Combine(_directoryInfoWrapper.FullName, x.Path)))
                : new FilePath[0];
        }
    }
}