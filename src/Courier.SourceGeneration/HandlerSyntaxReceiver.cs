using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Courier;

internal class HandlerSyntaxReceiver : ISyntaxReceiver
{
    public List<ClassDeclarationSyntax> Handlers { get; } =  new();

    public void OnVisitSyntaxNode(SyntaxNode syntaxNode)
    {
        if (syntaxNode is not ClassDeclarationSyntax cds)
            return;
            Handlers.Add(cds);
    }
}