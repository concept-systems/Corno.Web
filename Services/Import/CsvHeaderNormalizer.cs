using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;

namespace Corno.Web.Services.Import
{
    /// <summary>
    /// Custom CSV column attribute that supports multiple possible column names
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class CsvColumnAlternativeAttribute : Attribute
    {
        /// <summary>
        /// Primary column name (matches LINQtoCSV CsvColumn Name)
        /// </summary>
        public string PrimaryName { get; set; }

        /// <summary>
        /// Alternative column names that should map to this property
        /// </summary>
        public string[] AlternativeNames { get; set; }

        public CsvColumnAlternativeAttribute(string primaryName, params string[] alternatives)
        {
            PrimaryName = primaryName;
            AlternativeNames = alternatives ?? Array.Empty<string>();
        }
    }

    /// <summary>
    /// CSV preprocessor that normalizes column headers to handle alternative naming conventions
    /// </summary>
    public static class CsvHeaderNormalizer
    {
        /// <summary>
        /// Normalizes CSV headers by replacing alternative column names with standard names
        /// </summary>
        /// <param name="stream">The CSV stream to normalize</param>
        /// <param name="headerMappings">Dictionary of old names to new standard names</param>
        /// <returns>A new stream with normalized headers</returns>
        public static Stream NormalizeHeaders(Stream stream, Dictionary<string, string> headerMappings)
        {
            using var reader = new StreamReader(stream);
            var headerLine = reader.ReadLine();
            
            if (string.IsNullOrEmpty(headerLine))
                throw new ArgumentException("CSV file is empty");

            // Replace alternative column names with standard names (case-insensitive, whole word match)
            foreach (var mapping in headerMappings)
            {
                headerLine = Regex.Replace(headerLine, $@"\b{Regex.Escape(mapping.Key)}\b", mapping.Value, RegexOptions.IgnoreCase);
            }

            // Create a new stream with the normalized header
            var normalizedContent = new StringBuilder();
            normalizedContent.AppendLine(headerLine);
            
            // Append the rest of the content
            string line;
            while ((line = reader.ReadLine()) != null)
            {
                normalizedContent.AppendLine(line);
            }

            var newStream = new MemoryStream(Encoding.UTF8.GetBytes(normalizedContent.ToString()));
            newStream.Position = 0;
            return newStream;
        }

        /// <summary>
        /// Normalizes CSV headers based on CsvColumnAlternativeAttribute definitions on the type
        /// </summary>
        public static Stream NormalizeHeaders<T>(Stream stream) where T : class, new()
        {
            var headerMappings = GetHeaderMappingsFromType<T>();
            return NormalizeHeaders(stream, headerMappings);
        }

        /// <summary>
        /// Gets header mappings from CsvColumnAlternativeAttribute definitions on the type
        /// </summary>
        private static Dictionary<string, string> GetHeaderMappingsFromType<T>() where T : class, new()
        {
            var mappings = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var properties = typeof(T).GetProperties();

            foreach (var property in properties)
            {
                var attr = property.GetCustomAttribute<CsvColumnAlternativeAttribute>();
                if (attr != null && attr.AlternativeNames.Length > 0)
                {
                    // Map each alternative name to the primary name
                    foreach (var altName in attr.AlternativeNames)
                    {
                        if (!string.IsNullOrEmpty(altName))
                        {
                            mappings[altName] = attr.PrimaryName;
                        }
                    }
                }
            }

            return mappings;
        }
    }
}
