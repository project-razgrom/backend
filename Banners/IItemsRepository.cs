using Project_Razgrom_v_9._184.Shared;

namespace Project_Razgrom_v_9._184
{
    public class CreateItemDto
    {
        public string Name { get; set; } = string.Empty;
        public Rarity Type { get; set; }
        public string Image { get; set; } = string.Empty;
    }
    public interface IItemsRepository : IBaseRepository<Items, CreateItemDto>
    {
        public Task<List<Items>> GetAllFromStandard();
    };

}
