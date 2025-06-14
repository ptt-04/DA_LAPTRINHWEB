using NguyenTienPhat_2280620311.Models;

namespace NguyenTienPhat_2280620311.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAllAsync();
        Task<Product> GetByIdAsync(int id);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task DeleteAsync(int id);
        object GetById(int productId);
        Task<IEnumerable<Product>> SearchByNameAsync(string keyword, int limit = 5);
    }
}
