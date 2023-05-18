using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SqliteDna.SourceGenerator;

namespace TestGenerator
{
    public class TestFullTypeName
    {
        [Fact]
        public void NamespaceForClass()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"namespace Namespace1	{ class Type1 {}}");
            var compilation = CSharpCompilation.Create("Test").AddSyntaxTrees(tree);

            ITypeSymbol type = compilation.GetSemanticModel(tree).GetDeclaredSymbol(tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>().First())!;
            Assert.Equal("Namespace1.Type1", Util.GetFullTypeName(type));
        }

        [Fact]
        public void Namespace2ForStruct()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"namespace Namespace1	{ namespace Namespace2 { struct Type1 {}}}");
            var compilation = CSharpCompilation.Create("Test").AddSyntaxTrees(tree);

            ITypeSymbol type = compilation.GetSemanticModel(tree).GetDeclaredSymbol(tree.GetRoot().DescendantNodes().OfType<StructDeclarationSyntax>().First())!;
            Assert.Equal("Namespace1.Namespace2.Type1", Util.GetFullTypeName(type));
        }

        [Fact]
        public void NamespaceClassForRecord()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"namespace Namespace1	{ class Class1 { record Type1 {}}}");
            var compilation = CSharpCompilation.Create("Test").AddSyntaxTrees(tree);

            var records = tree.GetRoot().DescendantNodes().OfType<RecordDeclarationSyntax>();
            var recordDeclaration = records.First(i => i.Identifier.ToString() == "Type1");
            ITypeSymbol type = compilation.GetSemanticModel(tree).GetDeclaredSymbol(recordDeclaration)!;
            Assert.Equal("Namespace1.Class1.Type1", Util.GetFullTypeName(type));
        }
    }
}