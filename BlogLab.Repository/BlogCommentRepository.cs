using BlogLab.Models.BlogComment;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BlogLab.Repository
{
    public class BlogCommentRepository : IBlogCommentRepository
    {
        private readonly IConfiguration _config;

        public BlogCommentRepository(IConfiguration config)
        {
            _config = config;
        }
        public async Task<int> DeleteAsync(int blogCommentID)
        {
            int affectedRows = 0;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                affectedRows = await connection.ExecuteAsync(
                    "BlogComment_Delete",
                    new { BlogCommentID = blogCommentID },
                    commandType: CommandType.StoredProcedure);

            }
            return affectedRows;
        }

        public async Task<List<BlogComment>> GetAllAsync(int blogID)
        {
            IEnumerable<BlogComment> blogComments;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                blogComments = await connection.QueryAsync<BlogComment>(
                    "BlogComment_GetAll",
                    new { BlogID = blogID },
                    commandType: CommandType.StoredProcedure);

            }
            return blogComments.ToList();
        }

        public async Task<BlogComment> GetAsync(int blogCommentID)
        {
            BlogComment blogComment;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                blogComment = await connection.QueryFirstOrDefaultAsync<BlogComment>(
                    "BlogComment_Get",
                    new { BlogCommentID = blogCommentID },
                    commandType: CommandType.StoredProcedure);
            }
            return blogComment;
        }

        public async Task<BlogComment> UpsertAsync(BlogCommentCreate blogCommentCreate, int applicationUserID)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("BlogCommentID", typeof(int));
            dataTable.Columns.Add("ParentBlogCommentID", typeof(int));
            dataTable.Columns.Add("BlogID", typeof(int));
            dataTable.Columns.Add("Content", typeof(string));

            dataTable.Rows.Add(blogCommentCreate.BlogCommentID, blogCommentCreate.ParentCommentID, blogCommentCreate.BlogID
                ,blogCommentCreate.Content);

            int newBlogCommentID;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                newBlogCommentID = await connection.ExecuteScalarAsync<int>(
                    "BlogComment_Upsert",
                    new { BlogCommetn = dataTable.AsTableValuedParameter("dbo.BlogCommentType") },
                    commandType: CommandType.StoredProcedure
                    );
            }
            newBlogCommentID=
            Photo photo = await GetAsync(newBlogCommentID);
            return photo;
        }
    }
}
