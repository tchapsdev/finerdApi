using Finerd.Api.Data;
using Finerd.Api.Model.Entities;
using Microsoft.EntityFrameworkCore;

namespace Finerd.Api.Services
{
    public class GenericService<T> : IGenericService<T> where T : EntityKey
    {
        private readonly ApplicationDbContext _context;
        public GenericService(ApplicationDbContext context)
        {
            _context = context;
        }

        public IEnumerable<T> Query()
        {
            return  _context.Set<T>().AsEnumerable();
        }

        public async Task<T> FindAsync(int id)
        {
            return await _context.Set<T>().FindAsync(id);
        }

        public async Task Update(int id, T entity)
        {
            if (id != entity.Id)
            {
                throw new ArgumentException();
            }
            _context.Entry(entity).State = EntityState.Modified;
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Exists(id))
                {
                    throw new Exception($"{typeof(T)} with key {id} does not exist");
                }
                else
                {
                    throw;
                }
            }
        }

        public async Task<T> Add(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return entity;
        }


        public async Task Delete(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null)
                throw new Exception($"{typeof(T)} with key {id} does not exist");
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
        }

        private bool Exists(int id)
        {
            return _context.Set<T>().Any(e => e.Id == id);
        }
    }
}
