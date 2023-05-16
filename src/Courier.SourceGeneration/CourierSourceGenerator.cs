using System.Diagnostics;
using System.Text;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Text;

namespace Courier;

[Generator]
public class CourierSourceGenerator : ISourceGenerator
{
    public void Initialize(GeneratorInitializationContext context)
    {
        if (!Debugger.IsAttached)
        {
            //Debugger.Launch();
        }
        context.RegisterForSyntaxNotifications(() => new HandlerSyntaxReceiver());
    }

    public void Execute(GeneratorExecutionContext context)
    {
        var namespaces = new List<string>();
        var servicesSb = new StringBuilder();
        var appSb = new StringBuilder();
        
        var handlerSyntaxReceiver = (HandlerSyntaxReceiver?)context.SyntaxReceiver;
        foreach (var handler in handlerSyntaxReceiver?.Handlers ?? Enumerable.Empty<ClassDeclarationSyntax>())
        {
            var baseType = handler.BaseList?.Types
                .FirstOrDefault(t => t.Type is GenericNameSyntax { Identifier.Text: "ICommandHandler" or "IQueryHandler" });
            
            if (baseType is null)
                continue;
            
            // get handler attribute of type EndpointAttribute and get http method and route template
            var handlerAttribute = handler.AttributeLists
                .SelectMany(al => al.Attributes)
                .FirstOrDefault(a => a.Name.ToString() == "Endpoint");

            if (handlerAttribute == null)
                continue;
            
            var baseTypeArgs = baseType.Type is GenericNameSyntax { TypeArgumentList: { Arguments: { Count: > 0 } args } }
                ? args
                : throw new InvalidOperationException("Base type must have at least one type argument.");
            
            var isCommandHandler = baseType.Type is GenericNameSyntax { Identifier.Text: "ICommandHandler" };
            var isQueryHandler = baseType.Type is GenericNameSyntax { Identifier.Text: "IQueryHandler" };
            
            string? idType = null;
            string? requestType = null;
            string? responseType = null;
            
            // assign id type
            if(isCommandHandler && baseTypeArgs.Count == 3 || isQueryHandler && baseTypeArgs.Count == 2)
                idType = baseTypeArgs[0].ToString();
            
            // assign request type
            if (isCommandHandler && baseTypeArgs.Count == 3)
                requestType = baseTypeArgs[1].ToString();
            if (isCommandHandler && baseTypeArgs.Count == 2)
                requestType = baseTypeArgs[0].ToString();
            
            // assign response type
            if (isCommandHandler && baseTypeArgs.Count == 3)
                responseType = baseTypeArgs[2].ToString();
            if(isCommandHandler && baseTypeArgs.Count == 2)
                responseType = baseTypeArgs[1].ToString();
            if(isQueryHandler && baseTypeArgs.Count == 2)
                responseType = baseTypeArgs[1].ToString();
            if(isQueryHandler && baseTypeArgs.Count == 1)
                responseType = baseTypeArgs[0].ToString();


            // get namespaces
            var handlerNamespace = GetNamespace( handler);
            var idNamespace = GetNamespace(ref context, idType);
            var requestNamespace = GetNamespace(ref context, requestType);
            var responseNamespace = GetNamespace(ref context, responseType);
            
            // add namespaces
            AddNamespace(ref namespaces, handlerNamespace);
            AddNamespace(ref namespaces, idNamespace);
            AddNamespace(ref namespaces, requestNamespace);
            AddNamespace(ref namespaces, responseNamespace);
            
            var handlerName = handler.Identifier.Text;

            var _httpMethod = handlerAttribute?.ArgumentList?.Arguments[0]
                ?.Expression.ToString();

            var httpMethod  = _httpMethod switch
            {
                "HttpMethods.Get" => "Get",
                "HttpMethods.Post" => "Post",
                "HttpMethods.Put" => "Put",
                "HttpMethods.Delete" => "Delete",
                "HttpMethods.Patch" => "Patch",
                "HttpMethods.Head" => "Head",
                "HttpMethods.Options" => "Options",
                _ => "Get"
            };  
            
            var routeTemplate = handlerAttribute?.ArgumentList?.Arguments[1]
                ?.Expression.ToString() ?? "/";
            
            // check if AllowAnonymous attribute is present on handler class
            var isAllowAnonymous = handler.AttributeLists
                .SelectMany(al => al.Attributes)
                .Any(a => a.Name.ToString() == "AllowAnonymous");
            
            // check if Authorize attribute is present on handler class and get policy and roles
            var authorizeAttribute = handler.AttributeLists
                .SelectMany(al => al.Attributes)
                .FirstOrDefault(a => a.Name.ToString() == "Authorize");
            
            // var policy = authorizeAttribute?.ArgumentList?.Arguments[0]
            //     ?.Expression.ToString();
            //
            // var roles = authorizeAttribute?.ArgumentList?.Arguments[1]
            //     ?.Expression.ToString();
            
            string? policy = null;
            string? roles = null;
            string? schemes = null;
            
            var hasPositionalArgs = authorizeAttribute?.ArgumentList?.Arguments.FirstOrDefault(x => x.NameEquals == null) != null;

            if (hasPositionalArgs)
            {
                policy = authorizeAttribute?.ArgumentList?.Arguments[0]
                    ?.Expression.ToString();
            }
            else
            {
                policy = authorizeAttribute?.ArgumentList?.Arguments
                    .FirstOrDefault(a => a?.NameEquals?.Name.ToString() == "Policy")?.Expression.ToString();
            }
            
            roles = authorizeAttribute?.ArgumentList?.Arguments
                .FirstOrDefault(a => a?.NameEquals?.Name.ToString() == "Roles")?.Expression.ToString();
            
            schemes = authorizeAttribute?.ArgumentList?.Arguments
                .FirstOrDefault(a => a?.NameEquals?.Name.ToString() == "AuthenticationSchemes")?.Expression.ToString();
            
            
            var methodHandleAsync = handler.DescendantNodes()
                .OfType<MethodDeclarationSyntax>()
                .FirstOrDefault(m => m.Identifier.Text == "HandleAsync");
            
            // get method parameters attribute like [FromRoute], [FromQuery], [FromBody], type and param name
            var methodParams = methodHandleAsync?.ParameterList.Parameters
                .Select(p => new
                {
                    Attribute = p.AttributeLists
                        .SelectMany(al => al.Attributes),
                    Type = p?.Type?.ToString(),
                    Name = p?.Identifier.Text
                }).ToList();
            
            // generate signature like ([FromRoute] int id, [FromBody] string request)
            var signature = methodParams?.Aggregate(new StringBuilder(), (sb, p) =>
            {
                var attr = p.Attribute.FirstOrDefault()?.Name.ToString();
                if (attr is not null)
                    sb.Append($"[{attr}] ");
                sb.Append($"{p.Type} {p.Name}, ");
                return sb;
            }, sb => sb.ToString().TrimEnd(',', ' '));
            




            var attrSb = new StringBuilder();
            if (isAllowAnonymous)
            {
                attrSb.Append("[AllowAnonymous]");
            }
            
            if (authorizeAttribute is not null)
            {
                attrSb.Append("[Authorize");
                if (policy is not null)
                    attrSb.Append($"(Policy = {policy})");
                
                if (roles is not null)
                    attrSb.Append($"(Roles = {roles})");
                
                if (schemes is not null)
                    attrSb.Append($"(AuthenticationSchemes = {schemes})");
                attrSb.Append("]");
            }



            if (isCommandHandler && idType is null)
            {
                appSb.Append($"app.Map{httpMethod}({routeTemplate}, {attrSb}({signature}, [FromServices] {handlerName} handler) => handler.HandleAsync(request, ctx));");
            }

            if (isCommandHandler && idType is not null)
            {
                appSb.Append($"app.Map{httpMethod}({routeTemplate}, {attrSb}({signature}, [FromServices] {handlerName} handler) => handler.HandleAsync(id, request, ctx));");
            }

            if (isQueryHandler && idType is null)
            {
                appSb.Append($"app.Map{httpMethod}({routeTemplate}, {attrSb}({signature}, [FromServices] {handlerName} handler) => handler.HandleAsync(ctx));");

            }

            if (isQueryHandler && idType is not null)
            {
                appSb.Append($"app.Map{httpMethod}({routeTemplate}, {attrSb}({signature}, [FromServices] {handlerName} handler) => handler.HandleAsync(id, ctx));");
            }

            appSb.AppendLine();

            servicesSb.AppendLine($"services.AddTransient<{handlerName}>();");

        }

        
        context.AddSource("Courier.g.cs", SourceText.From($$"""
        using Microsoft.AspNetCore.Mvc;
        using Microsoft.AspNetCore.Authorization;
        {{string.Join("\n", namespaces.ToArray())}}
        
        namespace Courier
        {
            public static class Extensions
            {
                public static IServiceCollection AddCourier(this IServiceCollection services)
                {
                    {{servicesSb}}
                    return services;
                }

                public static WebApplication UseCourier(this WebApplication app)
                {
                    {{appSb}}
                    return app;
                }
            }
        }
        """, Encoding.UTF8));
    }


    private string? GetNamespace( ClassDeclarationSyntax cds)
    {
      
            return cds.Ancestors().OfType<NamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString() ??
                   cds.Ancestors().OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString();
    }

    private string? GetNamespace(ref GeneratorExecutionContext context, string? identifier)
    {
        if (identifier == null)
        {
            return null;
        }

        var syntaxNodes = context.Compilation.SyntaxTrees
                .FirstOrDefault(x => x.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>()
                    .Any(c => c.Identifier.Text == identifier))?.GetRoot().DescendantNodes();

        if(syntaxNodes == null)
            return null;

        return syntaxNodes.OfType<NamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString() ??
                    syntaxNodes.OfType<FileScopedNamespaceDeclarationSyntax>().FirstOrDefault()?.Name.ToString();

    }

    private void AddNamespace(ref List<string> namespaces, string? @namespace)
    {
        
        if (@namespace is null)
            return;

        var ns = $"using {@namespace};";

        if (!namespaces.Contains(ns))
            namespaces.Add(ns);
    }
}