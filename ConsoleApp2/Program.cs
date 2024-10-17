using CodeAnalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace ConsoleApp2
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 测试路径列表
            List<string> testPaths = new List<string>
            {
                @"C:\Projects\MyApp\PackageCache\SomeFile.cs",
                @"C:\Projects\MyApp\ThirdLibs\SomeLibrary.cs",
                @"C:\Projects\MyApp\Plugins\SomePlugin.dll",
                @"C:\Projects\MyApp\Library\SomeLibraryFile.dll",
                @"C:\Projects\MyApp\Scripts\MyScript.cs" // 不应排除的路径
            };

            // 遍历路径并检查是否应该排除
            foreach (var path in testPaths)
            {
                if (ConstraintDefinition.ExcludeAnalize(path))
                {
                    Console.WriteLine($"Path '{path}' is excluded.");
                }
                else
                {
                    Console.WriteLine($"Path '{path}' is NOT excluded.");
                }
            }

            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
        }
    }
}
