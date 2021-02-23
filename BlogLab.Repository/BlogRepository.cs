using BlogLab.Models.Blog;
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
    public class BlogRepository : IBlogRepository
    {
        private readonly IConfiguration _config;

        public BlogRepository(IConfiguration config)
        {
            _config = config;
        }
        public async Task<int> DeleteAsync(int blogID)
        {
            int affectedRows = 0;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                affectedRows = await connection.ExecuteAsync("Blog_Delete",
                    new { BlogID = blogID },
                    commandType: CommandType.StoredProcedure);

            }
            return affectedRows;
        }

        public async Task<PagedResults<Blog>> GetAllAsync(BlogPaging blogPaging)
        {
            var results = new PagedResults<Blog>();

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                using(var multi=await connection.QueryMultipleAsync("Blog_All",
                    new
                    {
                    Offset = ( blogPaging.Page-1)*blogPaging.PageSize,
                    PageSize = blogPaging.PageSize
                    },
            commandType:CommandType.StoredProcedure))
                {
                results.Items = multi.Read<Blog>();
                results.TotalCount = multi.ReadFirst<int>();
                }
            }
            return results;
        }

        public async Task<List<Blog>> GetAllFamousAsync()
        {
            IEnumerable<Blog> famousBlogs;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                famousBlogs = await connection.QueryAsync<Blog>("Blog_GetAllFamous",
                    new {  },
                    commandType: CommandType.StoredProcedure);

            }
            return famousBlogs.ToList();
        }

        public async Task<List<Blog>> GetAllUserIDAsync(int applicationUserID)
        {
            IEnumerable<Blog> blogs;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                blogs = await connection.QueryAsync<Blog>("Blog_getByUserID",
                    new { ApplicationUserID = applicationUserID },
                    commandType: CommandType.StoredProcedure);

            }
            return blogs.ToList();
        }

        public async Task<Blog> GetAsync(int blogID)
        {
            Blog blog;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                blog = await connection.QueryFirstOrDefaultAsync<Blog>(
                    "Blog_Get",
                    new { BlogID = blogID },
                    commandType: CommandType.StoredProcedure);
            }
            return blog;
        }

        public async Task<Blog> UpsertAsync(BlogCreate blogCreate, int applicationUserID)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("BlogID", typeof(string));
            dataTable.Columns.Add("Title", typeof(string));
            dataTable.Columns.Add("Content", typeof(string));
            dataTable.Columns.Add("PhotoID", typeof(string));

            dataTable.Rows.Add(blogCreate.BlogID, blogCreate.Title, blogCreate.Content,blogCreate.PhotoID);

            int? newBlogID;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                newBlogID = await connection.ExecuteScalarAsync<int>(
                    "Blog_Upsert",
                    new
                    {
                        Blog = dataTable.AsTableValuedParameter("dbo.BlogType"),ApplicationUserID=applicationUserID
                    },
                    commandType: CommandType.StoredProcedure
                    );
            }
            newBlogID = newBlogID ?? blogCreate.BlogID;
            Blog blog = await GetAsync(newBlogID.Value);
            return blog;
        }
    }
}
