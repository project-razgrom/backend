using Microsoft.EntityFrameworkCore;
using Project_Razgrom_v_9._184;
using Project_Razgrom_v_9._184.Shared;
using System.Reflection;

namespace Project_Razgrom_v_9._184
{
    public class RollRepository : BaseRepository<Rolls, CreateRollDto>, IRollsRepository
    {
        public RollRepository(AppDbContext context) : base(context) { }
        public override async Task<Rolls> Create(CreateRollDto entity)
        {
            var newRoll = new Rolls()
            {
                Counter = entity.Counter,
                User = entity.User,
                Banners = entity.Banners,
                Time = entity.Time,
                Item = entity.Item,
                Id = new Guid()
            };
            await context.AddAsync(newRoll);
            await context.SaveChangesAsync();
            return newRoll ?? throw new NullReferenceException();
        }
        public override async Task<Rolls> Update(Rolls entity)
        {
            // Получаем ссылку на объект, который хотим изменить
            var candidate = await context.Set<Rolls>().FirstOrDefaultAsync(roll => roll.Id == entity.Id);
            if (candidate == null)
            {
                throw new ArgumentException();
            }

            // Меняем значения в объекте
            candidate.Counter = entity.Counter;
            candidate.User = entity.User;
            candidate.Banners = entity.Banners;
            candidate.Time = entity.Time;
            candidate.Item = entity.Item;

            // Сохраняем изменения
            await context.SaveChangesAsync();

            // Опционально: возвращаем измененный объект
            return candidate;
        }
        public async Task<Rolls> GetLastRollOfUser(Users user, Banners banners)
        {
            return await context.Set<Rolls>().LastOrDefaultAsync(roll =>
                roll.User.Id == user.Id && roll.Banners.Id == banners.Id);
        }

        public async Task<List<Rolls>> GetBannerHistory(Banners banners)
        {
            return await context.Set<Rolls>().OrderByDescending(roll => roll.Time)
                .Where(roll => roll.Banners.Id == banners.Id).Take(10).ToListAsync();
        }

        public async Task<List<Rolls>> GetLastRolls(int count = 10)
        {
            return await context.Set<Rolls>().OrderByDescending(roll => roll.Time).Take(count).ToListAsync();
        }

        public async Task<List<Rolls>> GetUserHistory(Users user, int limit = 15)
        {
            return await context.Set<Rolls>().OrderByDescending(roll => roll.Time)
                .Where(roll => roll.User.Id == user.Id).Take(limit).ToListAsync();
        }
    }
}