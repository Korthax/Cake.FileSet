using System.Collections.Generic;
using System.Linq;
using System.Text;
using Cake.Core.IO;

namespace Cake.FileSet
{
    /// <summary>
    /// Settings for the fileset search.
    /// </summary>
    public class FileSetSettings
    {
        /// <summary>
        /// The patterns to include.
        /// </summary>
        public IEnumerable<string> Includes { get; set; } = new List<string>();

        /// <summary>
        /// The patterns to exclude.
        /// </summary>
        public IEnumerable<string> Excludes { get; set; } = new List<string>();

        /// <summary>
        /// Whether the patterns are case sensitive or not.
        /// </summary>
        public bool CaseSensitive { get; set; }

        /// <summary>
        /// The base path for the patterns.
        /// </summary>
        public DirectoryPath BasePath { get; set; }

        /// <inheritdoc />
        public override string ToString()
        {
            var stringBuilder = new StringBuilder();
            stringBuilder.AppendLine("{");
            stringBuilder.AppendLine($"\t\"Includes\": [ {string.Join(",", Includes.Select(x => $"\"{x}\""))} ], ");
            stringBuilder.AppendLine($"\t\"Excludes\": [ {string.Join(",", Excludes.Select(x => $"\"{x}\""))} ], ");
            stringBuilder.AppendLine($"\t\"CaseSensitive\": {CaseSensitive}, ");
            stringBuilder.AppendLine($"\t\"BasePath\": {BasePath.FullPath}");
            stringBuilder.AppendLine("}");
            return stringBuilder.ToString();
        }
    }
}