using Project_Razgrom_v_9._184.Shared;
using System.ComponentModel.DataAnnotations;

namespace Project_Razgrom_v_9._184
{
    public class Comments : BaseModel
    {
        public Users User { get; set; }
        public string Comment { get; set; } = string.Empty;
    }
}
