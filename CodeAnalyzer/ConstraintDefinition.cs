using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace CodeAnalyzer
{
    public class ConstraintDefinition
    {
        public static List<string> AnalyzerExcludePath = new List<string>() {
            "/PackageCache/",
            "/ThirdLibs/",
            "/Plugins/",
            "/Library/"
        };

        public static bool ExcludeAnalize(string path)
        {
            string normalizedPath = path.Replace("\\", "/");

            foreach (var file in AnalyzerExcludePath)
            {
                if (normalizedPath.Contains(file))
                {
                    Console.WriteLine($"Excluding file: {path} (matches {file})");
                    return true;
                }
            }
            return false;
        }
    }
}
