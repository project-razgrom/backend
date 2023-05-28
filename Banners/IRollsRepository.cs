using Project_Razgrom_v_9._184.Shared;

namespace Project_Razgrom_v_9._184
{
    public class CreateRollDto
    {
        public int Counter { get; set; }
        public Users User { get; set; }
        public Banners Banners { get; set; }
        public DateTime Time { get; set; }
        public Items Item { get; set; }

    }
    public interface IRollsRepository : IBaseRepository<Rolls, CreateRollDto>
    {
        public Task<Rolls> GetLastRollOfUser(Users user, Banners banners);
        public Task<List<Rolls>> GetBannerHistory(Banners banners);
        public Task<List<Rolls>> GetLastRolls(int count);
        public Task<List<Rolls>> GetUserHistory(Users user, int limit = 15);

    }
}
