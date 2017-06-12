using System.Collections.Immutable;
using Analyzers.CodeAnalysis.AnalyzersMetadata;
using Analyzers.CodeAnalysis.AnalyzersMetadata.DiagnosticIdentifiers;
using Analyzers.Extensions;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;

namespace Analyzers.CodeAnalysis.Async.UseConfigureAwaitFalseWhenPossible
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public sealed class UseConfigureAwaitFalseWhenPossibleDiagnosticAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = AsyncDiagnosticIdentifiers.UseConfigureAwaitFalseWhenPossible;
        private static readonly LocalizableString Title = "Consider using ConfigureAwait(false)";
        private static readonly LocalizableString MessageFormat = "If possible use ConfigureAwait(false)";

        private static readonly DiagnosticDescriptor Rule = new DiagnosticDescriptor(
            DiagnosticId,
            Title,
            MessageFormat,
            DiagnosticCategories.Performance,
            DiagnosticSeverity.Warning,
            isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics => ImmutableArray.Create(Rule);

        public override void Initialize(AnalysisContext context)
        {
            context.EnableConcurrentExecution();
            context.ConfigureGeneratedCodeAnalysis(GeneratedCodeAnalysisFlags.None);
            context.RegisterSyntaxNodeAction(AnalyzeAwaitExpression, SyntaxKind.AwaitExpression);
        }

        private void AnalyzeAwaitExpression(SyntaxNodeAnalysisContext context)
        {
            var result = context.TryGetSyntaxNode<AwaitExpressionSyntax>();
            if (!result.success) return;

            var awaitExpression = result.syntaxNode;
            var model = context.SemanticModel;
            var expression = awaitExpression.Expression;

            var methodSymbol = model.GetSymbolInfo(expression, context.CancellationToken).Symbol as IMethodSymbol;
            if (!methodSymbol.ReturnsTask()) return;

            context.ReportDiagnostic(Diagnostic.Create(Rule, awaitExpression.GetLocation()));
        }
    }
}
