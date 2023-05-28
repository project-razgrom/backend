using Microsoft.EntityFrameworkCore;
using Project_Razgrom_v_9._184.Shared;

namespace Project_Razgrom_v_9._184
{
    public class UserRepository : BaseRepository<Users, CreateUserDto>, IUsersRepository
    {
        public UserRepository(AppDbContext context) : base(context) { }

        public override async Task<Users> Create(CreateUserDto entity)
        {
            var newUser = new Users()
            {
                Email = entity.Email,
                Name = entity.Name,
                Password = entity.Password,
                Username = entity.Username,
                Id = new Guid()
            };
            await context.AddAsync(newUser);
            await context.SaveChangesAsync();
            return newUser ?? throw new NullReferenceException();
        }

        public async Task<Users> GetByLoginInfo(string password, string name)
        {
            return await context.Set<Users>().FirstOrDefaultAsync(user =>
            user.Email == name && user.Password == password);
        }

        public override async Task<Users> Update(Users entity)
        {
            // Получаем ссылку на объект, который хотим изменить
            var candidate = await context.Set<Users>().FirstOrDefaultAsync(user => user.Id == entity.Id);
            if (candidate == null)
            {
                throw new ArgumentException();
            }

            // Меняем значения в объекте
            candidate.Name = entity.Name;
            candidate.Password = entity.Password;
            candidate.Username = entity.Username;
            candidate.Email = entity.Email;

            // Сохраняем изменения
            await context.SaveChangesAsync();

            // Опционально: возвращаем измененный объект
            return candidate;
        }
    }
}
