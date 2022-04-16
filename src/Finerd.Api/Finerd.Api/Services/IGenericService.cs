using Finerd.Api.Model.Entities;

namespace Finerd.Api.Services
{
    public interface IGenericService<T> where T : EntityKey
    {
        Task<T> Add(T entity);
        Task Delete(int id);
        IEnumerable<T> Query();
        Task<T> FindAsync(int id);
        Task Update(int id, T entity);
    }
}