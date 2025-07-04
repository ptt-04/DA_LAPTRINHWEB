﻿using Microsoft.EntityFrameworkCore;
using NguyenTienPhat_2280620311.Models;

namespace NguyenTienPhat_2280620311.Repositories
{
    public class EFProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;
        public EFProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            // return await _context.Products.ToListAsync();
            return await _context.Products.Include(p => p.Category) // Include thông tin về category
            .ToListAsync();
        }
        public async Task<Product> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Reviews)
                    .ThenInclude(r => r.User)
                .FirstOrDefaultAsync(p => p.Id == id);
        }
        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }
        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products.FindAsync(id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        public object GetById(int productId)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<Product>> SearchByNameAsync(string keyword, int limit = 5)
        {
            return await _context.Products
                .Where(p => p.Name.Contains(keyword))
                .OrderBy(p => p.Name)
                .Take(limit)
                .ToListAsync();
        }
    }
}
