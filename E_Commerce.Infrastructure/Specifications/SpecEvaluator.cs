using E_Commerce.Domain.Common;
using E_Commerce.Domain.Contracts;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Infrastructure.Specifications
{
    internal static class SpecEvaluator
    {
        public static IQueryable<TEntity> CreateQuery<TEntity,Tkey>(IQueryable<TEntity> QueryStart, ISpecifications<TEntity,Tkey> Spec) where TEntity : BaseEntity<Tkey>
        {
            var query = QueryStart;
            if(Spec.Criteria != null)
            {
                query = query.Where(Spec.Criteria);
            }
            if (Spec.IncludesExp.Any())
            {
                query = Spec.IncludesExp.Aggregate(query, (current, nextExp) => current.Include(nextExp));
                                            /*
                                                foreach(var exp in specifications.IncludesExp)
                                                {
                                                  Query =  Query.Include(exp);
                                                }
                                            */
            }
            if(Spec.OrderBy != null)
            {
                query = query.OrderBy(Spec.OrderBy);
            }
            else if(Spec.OrderByDesc != null)
            {
                query = query.OrderByDescending(Spec.OrderByDesc);
            }
            if (Spec.IsPaginated)
            {
                query = query.Skip(Spec.Skip).Take(Spec.Take);
            }
            return query;
        }
    }
}
