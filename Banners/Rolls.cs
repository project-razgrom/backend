using Project_Razgrom_v_9._184.Shared;

namespace Project_Razgrom_v_9._184
{
    public class Rolls : BaseModel
    {
        public int Counter { get; set; }
        public Users User { get; set; }
        public Banners Banners { get; set; }
        public DateTime Time { get; set; }
        public Items Item { get; set; }



    }
}
