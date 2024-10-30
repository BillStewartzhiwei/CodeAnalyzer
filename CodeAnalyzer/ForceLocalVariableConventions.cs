using CodeAnalyzer.CdeAnalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace CodeAnalyzer
{
    /// <summary>
    /// 本地变量检查
    /// </summary>

    [DiagnosticAnalyzer(Microsoft.CodeAnalysis.LanguageNames.CSharp)]
    public class ForceLocalVariableConventions : DiagnosticAnalyzer
    {
        private const string PublicVarDescriptorTitle = "Temporary variable naming does not comply with the specification, please use small hump naming";
        private const string PublicVarDescriptorMessageFormat = "The variable '{0}' requires the small hump nomenclature";
        /// <summary>
        /// 错误描述
        /// </summary>
        private static readonly DiagnosticDescriptor LocalVarDescriptor =
            new DiagnosticDescriptor(
                DianogsticIDs.FORCE_NAMING_CONVENTIONS_ID,          // ID
                PublicVarDescriptorTitle,    // PascalCaseTitle
                PublicVarDescriptorMessageFormat, // Message format
                DiagnosticCategories.Naming,                // PascalCaseCategory
                DiagnosticSeverity.Error, // Severity
                isEnabledByDefault: true    // Enabled by default
            );

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(LocalVarDescriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(CheckTemporaryVariable);
        }

        private static void CheckTemporaryVariable(SyntaxTreeAnalysisContext context)
        {
            //找到文档的语法根树
            var root = context.Tree.GetRoot(context.CancellationToken);
            if (ConstraintDefinition.ExcludeAnalize(context.Tree.FilePath))
            {//排除特殊目录
                return;
            }
            var localNodeList = root.DescendantNodes()?.OfType<LocalDeclarationStatementSyntax>();
            foreach (var localNode in localNodeList)
            {
                var varList = localNode.Declaration.Variables;
                foreach (var localVar in varList)
                {
                    var localName = localVar.Identifier.Value.ToString();
                    var firstChar = localName.First().ToString();
                    if (firstChar.ToLower() != firstChar)
                    {
                        //判断第一个字母是否是小写
                        //报错
                        var diagnostic = Diagnostic.Create(LocalVarDescriptor, localNode.GetFirstToken().GetLocation(), localName);
                        context.ReportDiagnostic(diagnostic);
                    }
                }
            }
        }
    }
}
