using System.Collections.Generic;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace AutoNotifyGenerator.Receivers;

internal sealed class FieldSyntaxReceiver : ISyntaxReceiver
{
    public List<FieldDeclarationSyntax> CandidateFields { get; } = [];

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is FieldDeclarationSyntax field && field.AttributeLists.Count > 0)
        {
            CandidateFields.Add(field);
        }
    }
}
