using CodeAnalyzer.CdeAnalyzer;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

namespace CodeAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class ForceMemberVariableConventions : DiagnosticAnalyzer
    {
        private const string PublicVarDescriptorTitle = "Public variables cannot be defined in a class. Use property accessors or methods";
        private const string PublicVarDescriptorMessageFormat = "The variable '{0}' cannot be declared Public";
        private const string PrivateVarDescriptorTitle = "Use '_' for private variables";
        private const string PrivateVarDescriptorMessageFormat = "The variable '{0}' must start with '_'";
        private static readonly DiagnosticDescriptor PublicVarDescriptor =
            new DiagnosticDescriptor(
                DianogsticIDs.FORCE_NAMING_CONVENTIONS_ID,          // ID
                PublicVarDescriptorTitle,    // PascalCaseTitle
                PublicVarDescriptorMessageFormat, // Message format
                DiagnosticCategories.Naming,                // PascalCaseCategory
                DiagnosticSeverity.Error, // Severity
                isEnabledByDefault: true    // Enabled by default
                
            );

        private static readonly DiagnosticDescriptor PrivateVarDescriptor =
            new DiagnosticDescriptor(
                DianogsticIDs.FORCE_NAMING_CONVENTIONS_ID,          // ID
                PrivateVarDescriptorTitle,    // PascalCaseTitle
                PrivateVarDescriptorMessageFormat, // Message format
                DiagnosticCategories.Criterion,                // PascalCaseCategory
                DiagnosticSeverity.Error, // Severity
                isEnabledByDefault: true    // Enabled by default
            );


        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(PublicVarDescriptor,PrivateVarDescriptor);

        public override void Initialize(AnalysisContext context)
        {
            context.RegisterSyntaxTreeAction(VariateCheck);
        }

        private void VariateCheck(SyntaxTreeAnalysisContext context)
        {
            //找到文档的语法根树
            var root = context.Tree.GetRoot(context.CancellationToken);
            if (ConstraintDefinition.ExcludeAnalize(context.Tree.FilePath))
            {
                //排除特殊目录
                return;
            }
            var fieldNodeList = root.DescendantNodes()?.OfType<FieldDeclarationSyntax>();
            foreach (var field in fieldNodeList)
            {
                // 检查字段是否为常量
                if (field.Modifiers.Any(SyntaxKind.ConstKeyword))
                {
                    // 如果是常量，则跳过当前字段
                    continue;
                }
                var filedName = field.Declaration.Variables.ToString();
                var firstChar = filedName.First().ToString();
                var tokens = field.ChildTokens();
                foreach (var token in tokens)
                {
                    //不能包含Public变量(可包含Public变量)
                    /*
                     
                    if (token.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PublicKeyword))
                    {
                        //报错
                        var diagnostic = Diagnostic.Create(PublicVarDescriptor, field.GetFirstToken().GetLocation());
                        context.ReportDiagnostic(diagnostic);
                        break;//只检查一次
                    } 
                     */
                    
                    //else 
                    if (token.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.PrivateKeyword)
                        || token.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.ProtectedKeyword)
                        || token.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.InternalKeyword)
                        || token.IsKind(Microsoft.CodeAnalysis.CSharp.SyntaxKind.IdentifierToken))
                    {
                        //其他:private protected 等等，使用_开头的小驼峰命名法
                        if (firstChar != "_" || filedName == filedName.ToUpper())
                        {
                            //报错
                            var diagnostic = Diagnostic.Create(PrivateVarDescriptor, field.GetFirstToken().GetLocation(), filedName);
                            context.ReportDiagnostic(diagnostic);
                        }
                        break;//只检查一次
                    }
                }
            }
        }
    }
}
