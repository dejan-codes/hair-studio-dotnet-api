using HairStudio.API.Infrastructure;
using HairStudio.Services.Common;
using HairStudio.Services.Errors;
using HairStudio.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HairStudio.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class MessageController : ControllerBase
    {
        private readonly IMessageService _messageService;
        private readonly ICurrentUserContext _currentUserContext;

        public MessageController(IMessageService messageService, ICurrentUserContext currentUserContext)
        {
            _messageService = messageService;
            _currentUserContext = currentUserContext;
        }

        [Authorize(Roles = "Administrator")]
        [HttpGet]
        public async Task<IActionResult> GetMessagesAsync(int page, int rowsPerPage)
        {
            var tokenUserId = _currentUserContext.UserId;
            if (tokenUserId == null)
                return Result.Failure(UserErrors.UserNotFound).ToActionResult();

            var result = await _messageService.GetMessagesAsync(tokenUserId.Value, page, rowsPerPage);
            return result.ToActionResult();
        }
    }
}