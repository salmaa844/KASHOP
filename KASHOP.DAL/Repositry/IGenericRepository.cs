using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace KASHOP.DAL.Repositry
{
    public interface IGenericRepository<T> where T : class
    {
        Task<List<T>> GetAllAsync(string[]? include = null);
        Task<T> CreateAsync(T entity);
        Task<T?> GetOne(Expression<Func<T, bool>> filter, string[]? include = null);

        Task<bool> DeleteAsync(T entity);
        Task<bool> UpdateAsync(T entity);
    }
}
