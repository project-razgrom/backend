using Microsoft.EntityFrameworkCore;
using Project_Razgrom_v_9._184.Shared;

namespace Project_Razgrom_v_9._184
{
    public class CommentRepository : BaseRepository<Comments, CreateCommentDto>, ICommentsRepository
    {
        public CommentRepository(AppDbContext context) : base(context) { }
        public override async Task<Comments> Create(CreateCommentDto entity)
        {
            var newComment = new Comments()
            {
                User = entity.User,
                Comment = entity.Comment,
                Id = new Guid()
            };
            await context.AddAsync(newComment);
            await context.SaveChangesAsync();
            return newComment ?? throw new NullReferenceException();
        }
        public override async Task<Comments> Update(Comments entity)
        {
            // Получаем ссылку на объект, который хотим изменить
            var candidate = await context.Set<Comments>().FirstOrDefaultAsync(comment => comment.Id == entity.Id);
            if (candidate == null)
            {
                throw new ArgumentException();
            }

            // Меняем значения в объекте
            candidate.User = entity.User;
            candidate.Comment = entity.Comment;
            candidate.Id = entity.Id;

            // Сохраняем изменения
            await context.SaveChangesAsync();

            // Опционально: возвращаем измененный объект
            return candidate;
        }
    }
}
