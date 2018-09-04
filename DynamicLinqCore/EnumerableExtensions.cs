using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DynamicLinqCore
{
    public static class EnumerableExtensions
    {
        public static IEnumerable<T> Where<T>(this IEnumerable<T> source, string predicate)
        {
            Ensure.ArgumentNotNull(source, "source");

            return source.AsQueryable().Where(predicate);
        }

        public static IEnumerable Where(this IEnumerable source, string predicate)
        {
            Ensure.ArgumentNotNull(source, "source");

            return source.AsQueryable().Where(predicate);
        }

        public static IEnumerable Select(this IEnumerable source, string selector, params object[] values)
        {
            Ensure.ArgumentNotNull(source, "source");
            Ensure.ArgumentNotNull(selector, "selector");

            return source.AsQueryable().Select(selector, values);
        }

        public static IEnumerable<T> OrderBy<T>(this IEnumerable<T> source, string ordering, Type orderedPropertyType)
        {
            Ensure.ArgumentNotNull(source, "source");

            return source.AsQueryable().OrderBy(ordering, orderedPropertyType);
        }

        public static IEnumerable OrderByDescending(this IEnumerable source, string ordering, Type orderedPropertyType)
        {
            Ensure.ArgumentNotNull(source, "source");

            return source.AsQueryable().OrderByDescending(ordering, orderedPropertyType);
        }

        public static IEnumerable<T> OrderByDescending<T>(this IEnumerable<T> source, string ordering, Type orderedPropertyType)
        {
            Ensure.ArgumentNotNull(source, "source");

            return source.AsQueryable().OrderByDescending(ordering, orderedPropertyType);
        }

        public static IEnumerable OrderBy(this IEnumerable source, string ordering, Type orderedPropertyType)
        {
            Ensure.ArgumentNotNull(source, "source");

            return source.AsQueryable().OrderBy(ordering, orderedPropertyType);
        }

        public static IEnumerable Take(this IEnumerable source, int count)
        {
            Ensure.ArgumentNotNull(source, "source");

            return source.AsQueryable().Take(count);
        }

        public static IEnumerable Skip(this IEnumerable source, int count)
        {
            Ensure.ArgumentNotNull(source, "source");

            return source.AsQueryable().Skip(count);
        }

        public static IEnumerable GroupBy(
            this IEnumerable source,
            string keySelector,
            string elementSelector,
            params object[] values)
        {
            Ensure.ArgumentNotNull(source, "source");
            Ensure.ArgumentNotNull(keySelector, "keySelector");
            Ensure.ArgumentNotNull(elementSelector, "elementSelector");

            return source.AsQueryable().GroupBy(keySelector, elementSelector, values);
        }

        public static bool Any(this IEnumerable source)
        {
            Ensure.ArgumentNotNull(source, "source");

            return source.AsQueryable().Any();
        }

        public static int Count(this IEnumerable source)
        {
            Ensure.ArgumentNotNull(source, "source");

            return source.AsQueryable().Count();
        }

        public static IEnumerable Distinct(this IEnumerable source)
        {
            Ensure.ArgumentNotNull(source, "source");

            return source.AsQueryable().Distinct();
        }
    }
}