namespace Project_Razgrom_v_9._184.Shared
{
    public interface IBaseRepository<T, TCreate>
    {
        public Task<List<T>> GetAll();
        public Task<T> GetById(Guid id);
        public Task<T> Create(TCreate entity);
        public Task<T> Update(T entity);
        public Task<bool> Delete(Guid id);
    }
}
