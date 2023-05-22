using Project_Razgrom_v_9._184.Shared;

namespace Project_Razgrom_v_9._184
{
    public class CreateLinkerDto
    {
        public Banners Banner { get; set; }
        public Items Item { get; set; }
    }
    public interface ILinkersRepository : IBaseRepository<Linker, CreateLinkerDto>
    {
    }
}
