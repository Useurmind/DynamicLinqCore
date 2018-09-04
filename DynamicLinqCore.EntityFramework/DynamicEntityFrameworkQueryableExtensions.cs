using System;
using System.Linq;
using System.Linq.Expressions;

using Microsoft.EntityFrameworkCore;

namespace DynamicLinqCore.EntityFramework
{
    public static class DynamicEntityFrameworkQueryableExtensions
    {
        public static IQueryable Include(this IQueryable source, string includePath)
        {
            Ensure.ArgumentNotNull(source, "source");

            // root to original entity framework method
            return source.Provider.CreateQuery(
                Expression.Call(
                    typeof(EntityFrameworkQueryableExtensions),
                    "Include",
                    new Type[] { source.ElementType },
                    source.Expression,
                    Expression.Constant(includePath)));
        }
    }
}
