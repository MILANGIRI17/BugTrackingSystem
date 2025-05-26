using BugTracker.Shared.Constants;
using BugTracker.Shared.Dtos;
using Microsoft.AspNetCore.Http;

namespace BugTracker.Shared.Helper
{
    public static class FileUploader
    {
        public static async Task<List<AttachmentCreateOrUpdateDto>> UploadFiles(IFormFileCollection? fileCollection, string webRootPath)
        {
            List<AttachmentCreateOrUpdateDto> returnList = new List<AttachmentCreateOrUpdateDto>();
            try
            {
                if (fileCollection == null) return returnList;

                foreach (IFormFile file in fileCollection)
                {
                    if (file.Length > 0)
                    {
                        string uploadDirectory = Path.Combine(webRootPath, FileInformation.Location);
                        if (!Directory.Exists(uploadDirectory))
                            Directory.CreateDirectory(uploadDirectory);

                        var fileName = Path.GetFileName(file.FileName);
                        var filePath = Path.Combine(FileInformation.Location, fileName);
                        var uploadPath = Path.Combine(uploadDirectory, file.FileName);
                        using (var stream = new FileStream(uploadPath, FileMode.Create))
                        {
                            await file.CopyToAsync(stream);
                        }
                        returnList.Add(new AttachmentCreateOrUpdateDto { Name = fileName, Path = filePath });
                    }
                }
                return returnList;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
