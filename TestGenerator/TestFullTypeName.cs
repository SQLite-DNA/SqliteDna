using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using SqliteDna.SourceGenerator;

namespace TestGenerator
{
    public class TestFullTypeName
    {
        [Fact]
        public void Namespace()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"namespace Namespace1	{ class Type1 {}}");
            var compilation = CSharpCompilation.Create("Test").AddSyntaxTrees(tree);

            var classes = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();
            var classDeclaration = classes.First();
            ITypeSymbol type = compilation.GetSemanticModel(tree).GetDeclaredSymbol(classDeclaration)!;

            Assert.Equal("Namespace1.Type1", Util.GetFullTypeName(type));
        }

        [Fact]
        public void Namespace2()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"namespace Namespace1	{ namespace Namespace2 { class Type1 {}}}");
            var compilation = CSharpCompilation.Create("Test").AddSyntaxTrees(tree);

            var classes = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();
            var classDeclaration = classes.First();
            ITypeSymbol type = compilation.GetSemanticModel(tree).GetDeclaredSymbol(classDeclaration)!;

            Assert.Equal("Namespace1.Namespace2.Type1", Util.GetFullTypeName(type));
        }

        [Fact]
        public void NamespaceClass()
        {
            SyntaxTree tree = CSharpSyntaxTree.ParseText(@"namespace Namespace1	{ class Class1 { class Type1 {}}}");
            var compilation = CSharpCompilation.Create("Test").AddSyntaxTrees(tree);

            var classes = tree.GetRoot().DescendantNodes().OfType<ClassDeclarationSyntax>();
            var classDeclaration = classes.First(i => i.Identifier.ToString() == "Type1");
            ITypeSymbol type = compilation.GetSemanticModel(tree).GetDeclaredSymbol(classDeclaration)!;

            //Assert.Equal("Namespace1.Class1.Type1", Util.GetFullTypeName(type));
        }
    }
}