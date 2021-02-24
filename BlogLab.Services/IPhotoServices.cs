using System.Threading.Tasks;
using System;
using CloudinaryDotNet.Actions;
using Microsoft.AspNetCore.Http;

namespace BlogLab.Services
{
    public interface IPhotoServices
    {
        public Task<ImageUploadResult> AddPhotoAsync(IFormFile file);
        public Task<DeletionResult> DeletionPhotoAsync(string publicID);
    }
}
