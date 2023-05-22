﻿using Microsoft.EntityFrameworkCore;
using Project_Razgrom_v_9._184.Shared;

namespace Project_Razgrom_v_9._184
{
    public class BannerRepository : BaseRepository<Banners, CreateBannerDto>, IBannersRepository
    {
        public BannerRepository(AppDbContext context) : base(context) { }
        public override async Task<Banners> Create(CreateBannerDto entity)
        {
            var newBanner = new Banners()
            {
                Name = entity.Name,
                Type = entity.Type,
                TimeStart = DateTime.Now,
                TimeEnd = DateTime.Now,
                ImagePath= entity.ImagePath,
                Id = new Guid()
            };
            await context.AddAsync(newBanner);
            await context.SaveChangesAsync();
            return newBanner ?? throw new NullReferenceException();
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