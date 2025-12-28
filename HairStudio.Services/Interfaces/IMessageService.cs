using HairStudio.Services.Common;

namespace HairStudio.Services.Interfaces
{
    public interface IMessageService
    {
        Task<Result<object>> GetMessagesAsync(int page, int rowsPerPage);
    }
}
