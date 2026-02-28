using System.Collections.Generic;
using System.Threading.Tasks;

namespace KASHOP.DAL.Repositry
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(string[]? include = null);
        Task<T> CreateAsync(T entity);
    }
}
