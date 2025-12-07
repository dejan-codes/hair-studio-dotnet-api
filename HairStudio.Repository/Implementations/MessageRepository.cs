using HairStudio.Model.Models;
using HairStudio.Repository.Interfaces;

namespace HairStudio.Repository.Implementations
{
    public class MessageRepository : BaseRepository<Message>, IMessageRepository
    {
        public MessageRepository(HairStudioContext context) : base(context)
        {
        }

        public void SaveMessage(short userId, string content)
        {
            Add(new Message
            {
                CreatedAt = DateTime.Now,
                Content = content,
                UserId = userId
            });
        }

        public IQueryable<Message> GetMessagesQuery()
        {
            return GetAll().OrderByDescending(o => o.MessageId);
        }
    }
}
