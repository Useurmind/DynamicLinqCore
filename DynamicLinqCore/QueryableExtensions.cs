using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Schema;

namespace DynamicLinqCore
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Where<T>(this IQueryable<T> source, string predicate)
        {
            return (IQueryable<T>)Where((IQueryable)source, predicate);
        }

        public static IQueryable<T> Where<T>(this IQueryable<T> source, string predicate, Type[] otherRequiredTypes)
        {
            return (IQueryable<T>)Where((IQueryable)source, predicate, otherRequiredTypes);
        }

        public static IQueryable Where(this IQueryable source, string predicate)
        {
            return source.Where(predicate, null);
        }

        public static IQueryable Where(this IQueryable source, string predicate, Type[] otherRequiredTypes)
        {
            Ensure.ArgumentNotNull(source, "source");
            Ensure.ArgumentNotNull(predicate, "predicate");

            var parser = new CSharpScriptingExpressionParser();
            var expression = parser.ParseLambdaAsync(source.ElementType, typeof(bool), predicate, otherRequiredTypes).Result;

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "Where",
                    new Type[] { source.ElementType },
                    source.Expression,
                    Expression.Quote(expression)));
        }

        //public static IQueryable Select(this IQueryable source, string selector, params object[] values)
        //{
        //    Ensure.ArgumentNotNull(source, "source");
        //    Ensure.ArgumentNotNull(selector, "selector");

        //    LambdaExpression lambda = DynamicExpression.ParseLambda(source.ElementType, null, selector, values);
        //    return source.Provider.CreateQuery(
        //        Expression.Call(
        //            typeof(Queryable),
        //            "Select",
        //            new Type[] { source.ElementType, lambda.Body.Type },
        //            source.Expression,
        //            Expression.Quote(lambda)));
        //}

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering, Type orderedPropertyType)
        {
            return (IQueryable<T>)OrderBy((IQueryable)source, ordering, orderedPropertyType, null);
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering, Type orderedPropertyType, Type[] otherRequiredTypes)
        {
            return (IQueryable<T>)OrderBy((IQueryable)source, ordering, orderedPropertyType, otherRequiredTypes);
        }

        public static IQueryable OrderBy(this IQueryable source, string ordering, Type orderedPropertyType)
        {
            return OrderBy(source, ordering, orderedPropertyType, null);
        }

        public static IQueryable OrderBy(this IQueryable source, string ordering, Type orderedPropertyType, Type[] otherRequiredTypes)
        {
            Ensure.ArgumentNotNull(source, "source");
            Ensure.ArgumentNotNull(ordering, "ordering");
            Ensure.ArgumentNotNull(orderedPropertyType, "orderedPropertyType");

            var parser = new CSharpScriptingExpressionParser();
            var expression = parser.ParseLambdaAsync(source.ElementType, orderedPropertyType, ordering, otherRequiredTypes).Result;

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "OrderBy",
                    new Type[] { source.ElementType, orderedPropertyType },
                    source.Expression,
                    Expression.Quote(expression)));
        }

        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string ordering, Type orderedPropertyType)
        {
            return (IQueryable<T>)OrderByDescending((IQueryable)source, ordering, orderedPropertyType, null);
        }

        public static IQueryable<T> OrderByDescending<T>(this IQueryable<T> source, string ordering, Type orderedPropertyType, Type[] otherRequiredTypes)
        {
            return (IQueryable<T>)OrderByDescending((IQueryable)source, ordering, orderedPropertyType, otherRequiredTypes);
        }

        public static IQueryable OrderByDescending(this IQueryable source, string ordering, Type orderedPropertyType)
        {
            return OrderByDescending(source, ordering, orderedPropertyType, null);
        }

        public static IQueryable OrderByDescending(this IQueryable source, string ordering, Type orderedPropertyType, Type[] otherRequiredTypes)
        {
            Ensure.ArgumentNotNull(source, "source");
            Ensure.ArgumentNotNull(ordering, "ordering");
            Ensure.ArgumentNotNull(orderedPropertyType, "orderedPropertyType");

            var parser = new CSharpScriptingExpressionParser();
            var expression = parser.ParseLambdaAsync(source.ElementType, orderedPropertyType, ordering, otherRequiredTypes).Result;

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "OrderByDescending",
                    new Type[] { source.ElementType, orderedPropertyType },
                    source.Expression,
                    Expression.Quote(expression)));
        }

        public static IQueryable Take(this IQueryable source, int count)
        {
            Ensure.ArgumentNotNull(source, "source");

            // root to original entity framework method
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "Take",
                    new Type[] { source.ElementType },
                    source.Expression,
                    Expression.Constant(count)));
        }

        public static IQueryable Skip(this IQueryable source, int count)
        {
            Ensure.ArgumentNotNull(source, "source");

            // root to original entity framework method
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "Skip",
                    new Type[] { source.ElementType },
                    source.Expression,
                    Expression.Constant(count)));
        }

        //public static IQueryable GroupBy(
        //    this IQueryable source,
        //    string keySelector,
        //    string elementSelector,
        //    params object[] values)
        //{
        //    Ensure.ArgumentNotNull(source, "source");
        //    Ensure.ArgumentNotNull(keySelector, "keySelector");
        //    Ensure.ArgumentNotNull(elementSelector, "elementSelector");

        //    LambdaExpression keyLambda = DynamicExpression.ParseLambda(source.ElementType, null, keySelector, values);
        //    LambdaExpression elementLambda = DynamicExpression.ParseLambda(
        //        source.ElementType,
        //        null,
        //        elementSelector,
        //        values);
        //    return source.Provider.CreateQuery(
        //        Expression.Call(
        //            typeof(Queryable),
        //            "GroupBy",
        //            new Type[] { source.ElementType, keyLambda.Body.Type, elementLambda.Body.Type },
        //            source.Expression,
        //            Expression.Quote(keyLambda),
        //            Expression.Quote(elementLambda)));
        //}

        //public static bool Any(this IQueryable source)
        //{
        //    Ensure.ArgumentNotNull(source, "source");

        //    return (bool)source.Provider.Execute(
        //        Expression.Call(
        //            typeof(Queryable),
        //            "Any",
        //            new Type[] { source.ElementType },
        //            source.Expression));
        //}

        //public static int Count(this IQueryable source)
        //{
        //    Ensure.ArgumentNotNull(source, "source");

        //    return (int)source.Provider.Execute(
        //        Expression.Call(
        //            typeof(Queryable),
        //            "Count",
        //            new Type[] { source.ElementType },
        //            source.Expression));
        //}

        //public static IQueryable Distinct(this IQueryable source)
        //{
        //    Ensure.ArgumentNotNull(source, "source");

        //    return source.Provider.CreateQuery(
        //        Expression.Call(
        //            typeof(Queryable),
        //            "Distinct",
        //            new Type[] { source.ElementType },
        //            source.Expression));
        //}

        ///// <summary>
        ///// Dynamically runs an aggregate function on the IQueryable.
        ///// </summary>
        ///// <param name="source">The IQueryable data source.</param>
        ///// <param name="function">The name of the function to run. Can be Sum, Average, Min, Max.</param>
        ///// <param name="member">The name of the property to aggregate over.</param>
        ///// <returns>The value of the aggregate function run over the specified property.</returns>
        //public static object Aggregate(this IQueryable source, string function, string member)
        //{
        //    Ensure.ArgumentNotNull(source, "source");
        //    Ensure.ArgumentNotNull(function, "function");
        //    Ensure.ArgumentNotNull(member, "member");

        //    // Properties
        //    PropertyInfo property = source.ElementType.GetProperty(member);
        //    ParameterExpression parameter = Expression.Parameter(source.ElementType, "s");
        //    Expression selector = Expression.Lambda(Expression.MakeMemberAccess(parameter, property), parameter);
        //    // We've tried to find an expression of the type Expression<Func<TSource, TAcc>>,
        //    // which is expressed as ( (TSource s) => s.Price );

        //    var methods = typeof(Queryable).GetMethods().Where(x => x.Name == function);

        //    // Method
        //    MethodInfo aggregateMethod = typeof(Queryable).GetMethods().SingleOrDefault(
        //        m => m.Name == function
        //             && m.ReturnType
        //             == property
        //                 .PropertyType // should match the type of the property
        //             && m.IsGenericMethod);

        //    // Sum, Average
        //    if (aggregateMethod != null)
        //    {
        //        return source.Provider.Execute(
        //            Expression.Call(
        //                null,
        //                aggregateMethod.MakeGenericMethod(new[] { source.ElementType }),
        //                new[] { source.Expression, Expression.Quote(selector) }));
        //    }
        //    // Min, Max
        //    else
        //    {
        //        aggregateMethod = typeof(Queryable).GetMethods().SingleOrDefault(
        //            m => m.Name == function && m.GetGenericArguments().Length == 2
        //                                    && m.IsGenericMethod);

        //        return source.Provider.Execute(
        //            Expression.Call(
        //                null,
        //                aggregateMethod.MakeGenericMethod(
        //                    new[] { source.ElementType, property.PropertyType }),
        //                new[] { source.Expression, Expression.Quote(selector) }));
        //    }
        //}

        //public static IAsyncEnumerable AsAsyncEnumerable(this IQueryable source)
        //{
        //    Ensure.ArgumentNotNull(source, "source");

        //    var enumerable = source as IAsyncEnumerable<TSource>;

        //    if (enumerable != null)
        //    {
        //        return enumerable;
        //    }

        //    var entityQueryableAccessor = source as IAsyncEnumerableAccessor<TSource>;

        //    if (entityQueryableAccessor != null)
        //    {
        //        return entityQueryableAccessor.AsyncEnumerable;
        //    }

        //    throw new InvalidOperationException(Strings.IQueryableNotAsync(typeof(TSource)));
        //}


        public static async Task<object[]> ToArrayAsync(this IQueryable source, CancellationToken cancellationToken = default(CancellationToken))
        {
            // basically the same as in AsAsyncEnumerable only with reflection
            Ensure.ArgumentNotNull(source, "source");

            object asyncEnumerable = source;
            // var enumerable = source as IAsyncEnumerable<TSource>;
            var isAsyncEnumerable = source.GetType()
                                          .GetInterfaces()
                                          .FirstOrDefault(x => x.Name.StartsWith("IAsyncEnumerable"))
                                    != null;

            if (!isAsyncEnumerable)
            {
                // var entityQueryableAccessor = source as IAsyncEnumerableAccessor<TSource>;
                var enumPropertyInfo =  source.GetType().GetProperty("AsyncEnumerable");

                asyncEnumerable = enumPropertyInfo.GetValue(source);
            }

            var toArrayMethodBase = typeof(AsyncEnumerable).GetMethods(BindingFlags.Public | BindingFlags.Static)
                                                       .Single(
                                                           x => x.GetParameters().Length == 2 && x.Name == "ToArray");
            var toArrayMethod = toArrayMethodBase.MakeGenericMethod(source.ElementType);

            var arrayTask = (Task)toArrayMethod.Invoke(null, new object[] { asyncEnumerable, cancellationToken });

            await arrayTask;

            var array = arrayTask.GetType()
                                 .GetProperty("Result")
                                 .GetValue(arrayTask);

            //var configureAwaitMethod = arrayTask.GetType().GetMethod("ConfigureAwait", new[] { typeof(bool) });

            //var arrayTask2 = configureAwaitMethod.Invoke(arrayTask, new object[] { false });

            return ((IEnumerable)array).Cast<object>().ToArray();
        }
    }
}