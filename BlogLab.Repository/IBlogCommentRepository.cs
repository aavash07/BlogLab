using BlogLab.Models.BlogComment;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlogLab.Repository
{
    public interface IBlogCommentRepository
    {
        public Task<BlogComment> UpsertAsync(BlogCommentCreate blogCommentCreate, int applicationUserID);
        public Task<List<BlogComment>> GetAllAsync(int blogID);
        public Task<BlogComment> GetAsync(int blogCommentID);
        public Task<int> DeleteAsync(int blogCommentID);
    }
}
