using Microsoft.AspNetCore.Mvc;

namespace HairStudio.Services.Common
{
    public static class ResultExtensions
    {
        public static IActionResult ToActionResult(this Result result)
        {
            if (result.IsSuccess)
                return new OkResult();

            var statusCode = ErrorHttpMapper.GetStatusCode(result.Error);
            return new ObjectResult(result.Error.Description) { StatusCode = statusCode };
        }

        public static IActionResult ToActionResult<T>(this Result<T> result)
        {
            if (result.IsSuccess)
                return new OkObjectResult(result.Value);

            var statusCode = ErrorHttpMapper.GetStatusCode(result.Error);
            return new ObjectResult(result.Error.Description) { StatusCode = statusCode };
        }
    }
}
