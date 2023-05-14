﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;
using System.Linq;

namespace SqliteDna.SourceGenerator
{
    [Generator]
    public class Generator : ISourceGenerator
    {
        public void Execute(GeneratorExecutionContext context)
        {
            if (!(context.SyntaxContextReceiver is SyntaxReceiver receiver))
                return;

            context.AnalyzerConfigOptions.GlobalOptions.TryGetValue("build_property.PublishAOT", out string? publishAOT);

            string source = """
// <auto-generated/>
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace SqliteDna.SourceGenerator
{
    public class Init
    {
        [UnmanagedCallersOnly(EntryPoint = "sqlite3_[ASSEMBLY-NAME]_init", CallConvs = new[] { typeof(CallConvCdecl) })]
        public unsafe static int sqlite3_sqlitedna_init(IntPtr db, byte** pzErrMsg, IntPtr pApi)
        {
            SqliteDna.Integration.Sqlite.Init(db, pzErrMsg, pApi);

[CREATE_FUNCTIONS]
[CREATE_TABLE_FUNCTIONS]
            return SqliteDna.Integration.Sqlite.SQLITE_OK;
        }
    }

    public class Functions
    {[FUNCTIONS]
    }
}
""";
            string assemblyName = context.Compilation.AssemblyName!;
            string assemblyNameSuffix = string.Compare(publishAOT, "true", true) == 0 ? "" : "ne";
            source = source.Replace("[ASSEMBLY-NAME]", assemblyName.ToLower() + assemblyNameSuffix);

            string functions = "";
            string createFunctions = "";
            foreach (var i in receiver.Functions)
            {
                string functionBody = """

                    [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
                    public static unsafe int [FUNCTION_NAME](IntPtr context, int argc, IntPtr argv)
                    {
                        IntPtr* values = (IntPtr*)argv;

                        try
                        {
                            [FUNCTION_CALL_AND_RESULT]
                        }
                        catch(Exception e)
                        {
                            SqliteDna.Integration.Sqlite.ResultError(context, e.Message);
                        }
                        
                        return SqliteDna.Integration.Sqlite.SQLITE_OK;
                    }
                    """;

                string functionValues = "";
                int valueIndex = 0;
                foreach (var p in i.Parameters)
                {
                    if (valueIndex > 0)
                        functionValues += ", ";
                    functionValues += $"SqliteDna.Integration.Sqlite.Value{AdaptType(p.Type)}(values, {valueIndex})";
                    ++valueIndex;
                }

                if (i.ReturnsVoid)
                {
                    string result = "[FULL_FUNCTION_NAME]([FUNCTION_VALUES]);";
                    functionBody = functionBody.Replace("[FUNCTION_CALL_AND_RESULT]", result);
                }
                else
                {
                    string result = "SqliteDna.Integration.Sqlite.Result[RESULT_TYPE](context, [FULL_FUNCTION_NAME]([FUNCTION_VALUES]));";
                    result = result.Replace("[RESULT_TYPE]", AdaptType(i.ReturnType));
                    functionBody = functionBody.Replace("[FUNCTION_CALL_AND_RESULT]", result);
                }

                functionBody = functionBody.Replace("[FUNCTION_VALUES]", functionValues);
                functionBody = functionBody.Replace("[FUNCTION_NAME]", i.Name);
                functionBody = functionBody.Replace("[FULL_FUNCTION_NAME]", $"{i.ContainingType.ContainingNamespace}.{i.ContainingType.Name}.{i.Name}");
                functions += functionBody;
                createFunctions += $"            SqliteDna.Integration.Sqlite.CreateFunction(\"{i.Name}\", {i.Parameters.Length}, &Functions.{i.Name});\r\n";
            }

            string createTableFunctions = "";
            foreach (var i in receiver.TableFunctions)
            {
                string fullFunctionName = $"{i.ContainingType.ContainingNamespace}.{i.ContainingType.Name}.{i.Name}";

                string properties = "";
                ITypeSymbol? elementType = GetGenericArgument(i.ReturnType);
                if (elementType != null)
                {
                    string fullTypeName = $"{elementType.ContainingNamespace}.{elementType.Name}";
                    foreach (string p in GetPropertyNames(elementType))
                        properties += $"typeof({fullTypeName}).GetProperty(\"{p}\")!,";
                }

                createTableFunctions += $"            SqliteDna.Integration.Sqlite.CreateModule(\"{i.Name}\", new System.Reflection.PropertyInfo[] {{{properties}}}, () => {fullFunctionName}());\r\n";
            }

            source = source.Replace("[FUNCTIONS]", functions);
            source = source.Replace("[CREATE_FUNCTIONS]", createFunctions);
            source = source.Replace("[CREATE_TABLE_FUNCTIONS]", createTableFunctions);

            context.AddSource($"SqliteDna.Init.g.cs", source);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        private static ITypeSymbol? GetGenericArgument(ITypeSymbol enumerable)
        {
            if (enumerable is INamedTypeSymbol namedEnumerable)
                return namedEnumerable.TypeArguments.FirstOrDefault();

            return null;
        }

        private static IEnumerable<string> GetPropertyNames(ITypeSymbol elementType)
        {
            List<string> result = new List<string>();
            if (AdaptType(elementType) == null)
            {
                foreach (ISymbol member in elementType.GetMembers())
                {
                    if (member.DeclaredAccessibility == Accessibility.Public && !member.IsImplicitlyDeclared)
                    {
                        if (member is IPropertySymbol property)
                            result.Add(property.Name);
                    }
                }
            }

            return result;
        }

        private static string? AdaptType(ITypeSymbol typeSymbol)
        {
            switch (typeSymbol.SpecialType)
            {
                case SpecialType.System_Int32:
                    return "Int";
                case SpecialType.System_Int64:
                    return "Int64";
                case SpecialType.System_Double:
                    return "Double";
                case SpecialType.System_String:
                    return "String";
                case SpecialType.System_DateTime:
                    return "DateTime";
            }

            if (typeSymbol is IArrayTypeSymbol arrayTypeSymbol)
            {
                if (arrayTypeSymbol.ElementType.SpecialType == SpecialType.System_Byte)
                    return "Blob";
            }

            return null;
        }

        class SyntaxReceiver : ISyntaxContextReceiver
        {
            public List<IMethodSymbol> Functions { get; } = new List<IMethodSymbol>();
            public List<IMethodSymbol> TableFunctions { get; } = new List<IMethodSymbol>();

            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                if (context.Node is MethodDeclarationSyntax methodSyntax)
                {
                    IMethodSymbol methodSymbol = (context.SemanticModel.GetDeclaredSymbol(methodSyntax) as IMethodSymbol)!;
                    if (methodSymbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString(fullNameFormat) == "SqliteDna.Integration.FunctionAttribute"))
                        Functions.Add(methodSymbol);
                    if (methodSymbol.GetAttributes().Any(a => a.AttributeClass?.ToDisplayString(fullNameFormat) == "SqliteDna.Integration.SqliteTableFunctionAttribute"))
                        TableFunctions.Add(methodSymbol);
                }
            }

            private static SymbolDisplayFormat fullNameFormat = new SymbolDisplayFormat(typeQualificationStyle: SymbolDisplayTypeQualificationStyle.NameAndContainingTypesAndNamespaces);
        }
    }
}
