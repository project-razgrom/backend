using Project_Razgrom_v_9._184.Shared;

namespace Project_Razgrom_v_9._184
{
    public class CreateCommentDto
    {
        public Users User { get; set; } = new Users();
        public string Comment { get; set; } = string.Empty;
    }
    public interface ICommentsRepository : IBaseRepository<Comments, CreateCommentDto>
    {
    }
}
