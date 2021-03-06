﻿using System.Collections.Immutable;
using Analyzers.AnalyzersMetadata;
using Analyzers.AnalyzersMetadata.DiagnosticIdentifiers;
using Analyzers.AnalyzersMetadata.DiagnosticMessageFormats;
using Analyzers.AnalyzersMetadata.DiagnosticTitles;
using Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzers.CodeAnalysis.Classes.SetClassAsSealed
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class SetClassAsSealedDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        private const string DiagnosticId = ClassDiagnosticIdentifiers.SetClassAsSealed;
        private static readonly LocalizableString Title = ClassDiagnosticTitles.SetClassAsSealed;
        private static readonly LocalizableString MessageFormat = ClassDiagnosticMessageFormats.SetClassAsSealed;

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            DiagnosticCategories.Simplification,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeClassDeclaration, SyntaxKind.ClassDeclaration);
        }

        private void AnalyzeClassDeclaration(SyntaxNodeAnalysisContext context)
        {
            var result = context.TryGetSyntaxNode<ClassDeclarationSyntax>();
            if (!result.success) return;

            var classDeclaration = result.syntaxNode;

            if (classDeclaration.IsSealed()
                || classDeclaration.IsStatic()
                || classDeclaration.HasAbstractOrVirtualMethods()
                || classDeclaration.HasAbstractOrVirtualProperties()
                || classDeclaration.HasAbstractOrVirtualEvents()
                || classDeclaration.HasAbstractOrVirtualEventFields()
                || classDeclaration.HasAbstractOrVirtualIndexers())
            {
                return;
            }

            context.ReportDiagnostic(Diagnostic.Create(Rule, classDeclaration.Identifier.GetLocation()));
        }
    }
}
