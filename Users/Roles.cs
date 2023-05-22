using Project_Razgrom_v_9._184.Shared;

namespace Project_Razgrom_v_9._184
{
    public class Roles : BaseModel
    {
        public string Name { get; set; } = string.Empty;
        public RolesType Type { get; set; }
    }
}   
public enum RolesType
{
    Admin, User, Moder
}
