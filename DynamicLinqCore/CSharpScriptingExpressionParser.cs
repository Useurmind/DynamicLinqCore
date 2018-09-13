using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

using Microsoft.CodeAnalysis.CSharp.Scripting;
using Microsoft.CodeAnalysis.Scripting;

namespace DynamicLinqCore
{
    /// <summary>
    /// Interface for a expression parser.
    /// </summary>
    public interface IExpressionParser
    {
        /// <summary>
        /// Parses a lambda expressions that takes an argument of the specified type and returns the stated return value.
        /// The expression is a default C# lambda expression of the form "x => x.IsDeleted == true"
        /// Other examples:
        /// x => x.ChildEntity.Name
        /// x => x.Name == "John"
        /// </summary>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="returnType">Type of the return value.</param>
        /// <param name="expression">The expression to parse.</param>
        /// <param name="otherRequiredTypes">The other required types.</param>
        /// <returns></returns>
        Task<LambdaExpression> ParseLambdaAsync(Type parameterType, Type returnType, string expression, params Type[] otherRequiredTypes);
    }

    public static class ExpressionParserExtensions {

        /// <summary>
        /// Parses a lambda expressions that takes an argument of the specified type and returns the stated return value.
        /// The expression is a default C# lambda expression of the form "x =&gt; x.IsDeleted == true"
        /// Other examples:
        /// x =&gt; x.ChildEntity.Name
        /// x =&gt; x.Name == "John"
        /// </summary>
        /// <typeparam name="TParameter">The type of the parameter.</typeparam>
        /// <typeparam name="TReturn">The type of the return value.</typeparam>
        /// <param name="parser">The parser.</param>
        /// <param name="expression">The expression to parse.</param>
        /// <param name="otherRequiredTypes">The other required types.</param>
        /// <returns></returns>
        public static async Task<Expression<Func<TParameter, TReturn>>> ParseLambdaAsync<TParameter, TReturn>(
            this IExpressionParser parser,
            string expression,
            params Type[] otherRequiredTypes)
        {
            var parsedExpression = await parser.ParseLambdaAsync(typeof(TParameter), typeof(TReturn), expression, otherRequiredTypes);
            return (Expression<Func<TParameter, TReturn>>)parsedExpression;
        }
    }

    /// <summary>
    /// Parses lambda expressions using the roslyn scripting API from the namespace Microsoft.CodeAnalysis.CSharp.Scripting.
    /// </summary>
    public class CSharpScriptingExpressionParser : IExpressionParser
    {
        /// <summary>
        /// Parses a lambda expressions that takes an argument of the specified type and returns the stated return value.
        /// The expression is a default C# lambda expression of the form "x => x.IsDeleted == true"
        /// Other examples:
        /// x => x.ChildEntity.Name
        /// x => x.Name == "John"
        /// </summary>
        /// <param name="parameterType">Type of the parameter.</param>
        /// <param name="returnType">Type of the return value.</param>
        /// <param name="expression">The expression to parse.</param>
        /// <param name="otherRequiredTypes">The other required types.</param>
        /// <returns></returns>
        public async Task<LambdaExpression> ParseLambdaAsync(Type parameterType, Type returnType, string expression, params Type[] otherRequiredTypes)
        {
            var references =
                new [] { parameterType.Assembly, returnType.Assembly, typeof(Func<,>).Assembly, typeof(Expression).Assembly }
                    .Union(otherRequiredTypes?.Select(x => x.Assembly)??new Assembly[0])
                    .ToHashSet();

            var imports = new[] { parameterType.Namespace, returnType.Namespace, "System", "System.Linq.Expressions", }
                          .Union(otherRequiredTypes?.Select(x => x.Namespace)??new string[0])
                          .ToHashSet();

            // we support single level of member classes
            var paramterTypeName = this.GetTypeName(parameterType);
            var returnTypeName = this.GetTypeName(returnType);

            object expressionObject = await CSharpScript.EvaluateAsync(
                $"(Expression<Func<{paramterTypeName}, {returnTypeName}>>)({expression})",
                ScriptOptions.Default
                             .WithReferences(references)
                             .WithImports(imports));

            return (LambdaExpression)expressionObject;
        }

        /// <summary>
        /// Gets the name of the type.
        /// In case of nested type we need some little logic to determine the correct name.
        /// But we only support single level of nested types currently.
        /// </summary>
        /// <param name="type">The type to get the name of.</param>
        /// <returns>The name of the type that can be used in the lambda expression.</returns>
        /// <exception cref="ArgumentException"></exception>
        private string GetTypeName(Type type)
        {
            if (!type.IsNestedPublic && !type.IsPublic)
            {
                throw new ArgumentException($"The type {type.FullName} is a (nested) private type and cannot be used in parsed expressions");
            }

            // we support single level of member classes
            var typeName = type.DeclaringType != null
                ? $"{type.DeclaringType.Name}.{type.Name}"
                : type.Name;

            // if the type is nullable we need to compose the correct type from the generic type arguments
            if (typeName == "Nullable`1")
            {
                typeName = typeName.Replace(
                    "`1",
                    $"<{type.GenericTypeArguments[0].FullName}>");
            }

            return typeName;
        }
    }
}
