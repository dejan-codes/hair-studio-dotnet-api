using HairStudio.Model.Models;

namespace HairStudio.Repository.Interfaces
{
    public interface IMessageRepository : IRepositoryBase<Message>
    {
        void SaveMessage(short userId, string content);
        IQueryable<Message> GetMessagesQuery();
    }
}
