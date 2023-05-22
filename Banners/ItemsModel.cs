using Project_Razgrom_v_9._184.Shared;

namespace Project_Razgrom_v_9._184
{
    public class Items : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public Rarity Type { get; set; }
        public string Image { get; set; } = string.Empty;
        public bool IsInStandard { get; set; } 

    }
}
public enum Rarity
{
    Common = 1, Uncommon, Rare, Epic, Legendary
}
