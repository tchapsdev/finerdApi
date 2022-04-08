using Finerd.Api.Data;
using Finerd.Api.Model.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Finerd.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GenericController<T> : BaseApiController where T : EntityKey
    {
        private readonly ApplicationDbContext _context;
        public GenericController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/EntityTypes
        [HttpGet]
        public async Task<ActionResult<IEnumerable<T>>> Get()
        {
            return await _context.Set<T>().ToListAsync();
        }

        // GET: api/EntityTypes/5
        [HttpGet("{id}")]
        public async Task<ActionResult<T>> Get(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null)
                return NotFound();
            return entity;
        }

        // PUT: api/EntityTypes/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> Put(int id, T entity)
        {
            if (id != entity.Id)
            {
                return BadRequest();
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
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return NoContent();
        }

        // POST: api/EntityTypes
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<T>> Post(T entity)
        {
            _context.Set<T>().Add(entity);
            await _context.SaveChangesAsync();
            return Ok(entity);
        }

        // DELETE: api/EntityTypes/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Set<T>().FindAsync(id);
            if (entity == null)
                return NotFound();
            _context.Set<T>().Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }

        private bool Exists(int id)
        {
            return _context.Set<T>().Any(e => e.Id == id);
        }
    }
}