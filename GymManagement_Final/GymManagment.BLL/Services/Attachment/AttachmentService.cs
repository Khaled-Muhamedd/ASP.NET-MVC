using GymManagment.BLL.Common;
using GymManagment.BLL.Services.Attachement;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GymManagment.BLL.Services.Attachment
{
    public class AttachmentService : IAttachmentService
    {
        private readonly long maxFileSize = 5 * 1024 * 1024;
        private readonly ILogger<AttachmentService> logger;
        private readonly IWebHostEnvironment env;
        private readonly string[] _allowedExtentions = { ".png", ".jpg", ".jpeg" };
        public AttachmentService(ILogger<AttachmentService> logger , IWebHostEnvironment env)
        {
            this.logger = logger;
            this.env = env;
        }
        async Task<Result<string?>> IAttachmentService.UploadAsync(Stream fileStream, string fileName, string folderName, CancellationToken ct)
        {
            if (fileStream == null || !fileStream.CanRead) return Result<string?>.NotFound("File Is Not Found Or Can Not Be Read");
            if (fileStream.Length == 0) return Result<string?>.NotFound("Failed To Read the File Because the File Is Empty");

            if (fileStream.Length > maxFileSize)
            {
                logger.LogError($"File Rejected Because It Is To Large{fileStream.Length}");
                return Result<string?>.Validation("Can Not Load More than 5 mega");
            }

            var extension = Path.GetExtension(fileName);
            if (string.IsNullOrWhiteSpace(extension) || !_allowedExtentions.Contains(extension)) 
            {
                logger.LogError($"File Rejected : Extension {extension} Not Allowed");
                return Result<string?>.NotFound("No Extension Found Or Extension Is Not Found");
            }

            // akwen el path of folder 
            var uploadsFolder = Path.Combine(env.WebRootPath, folderName);
            Directory.CreateDirectory(uploadsFolder);


            var storedFileName = $"{Guid.NewGuid()} {extension}";
            var filePath = Path.Combine(uploadsFolder, storedFileName);



            try
            {
                await using var fs = new FileStream(filePath, FileMode.CreateNew, FileAccess.Write, FileShare.None);
                await fileStream.CopyToAsync(fs, ct);
                return Result<string?>.OK(storedFileName);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to upload file {FileName}.", fileName);
                return Result<string?>.Fail("Failed To Upload File");
            }
        }
        public (Stream Stream, string ContentType)? GetFile(string fileName, string folderName)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(folderName)) return null;

            var fullPath = Path.Combine(env.ContentRootPath, folderName, fileName);

            if (!File.Exists(fullPath)) return null;

            var contentType = Path.GetExtension(fullPath).ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => "image/jpeg",
                ".png" => "image/png",
                _ => "application/octet-stream"//Binary data
            };

            var stream = new FileStream(fullPath, FileMode.Open, FileAccess.Read, FileShare.Read);
            return (stream, contentType);

        }


        public bool Delete(string fileName, string folderName)
        {
            if (string.IsNullOrEmpty(fileName) || string.IsNullOrEmpty(folderName)) return false;

            try
            {
                var fullPath = Path.Combine(env.WebRootPath, folderName, fileName);

                if (!File.Exists(fullPath)) return false;
                File.Delete(fullPath);
                return true;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Failed to delete attachment {File}.", fileName);
                return false;
            }
        }
    }
}
