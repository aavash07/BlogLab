using BlogLab.Models.BlogComment;
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
    public class BlogCommentController : ControllerBase
    {
        private readonly IBlogCommentRepository _blogCommentRepository;

        public BlogCommentController(IBlogCommentRepository blogCommentRepository)
        {
            _blogCommentRepository = blogCommentRepository;
        }
        [Authorize]
        [HttpPost]
        public async Task<ActionResult<BlogComment>> Create(BlogCommentCreate blogCommentCreate)
        {
            int applicationUserID = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);

            var createdBlogComment = await _blogCommentRepository.UpsertAsync(blogCommentCreate, applicationUserID);

            return Ok(createdBlogComment);
        }

        [HttpGet("{BlogID}")]
        public async Task<ActionResult<List<BlogComment>>> GetAll(int blogID)
        {
            var blogComments = await _blogCommentRepository.GetAllAsync(blogID);
            return Ok(blogComments);
        }

        [Authorize]
        [HttpDelete("{blogCommentID}")]
        public async Task<ActionResult<int>> Delete(int blogCommentID)
        {
            int applicationUserID = int.Parse(User.Claims.First(i => i.Type == JwtRegisteredClaimNames.NameId).Value);

            var foundBlogComment = await _blogCommentRepository.GetAsync(blogCommentID);

            if (foundBlogComment == null) return BadRequest("Comment Does not exist");

            if (foundBlogComment.ApplicationUserID== applicationUserID)
            {
                var affectedRows = await _blogCommentRepository.DeleteAsync(blogCommentID);
                return Ok(affectedRows);
            }
            else
            {
                return BadRequest("This comment was not created by the current user");
            }
        }
    }
}
