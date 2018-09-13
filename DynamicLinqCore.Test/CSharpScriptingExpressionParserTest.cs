using System;
using System.Collections.Generic;
using System.Text;

using Xunit;

namespace DynamicLinqCore.Test
{
    public class CSharpScriptingExpressionParserTest
    {
        public class MyEntity
        {
            public int Id { get; set; }

            public int? NullableInt { get; set; }

            public string Name { get; set; }

            public MyEntity ChildEntity { get; set; }
        }

        /// <summary>
        /// Check that <see cref="CSharpScriptingExpressionParser.ParseLambdaAsync" />can parse a predicate on
        /// a single string property.
        /// </summary>
        [Fact]
        public void ParseLambdaAsync_ParsesPredicateOnStringPropertyCorrectly()
        {
            var parser = new CSharpScriptingExpressionParser();

            var expression = parser.ParseLambdaAsync<MyEntity, bool>("x => x.Id == 1").Result;
            var expressionCompiled = expression.Compile();

            Assert.True(
                expressionCompiled(
                    new MyEntity()
                    {
                        Id = 1
                    }));

            Assert.False(
                expressionCompiled(
                    new MyEntity()
                    {
                        Id = 2
                    }));
        }

        /// <summary>
        /// Check that <see cref="CSharpScriptingExpressionParser.ParseLambdaAsync" /> can parse a combined predicate.
        /// </summary>
        [Fact]
        public void ParseLambdaAsync_ParsesCombinedPredicateCorrectly()
        {
            var parser = new CSharpScriptingExpressionParser();

            var expression = parser.ParseLambdaAsync<MyEntity, bool>("x => x.Id == 1 && x.Name == \"John\"")
                                   .Result;
            var expressionCompiled = expression.Compile();

            Assert.True(
                expressionCompiled(
                    new MyEntity()
                    {
                        Id = 1,
                        Name = "John"
                    }));

            Assert.False(
                expressionCompiled(
                    new MyEntity()
                    {
                        Id = 2,
                        Name = "John"
                    }));

            Assert.False(
                expressionCompiled(
                    new MyEntity()
                    {
                        Id = 1,
                        Name = "LittleJohn"
                    }));
        }

        /// <summary>
        /// Check that <see cref="CSharpScriptingExpressionParser.ParseLambdaAsync" /> can parse a path to a child
        /// entity.
        /// </summary>
        [Fact]
        public void ParseLambdaAsync_ParsesChildEntityPathCorrectly()
        {
            var parser = new CSharpScriptingExpressionParser();

            var expression = parser.ParseLambdaAsync<MyEntity, MyEntity>("x => x.ChildEntity.ChildEntity")
                                   .Result;
            var expressionCompiled = expression.Compile();

            var parentEntity = new MyEntity()
            {
                Id = 1,
                Name = "John",
                ChildEntity = new MyEntity()
                {
                    ChildEntity = new MyEntity()
                }
            };

            var childChildEntity = parentEntity.ChildEntity.ChildEntity;

            Assert.Same(expressionCompiled(parentEntity), childChildEntity);
        }

        /// <summary>
        /// Check that <see cref="CSharpScriptingExpressionParser.ParseLambdaAsync" /> can parse a path to a nullable
        ///  int property.
        /// </summary>
        [Fact]
        public void ParseLambdaAsync_ParsesNullableIntPathCorrectly()
        {
            var parser = new CSharpScriptingExpressionParser();

            var expression = parser.ParseLambdaAsync<MyEntity, int?>("x => x.NullableInt")
                                   .Result;
            var expressionCompiled = expression.Compile();

            var entity1 = new MyEntity()
            {
                NullableInt = 4
            };

            var entity2 = new MyEntity()
            {
                NullableInt = null
            };

            Assert.Equal(expressionCompiled(entity1), entity1.NullableInt);
            Assert.Equal(expressionCompiled(entity2), entity2.NullableInt);
        }
    }
}
