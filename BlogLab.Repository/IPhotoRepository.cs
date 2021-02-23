using BlogLab.Models.Photo;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace BlogLab.Repository
{
    public interface IPhotoRepository
    {
        public Task<Photo> InsertAsync(PhotoCreate photoCreate, int applicationUserID);

        public Task<Photo> GetAsync(int photoID);

        public Task<List<Photo>> GetAllByUserIDAsync(int applicationUserID);

        public Task<int> DeleteAsync(int photoID);
    }
}
