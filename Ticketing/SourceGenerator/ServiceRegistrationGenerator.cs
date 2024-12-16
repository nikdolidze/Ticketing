using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Linq;
using SourceGenerator;

[Generator]
public class ServiceRegistrationGenerator : IIncrementalGenerator
{
    public const string AddSingleton = "AddSingleton";
    public const string AddScoped = "AddScoped";
    public const string AddTransient = "AddTransient";
    public void Initialize(IncrementalGeneratorInitializationContext context)
    {
        var classDeclarations = context.SyntaxProvider
            .CreateSyntaxProvider(
                predicate: (node, _) => IsClassWithRegisterServiceAttribute(node),
                transform: (context, _) => (ClassDeclarationSyntax)context.Node)
            .Where(c => c != null);

        context.RegisterSourceOutput(classDeclarations, GenerateServiceRegistrationCode);
    }

    private static bool IsClassWithRegisterServiceAttribute(SyntaxNode node)
    {
        if (node is ClassDeclarationSyntax classDeclaration)
        {
            return classDeclaration.AttributeLists
                .Any(attrList => attrList.Attributes
                    .Any(attr => attr.Name.ToString().Contains("RegisterService")));
        }

        return false;
    }

    private static void GenerateServiceRegistrationCode(SourceProductionContext context, ClassDeclarationSyntax classDeclaration)
    {
        AttributeSyntax attribute = classDeclaration.AttributeLists
            .SelectMany(list => list.Attributes)
            .FirstOrDefault(attr => attr.Name.ToString().Contains("RegisterService"));
        var serviceTypeExpression = attribute.ArgumentList.Arguments[0].Expression;
        string serviceType = "";
        if (serviceTypeExpression is TypeOfExpressionSyntax typeOfExpression)
        {
            serviceType = typeOfExpression.Type.ToString();
        }

        var lifeTimeArgument = attribute.ArgumentList.Arguments[1].Expression.ToString();
        if (!Enum.TryParse<LifeTime>(lifeTimeArgument.Replace("LifeTime.", ""), out var lifeTime))
        {
            throw new ArgumentException($"Unknown Lifetime: {lifeTimeArgument}");
        }
        
        var registrationCode = CreateServiceRegistrationCode(classDeclaration, serviceType, lifeTime);

        context.AddSource($"{classDeclaration.Identifier.Text}_ServiceRegistration.g.cs", registrationCode);
    }

    private static string CreateServiceRegistrationCode(ClassDeclarationSyntax classDeclaration, string serviceType, LifeTime lifeTime)
    {
        var className = classDeclaration.Identifier.Text;
        var serviceName = serviceType.Trim('"');

        // Ensure the lifetime corresponds to the proper method
        string registrationMethod = lifeTime switch
        {
            LifeTime.Singleton => AddSingleton,
            LifeTime.Scoped => AddScoped,
            LifeTime.Transient => AddTransient,
            _ => throw new ArgumentOutOfRangeException(nameof(lifeTime), lifeTime, null)
        };

        return $@"
using Microsoft.Extensions.DependencyInjection;

namespace GeneratedServiceRegistrations
{{
    public static class {className}ServiceRegistration
    {{
        public static void Register(IServiceCollection services)
        {{
            services.{registrationMethod}<{serviceName}, {className}>();
        }}
    }}
}}";
    }
}