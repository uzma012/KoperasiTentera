using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace KT.Interfaces.Repositories
{
    public interface IRepository<T> where T : class
    {
        /// <summary>
        ///
        /// Query like normal ef query
        ///
        /// <code>Query()</code>
        ///</summary>
        IQueryable<T> Query();

        /// <summary>
        ///
        /// Query like normal ef query
        ///

        ///</summary>
        ICollection<T> PaggedList(int? pageSize, int? page, params Expression<Func<T, object>>[] navigationProperties);

        /// <summary>
        /// Initializes a new instance of the <see cref="Repository{T}"/> class.
        /// </summary>
        Task<ICollection<T>> PaggedListAsync(int? pageSize, int? page, params Expression<Func<T, object>>[] navigationProperties);

        IQueryable<T> GetAll();

        //Task<IQueryable<T>> GetAllAsync();

        T GetById(int? id);
      

        /// <summary>
        /// return object table data by it's id
        /// </summary>
        Task<T> GetByIdAsync(int? id);

        T Find(Expression<Func<T, bool>> match);

        Task<T> FindAsync(Expression<Func<T, bool>> match);

        List<T> FindAll(Expression<Func<T, bool>> match);

        Task<List<T>> FindAllAsync(Expression<Func<T, bool>> match);

        T Insert(T entity);

        Task<T> InsertAsync(T entity);

        void Update(T updated);

        Task<List<T>> InsertRangeAsync(List<T> entities);

        void Delete(T t);

        void DeleteRange(List<T> entities);

        int Count();

        Task<int> CountAsync();

        //Task BulkInsertAsync(List<T> entities);

        //void BulkInsert(List<T> entities);

        IQueryable<T> Filter(
            Expression<Func<T, bool>> filter = null,
            Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null,
            string includeProperties = "",
            int? page = null,
            int? pageSize = null);

        IQueryable<T> FindBy(Expression<Func<T, bool>> predicate);

        bool Exist(Expression<Func<T, bool>> predicate);

        IList<T> GetAllInclude(params Expression<Func<T, object>>[] navigationProperties);

        Task<IList<T>> GetAllIncludeAsync(params Expression<Func<T, object>>[] navigationProperties);

        IList<T> GetIncludeList(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties);

        T GetSingleInclude(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties);

        Task<T> GetSingleIncludeAsync(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] navigationProperties);

        //public IEnumerable<T> ExecuteCommandQuery(string command);
        public void UpdateRange(List<T> entities);
        public void AttachRange(List<T> entities);
        public void AddOrUpdateByPredicate(Expression<Func<T, bool>> predicate, T updated);

    }
}