﻿using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System;
using System.Collections.Generic;

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
                        IntPtr* pValues = (IntPtr*)argv;

                        SqliteDna.Integration.Sqlite.ResultDouble(context, [FULL_FUNCTION_NAME]());
                        return SqliteDna.Integration.Sqlite.SQLITE_OK;
                    }
                    """;
                functionBody = functionBody.Replace("[FUNCTION_NAME]", i.Name);
                functionBody = functionBody.Replace("[FULL_FUNCTION_NAME]", $"{i.ContainingType.ContainingNamespace}.{i.ContainingType.Name}.{i.Name}");
                functions += functionBody;
                createFunctions += $"            SqliteDna.Integration.Sqlite.CreateFunction(\"{i.Name}\", 0, &Functions.{i.Name});\r\n";
            }
            source = source.Replace("[FUNCTIONS]", functions);
            source = source.Replace("[CREATE_FUNCTIONS]", createFunctions);

            context.AddSource($"SqliteDna.Init.g.cs", source);
        }

        public void Initialize(GeneratorInitializationContext context)
        {
            context.RegisterForSyntaxNotifications(() => new SyntaxReceiver());
        }

        class SyntaxReceiver : ISyntaxContextReceiver
        {
            public List<IMethodSymbol> Functions { get; } = new List<IMethodSymbol>();

            public void OnVisitSyntaxNode(GeneratorSyntaxContext context)
            {
                if (context.Node is MethodDeclarationSyntax methodDeclarationSyntax)
                {
                    Functions.Add((context.SemanticModel.GetDeclaredSymbol(methodDeclarationSyntax) as IMethodSymbol)!);
                }
            }
        }
    }
}
