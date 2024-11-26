

using KT.Interfaces.Repositories;
using KT.Models.DBContext;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace KT.Repositories
{
    public class GenericRepository<T> : IRepository<T> where T : class
    {
        private readonly ApplicationContext _context;
        private readonly DbSet<T> _dbSet;
        public string ErrorMessage { get; set; } = string.Empty;

        public GenericRepository(ApplicationContext context)
        {
            _context = context;
            _dbSet = context.Set<T>();
        }

        public int Count()
        {
            return _context.Set<T>().Count();
        }

        public async Task<int> CountAsync()
        {
            return await _context.Set<T>().CountAsync();
        }

        public void Delete(T t)
        {
            if (_context.Set<T>() == null)
            {
                ErrorMessage = "The item you trying to Delete is not in Database anymore";
                throw new ArgumentNullException(nameof(t));
            }
            _context.Set<T>().Remove(t);
        }

        public void DeleteRange(List<T> entities)
        {
            if (_context.Set<T>() == null)
            {
                ErrorMessage = "The item you trying to Delete is not in Database anymore";
                throw new ArgumentNullException(nameof(entities));
            }
            _context.Set<T>().RemoveRange(entities);
        }

        public bool Exist(Expression<Func<T, bool>> predicate)
        {
            var exist = _context.Set<T>().Where(predicate);
            return exist.Any();
        }

        //public async Task BulkInsertAsync(List<T> entities)
        //{
        //    await _context.BulkInsertAsync(entities);
        //}

        //public void BulkInsert(List<T> entities)
        //{
        //    _context.BulkInsert(entities);
        //}

        public IQueryable<T> Filter(Expression<Func<T, bool>> filter = null, Func<IQueryable<T>, IOrderedQueryable<T>> orderBy = null, string includeProperties = "", int? page = null, int? pageSize = null)
        {
            IQueryable<T> query = _context.Set<T>();
            if (filter != null)
            {
                query = query.Where(filter);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }

            if (includeProperties != null)
            {
                foreach (
                    var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
                {
                    query = query.Include(includeProperty);
                }
            }

            if (page != null && pageSize != null)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return query;
        }

        public T Find(Expression<Func<T, bool>> match)
        {
            return _context.Set<T>().SingleOrDefault(match);
        }

        public List<T> FindAll(Expression<Func<T, bool>> match)
        {
            return _context.Set<T>().Where(match).ToList();
        }

        public async Task<List<T>> FindAllAsync(Expression<Func<T, bool>> match)
        {
            return await _context.Set<T>().Where(match).ToListAsync();
        }

        public async Task<T> FindAsync(Expression<Func<T, bool>> match)
        {
            return await _context.Set<T>().SingleOrDefaultAsync(match);
        }

        //public IEnumerable<T> ExecuteCommandQuery(string command)
        //            => _dbSet.FromSqlRaw(command);

        public IQueryable<T> FindBy(Expression<Func<T, bool>> predicate)
        {
            return _context.Set<T>().Where(predicate);
        }

        public IQueryable<T> GetAll()
        {
            return _context.Set<T>();
        }

        public IList<T> GetAllInclude(params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> dbQuery = _context.Set<T>();

            //Apply eager loading
            foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                dbQuery = dbQuery.Include<T, object>(navigationProperty);

            var list = dbQuery
                .AsNoTracking()
                .ToList();

            return list;
        }

        public async Task<IList<T>> GetAllIncludeAsync(params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> dbQuery = _context.Set<T>();

            //Apply eager loading
            foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                dbQuery = dbQuery.Include<T, object>(navigationProperty);

            return await dbQuery.AsNoTracking().ToListAsync();
        }

        /// <summary>
        /// returns list of cattle by firm
        /// </summary>
        public T GetById(int? id)
        {
            return _context.Set<T>().Find(id);
        }

        public async Task<T> GetByIdAsync(int? id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public IList<T> GetIncludeList(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties)
        {
            List<T> list;
            IQueryable<T> dbQuery = _context.Set<T>();

            //Apply eager loading
            foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                dbQuery = dbQuery.Include<T, object>(navigationProperty);

            list = dbQuery
                .AsNoTracking().AsEnumerable()
                .Where(where)
                .ToList<T>();

            return list;
        }

        public T GetSingleInclude(Func<T, bool> where, params Expression<Func<T, object>>[] navigationProperties)
        {
            T item = null;
            IQueryable<T> dbQuery = _context.Set<T>();

            //Apply eager loading
            foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                dbQuery = dbQuery.Include<T, object>(navigationProperty);

            item = dbQuery
                .AsNoTracking() //Don't track any changes for the selected item
                .FirstOrDefault(where); //Apply where clause
            return item;
        }

        public async Task<T> GetSingleIncludeAsync(Expression<Func<T, bool>> where, params Expression<Func<T, object>>[] navigationProperties)
        {
            T item = null;
            IQueryable<T> dbQuery = _context.Set<T>();

            //Apply eager loading
            foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                dbQuery = dbQuery.Include<T, object>(navigationProperty);

            item = await dbQuery
                .AsNoTracking() //Don't track any changes for the selected item
                .SingleOrDefaultAsync(where); //Apply where clause
            return item;
        }

        public T Insert(T entity)
        {
            _context.Set<T>().Add(entity);
            return entity;
        }

        public async Task<T> InsertAsync(T entity)
        {
            await _context.Set<T>().AddAsync(entity);
            return entity;
        }

        public async Task<List<T>> InsertRangeAsync(List<T> entities)
        {
            await _context.Set<T>().AddRangeAsync(entities);
            return entities;
        }

        public ICollection<T> PaggedList(int? pageSize, int? page, params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> query = _context.Set<T>();

            //Apply eager loadingf
            foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                query = query.Include<T, object>(navigationProperty);

            if (page != null && pageSize != null)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return query.ToList();
        }

        public async Task<ICollection<T>> PaggedListAsync(int? pageSize, int? page, params Expression<Func<T, object>>[] navigationProperties)
        {
            IQueryable<T> query = _context.Set<T>();

            //Apply eager loadingf
            foreach (Expression<Func<T, object>> navigationProperty in navigationProperties)
                query = query.Include<T, object>(navigationProperty);

            if (page != null && pageSize != null)
            {
                query = query.Skip((page.Value - 1) * pageSize.Value).Take(pageSize.Value);
            }

            return await query.ToListAsync();
        }

        /// <summary>
        ///
        /// Query like normal ef query
        ///
        /// <code>Query()</code>
        ///</summary>
        public IQueryable<T> Query()
        {
            return _context.Set<T>().AsQueryable();
        }

        public void Update(T updated)
        {
            if (_context.Set<T>() == null)
            {
                ErrorMessage = "No Database detected";
                throw new ArgumentNullException(nameof(updated));
            }

            _context.Set<T>().Attach(updated);
            _context.Entry(updated).State = EntityState.Modified;
        }

        public void AttachRange(List<T> entities)
        {
            if (_context.Set<T>() == null)
            {
                ErrorMessage = "The item you trying to updateis not in Database anymore";
                throw new ArgumentNullException(nameof(entities));
            }

            _context.Set<T>().AttachRange(entities);
        }

        public void UpdateRange(List<T> entities)
        {
            if (_context.Set<T>() == null)
            {
                ErrorMessage = "The item you trying to updateis not in Database anymore";
                throw new ArgumentNullException(nameof(entities));
            }

            _context.Set<T>().UpdateRange(entities);
            //var bulkConfig = new BulkConfig { WithHoldlock = false };
            //DbContextBulkExtensions.BulkUpdate(_context, entities, bulkConfig);
        }

        public void AddOrUpdateByPredicate(Expression<Func<T, bool>> predicate, T updated)
        {
            bool exists = _context.Set<T>().Any(predicate);
            if (exists)
            {
                _context.Set<T>().Update(updated);
            }
            else
            {
                _context.Set<T>().Add(updated);
            }
        }
    }
}