using E_Commerce.Domain.Common;
using E_Commerce.Domain.Contracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace E_Commerce.Application.Specifications
{
    internal abstract class BaseSpecification<TEntity, Tkey> : ISpecifications<TEntity, Tkey> where TEntity : BaseEntity<Tkey>
    {
        public ICollection<Expression<Func<TEntity, object>>> IncludesExp { get; } = [];
        public Expression<Func<TEntity, bool>> Criteria { get; private set; }

        public Expression<Func<TEntity, object>>? OrderBy { get; private set; }

        public Expression<Func<TEntity, object>>? OrderByDesc { get; private set; }

        public int Take { get; private set; }
        public int Skip { get; private set; }
        public bool IsPaginated { get; private set; }

        protected void ApplyPagination(int pageSize, int PageIndex)
        {
            // 40 Products
            // page size = 10 
            // page Index = 2
            IsPaginated = true;
            Take = pageSize;
            Skip = (PageIndex -1) * pageSize;
        }
        protected void AddOrderBy(Expression<Func<TEntity, object>> OrderByExp)
        {
            OrderBy = OrderByExp;
        }
        protected void AddOrderByDesc(Expression<Func<TEntity, object>> OrderByDescExp)
        {
            OrderByDesc = OrderByDescExp;
        }
        protected BaseSpecification(Expression<Func<TEntity, bool>> criteria)
        {
            Criteria = criteria;
        }
        protected void AddIncludeExp(Expression<Func<TEntity, object>> includeExp){
            IncludesExp.Add(includeExp);
        }

        

    }
}
