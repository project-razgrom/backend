using Microsoft.EntityFrameworkCore;
using Project_Razgrom_v_9._184.Shared;

namespace Project_Razgrom_v_9._184
{
    public class LinkerRepository : BaseRepository<Linker, CreateLinkerDto>, ILinkersRepository
    {
        public LinkerRepository(AppDbContext context) : base(context) { }
        public override async Task<Linker> Create(CreateLinkerDto entity)
        {
            var banner = await context.Set<Banners>()
                .FirstOrDefaultAsync(ban => ban.Id == entity.Banner);
            var item = await context.Set<Items>()
                .FirstOrDefaultAsync(it => it.Id == entity.Item);

            if (banner == null || item == null) 
            { 
                throw new ArgumentException("Banner or item id is not found");
            }

            var newLinker = new Linker()
            {
                Banner = banner,
                Item = item,
                Id = new Guid()
            };
            await context.AddAsync(newLinker);
            await context.SaveChangesAsync();
            return newLinker ?? throw new NullReferenceException();
        }

        public async Task<List<Linker>> GetByBanner(Banners banners)
        {
            var list = await context.Set<Linker>()
                .Where(link => link.Banner.Id == banners.Id)
                .ToListAsync();

            return list;
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
