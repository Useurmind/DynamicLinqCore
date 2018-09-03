using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DynamicLinqCore
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> Where<T>(this IQueryable<T> source, string predicate, params object[] values)
        {
            return (IQueryable<T>)Where((IQueryable)source, predicate, values);
        }

        public static IQueryable Where(this IQueryable source, string predicate, params object[] values)
        {
            Ensure.ArgumentNotNull(source, "source");
            Ensure.ArgumentNotNull(predicate, "predicate");

            LambdaExpression lambda = DynamicExpression.ParseLambda(
                source.ElementType,
                typeof(bool),
                predicate,
                values);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "Where",
                    new Type[] { source.ElementType },
                    source.Expression,
                    Expression.Quote(lambda)));
        }

        public static IQueryable Select(this IQueryable source, string selector, params object[] values)
        {
            Ensure.ArgumentNotNull(source, "source");
            Ensure.ArgumentNotNull(selector, "selector");

            LambdaExpression lambda = DynamicExpression.ParseLambda(source.ElementType, null, selector, values);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "Select",
                    new Type[] { source.ElementType, lambda.Body.Type },
                    source.Expression,
                    Expression.Quote(lambda)));
        }

        public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string ordering, params object[] values)
        {
            return (IQueryable<T>)OrderBy((IQueryable)source, ordering, values);
        }

        public static IQueryable OrderBy(this IQueryable source, string ordering, params object[] values)
        {
            Ensure.ArgumentNotNull(source, "source");
            Ensure.ArgumentNotNull(ordering, "ordering");

            ParameterExpression[] parameters = new ParameterExpression[]
                                                   { Expression.Parameter(source.ElementType, "") };
            ExpressionParser parser = new ExpressionParser(parameters, ordering, values);
            IEnumerable<DynamicOrdering> orderings = parser.ParseOrdering();
            Expression queryExpr = source.Expression;
            string methodAsc = "OrderBy";
            string methodDesc = "OrderByDescending";
            foreach (DynamicOrdering o in orderings)
            {
                queryExpr = Expression.Call(
                    typeof(Queryable),
                    o.Ascending ? methodAsc : methodDesc,
                    new Type[] { source.ElementType, o.Selector.Type },
                    queryExpr,
                    Expression.Quote(Expression.Lambda(o.Selector, parameters)));
                methodAsc = "ThenBy";
                methodDesc = "ThenByDescending";
            }

            return source.Provider.CreateQuery(queryExpr);
        }

        public static IQueryable Take(this IQueryable source, int count)
        {
            Ensure.ArgumentNotNull(source, "source");

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

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "Skip",
                    new Type[] { source.ElementType },
                    source.Expression,
                    Expression.Constant(count)));
        }

        public static IQueryable GroupBy(
            this IQueryable source,
            string keySelector,
            string elementSelector,
            params object[] values)
        {
            Ensure.ArgumentNotNull(source, "source");
            Ensure.ArgumentNotNull(keySelector, "keySelector");
            Ensure.ArgumentNotNull(elementSelector, "elementSelector");

            LambdaExpression keyLambda = DynamicExpression.ParseLambda(source.ElementType, null, keySelector, values);
            LambdaExpression elementLambda = DynamicExpression.ParseLambda(
                source.ElementType,
                null,
                elementSelector,
                values);
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "GroupBy",
                    new Type[] { source.ElementType, keyLambda.Body.Type, elementLambda.Body.Type },
                    source.Expression,
                    Expression.Quote(keyLambda),
                    Expression.Quote(elementLambda)));
        }

        public static bool Any(this IQueryable source)
        {
            Ensure.ArgumentNotNull(source, "source");

            return (bool)source.Provider.Execute(
                Expression.Call(
                    typeof(Queryable),
                    "Any",
                    new Type[] { source.ElementType },
                    source.Expression));
        }

        public static int Count(this IQueryable source)
        {
            Ensure.ArgumentNotNull(source, "source");

            return (int)source.Provider.Execute(
                Expression.Call(
                    typeof(Queryable),
                    "Count",
                    new Type[] { source.ElementType },
                    source.Expression));
        }

        public static IQueryable Distinct(this IQueryable source)
        {
            Ensure.ArgumentNotNull(source, "source");

            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(Queryable),
                    "Distinct",
                    new Type[] { source.ElementType },
                    source.Expression));
        }

        /// <summary>
        /// Dynamically runs an aggregate function on the IQueryable.
        /// </summary>
        /// <param name="source">The IQueryable data source.</param>
        /// <param name="function">The name of the function to run. Can be Sum, Average, Min, Max.</param>
        /// <param name="member">The name of the property to aggregate over.</param>
        /// <returns>The value of the aggregate function run over the specified property.</returns>
        public static object Aggregate(this IQueryable source, string function, string member)
        {
            Ensure.ArgumentNotNull(source, "source");
            Ensure.ArgumentNotNull(function, "function");
            Ensure.ArgumentNotNull(member, "member");

            // Properties
            PropertyInfo property = source.ElementType.GetProperty(member);
            ParameterExpression parameter = Expression.Parameter(source.ElementType, "s");
            Expression selector = Expression.Lambda(Expression.MakeMemberAccess(parameter, property), parameter);
            // We've tried to find an expression of the type Expression<Func<TSource, TAcc>>,
            // which is expressed as ( (TSource s) => s.Price );

            var methods = typeof(Queryable).GetMethods().Where(x => x.Name == function);

            // Method
            MethodInfo aggregateMethod = typeof(Queryable).GetMethods().SingleOrDefault(
                m => m.Name == function
                     && m.ReturnType
                     == property
                         .PropertyType // should match the type of the property
                     && m.IsGenericMethod);

            // Sum, Average
            if (aggregateMethod != null)
            {
                return source.Provider.Execute(
                    Expression.Call(
                        null,
                        aggregateMethod.MakeGenericMethod(new[] { source.ElementType }),
                        new[] { source.Expression, Expression.Quote(selector) }));
            }
            // Min, Max
            else
            {
                aggregateMethod = typeof(Queryable).GetMethods().SingleOrDefault(
                    m => m.Name == function && m.GetGenericArguments().Length == 2
                                            && m.IsGenericMethod);

                return source.Provider.Execute(
                    Expression.Call(
                        null,
                        aggregateMethod.MakeGenericMethod(
                            new[] { source.ElementType, property.PropertyType }),
                        new[] { source.Expression, Expression.Quote(selector) }));
            }
        }
    }
}