using Microsoft.AspNetCore.Http;

namespace HairStudio.Services.Common
{
    public static class FileHelper
    {
        public static byte[] FileToByteArray(IFormFile file)
        {
            using var ms = new MemoryStream();
            file.CopyTo(ms);
            return ms.ToArray();
        }
    }
}
