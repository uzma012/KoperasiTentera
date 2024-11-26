using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using KT.Models.DBContext;
using System.Threading.Tasks;

namespace KT.Interfaces.Repositories
{
    public interface IUnitOfWork
    {
        IRepository<T> Repository<T>() where T : class;

        Task<int> CommitAsync();

        void Commit();

        void StoreProcedureCall(string query, params object[] parameters);

        public IDbContextTransaction BeginTransaction();

        public IExecutionStrategy CreateExecutionStrategy();

        public void CommitTransaction();

        void Rollback();
    }
}