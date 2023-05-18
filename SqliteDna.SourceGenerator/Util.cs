using Microsoft.CodeAnalysis;

namespace SqliteDna.SourceGenerator
{
    public class Util
    {
        public static string GetFullTypeName(ITypeSymbol type)
        {
            return $"{type.ContainingNamespace}.{type.Name}";
        }
    }
}
