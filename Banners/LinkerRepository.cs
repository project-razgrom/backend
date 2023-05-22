using Microsoft.EntityFrameworkCore;
using Project_Razgrom_v_9._184.Shared;

namespace Project_Razgrom_v_9._184
{
    public class LinkerRepository : BaseRepository<Linker, CreateLinkerDto>, ILinkersRepository
    {
        public LinkerRepository(AppDbContext context) : base(context) { }
        public override async Task<Linker> Create(CreateLinkerDto entity)
        {
            var newLinker = new Linker()
            {
                Banner = entity.Banner,
                Item = entity.Item,
                Id = new Guid()
            };
            await context.AddAsync(newLinker);
            await context.SaveChangesAsync();
            return newLinker ?? throw new NullReferenceException();
        }
        public override async Task<Linker> Update(Linker entity)
        {
            // Получаем ссылку на объект, который хотим изменить
            var candidate = await context.Set<Linker>().FirstOrDefaultAsync(linker => linker.Id == entity.Id);
            if (candidate == null)
            {
                throw new ArgumentException();
            }

            // Меняем значения в объекте
            candidate.Banner = entity.Banner;
            candidate.Item = entity.Item;

            // Сохраняем изменения
            await context.SaveChangesAsync();

            // Опционально: возвращаем измененный объект
            return candidate;
        }
    }
}
