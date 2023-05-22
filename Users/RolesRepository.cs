using Microsoft.EntityFrameworkCore;
using Project_Razgrom_v_9._184.Shared;

namespace Project_Razgrom_v_9._184
{
    public class RolesRepository : BaseRepository<Roles, CreateRoleDto>, IRolesRepository
    {
        public RolesRepository(AppDbContext context) : base(context) { }

        public override async Task<Roles> Create(CreateRoleDto entity)
        {
            var newRole = new Roles()
            {
                Name = entity.Name,
                Id = new Guid(),
                Type = entity.Type,
            };
            await context.AddAsync(newRole);
            await context.SaveChangesAsync();
            return newRole ?? throw new NullReferenceException();
        }

        public override async Task<Roles> Update(Roles entity)
        {
            // Получаем ссылку на объект, который хотим изменить
            var candidate = await context.Set<Roles>().FirstOrDefaultAsync(role => role.Id == entity.Id);
            if (candidate == null)
            {
                throw new ArgumentException();
            }

            // Меняем значения в объекте
            candidate.Name = entity.Name;
            candidate.Type = entity.Type;

            // Сохраняем изменения
            await context.SaveChangesAsync();

            // Опционально: возвращаем измененный объект
            return candidate;
        }
    }
}
