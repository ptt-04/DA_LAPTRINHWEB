using Microsoft.EntityFrameworkCore;
using NguyenTienPhat_2280620311.Models;

namespace NguyenTienPhat_2280620311.Repositories
{
    public class EFCategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public EFCategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        // Fix for CS0738: Ensure the return type matches the interface definition
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories.ToListAsync();
        }

        // Fix for CS0738: Ensure the return type matches the interface definition
        public async Task<Category> GetByIdAsync(int id)
        {
            return await _context.Categories.FirstOrDefaultAsync(c => c.Id == id);
        }

        // Fix for CS0535: Implement AddAsync(Category) method
        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        // Fix for CS0535: Implement UpdateAsync(Category) method
        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        // Fix for CS0535: Implement DeleteAsync(int) method
        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
