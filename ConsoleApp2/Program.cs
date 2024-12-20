﻿using CodeAnalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Diagnostics;
using System.Collections.Immutable;

namespace ConsoleApp
{
    internal class Program
    {
        public static async Task Main(string[] args)
        {
            // 示例：要分析的 C# 代码
            string codeToAnalyze = @"
            public class MyClass
            {
                public void myMethod() {}
                public void CorrectMethod() {}
            }
        ";

            // 创建一个编译对象来包含代码
            SyntaxTree syntaxTree = CSharpSyntaxTree.ParseText(codeToAnalyze);
            var compilation = CSharpCompilation.Create("TestCompilation")
                .AddReferences(MetadataReference.CreateFromFile(typeof(object).Assembly.Location))
                .AddSyntaxTrees(syntaxTree);

            // 创建分析器
            var analyzer = new PascalCaseMethodAnalyzer();
            var analyzers = ImmutableArray.Create<DiagnosticAnalyzer>(analyzer);

            // 运行分析器
            var compilationWithAnalyzers = compilation.WithAnalyzers(analyzers);
            var diagnostics = await compilationWithAnalyzers.GetAnalyzerDiagnosticsAsync();

            // 输出诊断结果
            Console.WriteLine("Diagnostics:");
            foreach (var diagnostic in diagnostics)
            {
                Console.WriteLine(diagnostic);
            }

            Console.ReadKey();
        }
        
        private static async Task VerificationMethodName()
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
