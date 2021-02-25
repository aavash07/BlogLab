using BlogLab.Models;
using BlogLab.Models.Photo;
using BlogLab.Repository;
using BlogLab.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Threading.Tasks;

namespace BlogLab.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PhotoController : ControllerBase
    {
        private readonly IPhotoRepository _photoRepository;
        private readonly IBlogRepository _blogRepository;
        private readonly IPhotoServices _photoService;

        public PhotoController(
            IPhotoRepository photoRepository, IBlogRepository blogRepository, IPhotoServices photoService)
        {
            _photoRepository = photoRepository;
            _blogRepository = blogRepository;
            _photoService = photoService;
        }

        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Photo>> UploadPhoto(IFormFile file)
        {
            int applciationUserID = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);

            var uploadResult = await _photoService.AddPhotoAsync(file);

            if (uploadResult.Error != null) return BadRequest(uploadResult.Error.Message);

            var photoCreate = new PhotoCreate
            {
                PublicID = uploadResult.PublicId,
                ImageUrl = uploadResult.SecureUrl.AbsoluteUri,
                Description = file.FileName
            };
            var photo = await _photoRepository.InsertAsync(photoCreate, applciationUserID);
            return Ok(photo);

        }

        [Authorize]
        [HttpGet]
        public async Task<ActionResult<List<Photo>>> GetByApplicationUserID()
        {
            int aplicationUserID = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);
            var photos = await _photoRepository.GetAllByUserIDAsync(aplicationUserID);
            return Ok(photos);
        }

        [Authorize]
        [HttpDelete("{phtoID}")]
        public async Task<ActionResult<int>> Delete(int photoID)
        {
            int applicationUserID = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);
            var foundPhoto = await _photoRepository.GetAsync(photoID);
            if (foundPhoto != null)
            {
                if (foundPhoto.ApplicationUserID == applicationUserID)
                {
                    var blogs = await _blogRepository.GetAllUserIDAsync(applicationUserID);
                    var userInBlog = blogs.Any(b => b.PhotoID == photoID);
                    if (userInBlog) return BadRequest("Cannot remove photo as it is being used in published blogs(s)");

                    var deleteResult = await _photoService.DeletionPhotoAsync(foundPhoto.PublicID);
                    if (deleteResult.Error != null) return BadRequest(deleteResult.Error);

                    var affectedRows = await _photoRepository.DeleteAsync(foundPhoto.PhotoID);
                    return Ok(affectedRows);
                }
                else
                {
                    return BadRequest("Photo was not uploaded by the current user");
                }
                
            }
            return BadRequest("Photo not found");
        }
    }
}
