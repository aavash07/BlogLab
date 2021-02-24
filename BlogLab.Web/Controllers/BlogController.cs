using BlogLab.Models.Blog;
using BlogLab.Repository;
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
    public class BlogController : ControllerBase
    {
        private readonly IBlogRepository _blogRepository;
        private readonly IPhotoRepository _photoRepository;

        public BlogController(
            IBlogRepository blogRepository, IPhotoRepository photoRepository
            )
        {
            _blogRepository = blogRepository;
            _photoRepository = photoRepository;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Blog>> Create(BlogCreate blogCreate)
        {
            int applicationUserID = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);

            if (blogCreate.PhotoID.HasValue)
            {
                var photo = await _photoRepository.GetAsync(blogCreate.PhotoID.Value);

                if (photo.ApplicationUserID != applicationUserID)
                {
                    return BadRequest("You did not upload the photo");
                }
            }
            var blog = await _blogRepository.UpsertAsync(blogCreate, applicationUserID);
            return Ok(blog);
        }

        [HttpGet]
        public async Task<ActionResult<PagedResults<Blog>>> GetAll([FromQuery] BlogPaging blogPaging)
        {
            var blogs = await _blogRepository.GetAllAsync(blogPaging);
            return Ok(blogs);
        }

        [HttpGet("{blogID}")]
        public async Task<ActionResult<Blog>> Get(int blogID)
        {
            var blog = await _blogRepository.GetAsync(blogID);
            return Ok(blog);
        }

        [HttpGet("user/{applicationUserID}")]
        public async Task<ActionResult<List<Blog>>> GetByApplicationUserID(int applicationUserID)
        {
            var blogs = await _blogRepository.GetAllUserIDAsync(applicationUserID);
            return Ok(blogs);
        }

        [HttpGet("{famous}")]
        public async Task<ActionResult<Blog>> GetAllFamous(int blogID)
        {
            var blog = await _blogRepository.GetAllFamousAsync();
            return Ok(blog);
        }

        [HttpGet("{blogID}")]
        public async Task<ActionResult<int>> Delete(int blogID)
        {
            int applicationUserID = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);

            var foundBlog = await _blogRepository.GetAsync(blogID);
            if (foundBlog == null) return BadRequest("Blog does not exist");

            if (foundBlog.ApplicationUserID == applicationUserID)
            {
                var affectedRows = await _blogRepository.DeleteAsync(blogID);
                return Ok(affectedRows);
            }
            else
            {
                return BadRequest("You did not create this blog")
            }
        }

    }
}
