using Project_Razgrom_v_9._184.Shared;

namespace Project_Razgrom_v_9._184
{
    public class CreateLinkerDto
    {
        public Guid Banner { get; set; }
        public Guid Item { get; set; }
    }
    public interface ILinkersRepository : IBaseRepository<Linker, CreateLinkerDto>
    {
        public Task<List<Linker>> GetByBanner(Banners banners);
    }
}
