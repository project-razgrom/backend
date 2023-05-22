using Microsoft.EntityFrameworkCore;

namespace Project_Razgrom_v_9._184.Shared
{
    public abstract class BaseRepository<T, TCreate> : IBaseRepository<T, TCreate> where T : BaseModel
    {
        public AppDbContext context;
        protected BaseRepository(AppDbContext context)
        {
            this.context = context;
        }


        public abstract Task<T> Create(TCreate entity);

        public async virtual Task<bool> Delete(Guid id)
        {
            var candidate = await context.Set<T>().FirstOrDefaultAsync();
            if (candidate == null)
            {
                return false;
            }
            context.Remove(candidate);
            await context.SaveChangesAsync();
            return true;

        }

        public async virtual Task<List<T>> GetAll()
        {
            return await context.Set<T>().ToListAsync();
        }

        public virtual async Task<T> GetById(Guid id)
        {
            return await context.Set<T>().FirstOrDefaultAsync(t => t.Id == id);
        }

        public abstract Task<T> Update(T entity);
    }
}
