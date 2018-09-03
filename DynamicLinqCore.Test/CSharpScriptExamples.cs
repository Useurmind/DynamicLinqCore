using System;

using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

using Xunit;

namespace DynamicLinqCore.Test
{
    /// <summary>
    /// Examples on how to use the scripting api
    /// https://github.com/dotnet/roslyn/wiki/Scripting-API-Samples
    /// </summary>
    public class CSharpScriptExamples
    {
        public class MyEntity
        {
            public int Id { get; set; }

            public string Name { get; set; }

            public MyEntity ChildEntity { get; set; }
        }

        [Fact]
        public void SimpleHelloWorldFunc()
        {
            // https://github.com/dotnet/roslyn/wiki/Scripting-API-Samples
            Func<string> getString = () => "Hello World";
            var getStringCompiled = (Func<string>)CSharpScript.EvaluateAsync(
                "(Func<string>)(() => \"Hello World\")",
                ScriptOptions.Default.WithImports("System")).Result;

            Assert.Equal(getString(), getStringCompiled());
        }

        [Fact]
        public void PredicateOnCustomObject()
        {
            var myEntity = new MyEntity() {Id = 1, Name = "Erich"};
            var myEntity2 = new MyEntity() { Id = 1, Name = "Stefan" };

            Func<MyEntity, bool> testEntity = e => e.Name == "Erich";
            var testEntityCompiled = (Func<MyEntity, bool>)CSharpScript.EvaluateAsync(
                "(Func<CSharpScriptExamples.MyEntity, bool>)(e => e.Name == \"Erich\")",
                ScriptOptions.Default
                             .WithReferences(typeof(MyEntity).Assembly, typeof(Func<,>).Assembly)
                             .WithImports("System", typeof(MyEntity).Namespace)).Result;

            Assert.True(testEntity(myEntity));
            Assert.False(testEntity(myEntity2));
            Assert.Equal(testEntity(myEntity), testEntityCompiled(myEntity));
            Assert.Equal(testEntity(myEntity2), testEntityCompiled(myEntity2));
        }

        [Fact]
        public void RelationPath()
        {
            var myEntity = new MyEntity() { Id = 1,  ChildEntity = new MyEntity()};

            Func<MyEntity, MyEntity> getChild = e => e.ChildEntity;
            var getChildCompiled = (Func<MyEntity, MyEntity>)CSharpScript.EvaluateAsync(
                "(Func<CSharpScriptExamples.MyEntity, CSharpScriptExamples.MyEntity>)(e => e.ChildEntity)",
                ScriptOptions.Default
                             .WithReferences(typeof(MyEntity).Assembly, typeof(Func<,>).Assembly)
                             .WithImports("System", typeof(MyEntity).Namespace)).Result;

            Assert.Same(getChild(myEntity), getChildCompiled(myEntity));
        }
    }
}
