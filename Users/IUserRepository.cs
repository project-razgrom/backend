using Project_Razgrom_v_9._184.Shared;
using System.ComponentModel.DataAnnotations;
namespace Project_Razgrom_v_9._184
{
    public class CreateUserDto
    {
        [Required,MinLength(1)]
        public string Name { get; set; } = string.Empty;
        [Required,EmailAddress]
        public string Email { get; set; } = string.Empty;
        [Required,MinLength(6)]
        public string Password { get; set; } = string.Empty;
        [Required,MinLength (6)]
        public string Username { get; set; } = string.Empty;

    }
        public interface IUsersRepository : IBaseRepository<Users, CreateUserDto>
    {
    }
}
