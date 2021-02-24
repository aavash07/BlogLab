using BlogLab.Models.Photo;
using Dapper;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BlogLab.Repository
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly IConfiguration _config;

        public PhotoRepository(IConfiguration config)
        {
            _config = config;
        }
        public async Task<int> DeleteAsync(int photoID)
        {
            int affectedRows = 0;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                affectedRows = await connection.ExecuteAsync("Photo_Delete",
                    new { PhotoID = photoID },
                    commandType: CommandType.StoredProcedure);
               
            }
            return affectedRows;
        }

        public async Task<List<Photo>> GetAllByUserIDAsync(int applicationUserID)
        {
            IEnumerable<Photo> photos;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                photos = await connection.QueryAsync<Photo>("Photo_getByUserID",
                    new { ApplicationUserID = applicationUserID },
                    commandType: CommandType.StoredProcedure);

            }
            return photos.ToList();
        }

        public async Task<Photo> GetAsync(int photoID)
        {
            Photo photo;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                photo = await connection.QueryFirstOrDefaultAsync<Photo>(
                    "Photo_Get",
                    new { PhotoID = photoID },
                    commandType: CommandType.StoredProcedure);
            }
            return photo;
        }

        public async Task<Photo> InsertAsync(PhotoCreate photoCreate, int applicationUserID)
        {
            var dataTable = new DataTable();
            dataTable.Columns.Add("PublicID", typeof(string));
            dataTable.Columns.Add("ImageUrl", typeof(string));
            dataTable.Columns.Add("Description", typeof(string));

            dataTable.Rows.Add(photoCreate.PublicID, photoCreate.ImageUrl, photoCreate.Description);

            int newPhotoID;

            using (var connection = new SqlConnection(_config.GetConnectionString("DefaultConnection")))
            {
                await connection.OpenAsync();
                newPhotoID = await connection.ExecuteScalarAsync<int>(
                    "Photo_Insert",
                    new {PhotoID =dataTable.AsTableValuedParameter("dbo.PhotoType"),
                        applicationUserID=applicationUserID},
                    commandType: CommandType.StoredProcedure
                    );
            }
            Photo photo = await GetAsync(newPhotoID);
            return photo;
        }
    }
}
