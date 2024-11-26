using System.Threading.Tasks;

namespace KT.Interfaces.Repositories
{
    public interface IUnitOfWorkCosmos
    {
        void Commit();

        void ClearTrack();
        Task<int> CommitAsync();

        void Rollback();
    }
}