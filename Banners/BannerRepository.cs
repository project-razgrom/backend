using Microsoft.EntityFrameworkCore;
using Project_Razgrom_v_9._184.Shared;

namespace Project_Razgrom_v_9._184
{
    public class BannerRepository : BaseRepository<Banners, CreateBannerDto>, IBannersRepository
    {
        public BannerRepository(AppDbContext context) : base(context) { }
        public override async Task<Banners> Create(CreateBannerDto entity)
        {
            entity.TimeStart.ToUniversalTime();

            var newBanner = new Banners()
            {
                Name = entity.Name,
                Type = entity.Type,
                TimeStart = entity.TimeStart.ToUniversalTime(),
                TimeEnd = entity.TimeEnd.ToUniversalTime(),
                ImagePath= entity.ImagePath,
                Id = new Guid()
            };
            await context.AddAsync(newBanner);
            await context.SaveChangesAsync();
            return newBanner ?? throw new NullReferenceException();
        }

        public async Task<List<Banners>> GetByTimeEnd(DateTime date)
        {
            return await context.Set<Banners>().Where(baner => baner.TimeEnd > date).ToListAsync();
        }

        public override async Task<Banners> Update(Banners entity)
        {
            // Получаем ссылку на объект, который хотим изменить
            var candidate = await context.Set<Banners>().FirstOrDefaultAsync(baner => baner.Id == entity.Id);
            if (candidate == null)
            {
                throw new ArgumentException();
            }

            // Меняем значения в объекте
            candidate.Name = entity.Name;
            candidate.Type = entity.Type;
            candidate.TimeStart = entity.TimeStart;
            candidate.TimeEnd = entity.TimeEnd;
            candidate.ImagePath = entity.ImagePath;

            // Сохраняем изменения
            await context.SaveChangesAsync();

            // Опционально: возвращаем измененный объект
            return candidate;
        }
    }
}
