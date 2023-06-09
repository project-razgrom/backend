﻿using Microsoft.EntityFrameworkCore;
using Project_Razgrom_v_9._184.Shared;

namespace Project_Razgrom_v_9._184
{
    public class ItemRepository : BaseRepository<Items, CreateItemDto>, IItemsRepository
    {
        public ItemRepository(AppDbContext context) : base(context) { }
        public override async Task<Items> Create(CreateItemDto entity)
        {
            var newItem = new Items()
            {
                Name = entity.Name,
                Type = entity.Type,
                Image = entity.Image,
                Id = new Guid(),
                IsInStandard = entity.IsStandard
            };
            await context.AddAsync(newItem);
            await context.SaveChangesAsync();
            return newItem ?? throw new NullReferenceException();
        }

        public async Task<List<Items>> GetAllFromStandard()
        {
            return await context.Set<Items>().Where(item => item.IsInStandard).ToListAsync();

        }

        public override async Task<Items> Update(Items entity)
        {
            // Получаем ссылку на объект, который хотим изменить
            var candidate = await context.Set<Items>().FirstOrDefaultAsync(item => item.Id == entity.Id);
            if (candidate == null)
            {
                throw new ArgumentException();
            }

            // Меняем значения в объекте
            candidate.Name = entity.Name;
            candidate.Type = entity.Type;
            candidate.Image = entity.Image;

            // Сохраняем изменения
            await context.SaveChangesAsync();

            // Опционально: возвращаем измененный объект
            return candidate;
        }
    }
}
