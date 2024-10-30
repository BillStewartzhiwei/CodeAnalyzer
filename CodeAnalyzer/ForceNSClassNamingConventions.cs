﻿using CodeAnalyzer.CdeAnalyzer;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.CodeAnalysis;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Text;
using System.Linq;

namespace CodeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ForceNSClassNamingConventions : DiagnosticAnalyzer
    {
        /// <summary>
        /// 错误描述
        /// </summary>
        private static readonly DiagnosticDescriptor ForceNamingConventionsDescriptor =
            new DiagnosticDescriptor(
                DianogsticIDs.FORCE_NAMING_CONVENTIONS_ID,          // ID
                "命名空间或类名不符合规范",    // PascalCaseTitle
                "命名空间或类名不符合规范", // Message format
                DiagnosticCategories.Criterion,                // PascalCaseCategory
                DiagnosticSeverity.Error, // Severity
                isEnabledByDefault: true    // Enabled by default
            );
        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(ForceNamingConventionsDescriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(AnalyzeSymbol);
        }

        private static void AnalyzeSymbol(SyntaxTreeAnalysisContext context)
        {
            //找到文档的语法根树
            var root = context.Tree.GetRoot(context.CancellationToken);
            if (ConstraintDefinition.ExcludeAnalize(context.Tree.FilePath))
            {
                //排除特殊目录
                return;
            }
            var classNodeList = root.DescendantNodes()?.OfType<ClassDeclarationSyntax>();
            foreach (var cls in classNodeList)
            {
                var clsName = cls.Identifier.ToString();
                var firstChar = clsName.First().ToString();
                //如果全是小写或全是大写或首字母非大写，则不符合驼峰命名法（粗略检查),复杂的规矩可以自行定义
                if (clsName == clsName.ToLower()
                    || clsName == clsName.ToUpper()
                    || firstChar != firstChar.ToUpper()
                    )
                {
                    //报错
                    var diagnostic = Diagnostic.Create(ForceNamingConventionsDescriptor, cls.GetFirstToken().GetLocation());
                    context.ReportDiagnostic(diagnostic);
                }
            }

            var nsNodeList = root.DescendantNodes()?.OfType<NamespaceDeclarationSyntax>();
            foreach (var ns in nsNodeList)
            {
                var nsName = ns.Name.ToString();
                //拆分命名空间的级段
                var nlist = nsName.Split(new char[] { '.' });
                foreach (var n in nlist)
                {
                    var firstChar = n.First().ToString();
                    //如果首字母非大写，则不符合驼峰命名法（粗略检查),复杂的规矩可以自行定义
                    if (firstChar != firstChar.ToUpper())
                    {
                        //报错
                        var diagnostic = Diagnostic.Create(ForceNamingConventionsDescriptor, ns.GetFirstToken().GetLocation());
                        context.ReportDiagnostic(diagnostic);
                        break;
                    }
                }
            }
        }
    }
}
