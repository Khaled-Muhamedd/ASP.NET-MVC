using GymManagment.BLL.Common;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Attachement
{
    public interface IAttachmentService
    {
        Task<Result<string?>> UploadAsync(Stream fileStream , string fileName , string folderName , CancellationToken ct =default);
        bool Delete(string fileName, string folderName);
        (Stream Stream, string ContentType)? GetFile(string fileName, string folderName);
    }
}
