using HairStudio.Repository.Extensions;
using HairStudio.Repository.Interfaces;
using HairStudio.Services.Common;
using HairStudio.Services.Enums;
using HairStudio.Services.Errors;
using HairStudio.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HairStudio.Services.Implementations
{
    public class MessageService : IMessageService
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;

        public MessageService(IMessageRepository messageRepository, IUserRepository userRepository)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
        }

        public async Task<Result<object>> GetMessagesAsync(short tokenUserId, int page, int rowsPerPage)
        {
            if (page < 1 || rowsPerPage < 1)
                return Result<object>.Failure(ValidationErrors.NumberOfPages);

            var user = await _userRepository.GetUserWithRolesAsync(tokenUserId);
            if (user == null || !user.IsActive)
                return Result<object>.Failure(UserErrors.UserNotFound);

            var roles = user.Roles.Select(r => r.RoleId).ToList();

            var messagesQuery = _messageRepository.GetMessagesQuery();
            long totalCount;

            if (!roles.Contains((int)RoleEnum.Employee) && !roles.Contains((int)RoleEnum.Administrator))
            {
                totalCount = await messagesQuery
                    .Where(m => m.UserId == user.UserId)
                    .CountAsync();

                messagesQuery = messagesQuery.Where(m => m.UserId == user.UserId);
            }
            else
            {
                totalCount = await messagesQuery
                    .CountAsync();
            }

            var messagesForTable = await messagesQuery.Paged(page, rowsPerPage).Select(o => new
                {
                    Message = o.Content,
                    Date = o.CreatedAt
                })
                .ToListAsync();

            return Result<object>.Success(new
            {
                TotalCount = totalCount,
                Messages = messagesForTable
            });
        }
    }
}
