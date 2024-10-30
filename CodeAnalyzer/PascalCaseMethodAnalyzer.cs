using System.Collections.Immutable;
using System.Linq;
using CodeAnalyzer.CdeAnalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace CodeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class PascalCaseMethodAnalyzer : DiagnosticAnalyzer
    {
        private const string PascalCaseTitle = "Method name should be PascalCase";
        private const string PascalCaseMessageFormat = "Method '{0}' should be named using PascalCase";
        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DianogsticIDs.FORCE_NAMING_CONVENTIONS_ID, 
            PascalCaseTitle,
            PascalCaseMessageFormat,
             DiagnosticCategories.Criterion,
            DiagnosticSeverity.Error,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);


        public override void Initialize(AnalysisContext context)
        {
            // 确保分析器的效率，注册只对方法声明的分析
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.EnableConcurrentExecution();
            context.RegisterSyntaxNodeAction(AnalyzeMethodDeclaration, SyntaxKind.MethodDeclaration);
        }

        private static void AnalyzeMethodDeclaration(SyntaxNodeAnalysisContext context)
        {
            //找到文档的语法根树
            var root = context.Node.SyntaxTree.GetRoot(context.CancellationToken);
            if (ConstraintDefinition.ExcludeAnalize(context.Node.SyntaxTree.FilePath))
            {//排除特殊目录
                return;
            }
            var methodDeclaration = (MethodDeclarationSyntax)context.Node;
            var methodName = methodDeclaration.Identifier.Text;

            // 检查方法名是否符合 PascalCase 规则
            if (!IsPascalCase(methodName))
            {
                var diagnostic = Diagnostic.Create(Rule, methodDeclaration.Identifier.GetLocation(), methodName);
                context.ReportDiagnostic(diagnostic);
            }
        }

        // 判断字符串是否为 PascalCase 格式
        private static bool IsPascalCase(string name)
        {
            if (string.IsNullOrEmpty(name))
                return false;

            // PascalCase: 第一个字母大写，剩下的字母小写或符合单词间大写字母规则
            return char.IsUpper(name[0]) && name.Skip(1).All(c => !char.IsUpper(c) || char.IsUpper(c));
        }
    }
}