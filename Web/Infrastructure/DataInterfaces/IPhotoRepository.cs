using System.Collections.Generic;
using System.Drawing;

using MediaCommMvc.Web.Models.Photos;
using MediaCommMvc.Web.Models.Users;

namespace MediaCommMvc.Web.Infrastructure.DataInterfaces
{
    public interface IPhotoRepository
    {
        void AddCategory(PhotoCategory category);

        void AddPhoto(int albumId, MediaCommUser uploader, string filePath);

        void AddPhoto(string albumName, int categoryId, MediaCommUser uploader, string targetPath);

        IEnumerable<PhotoAlbum> Get4NewestAlbums();

        PhotoAlbum GetAlbumById(int albumId);

        IEnumerable<PhotoAlbum> GetAlbumsForCategoryIdStartingWith(int catId, string term);

        IEnumerable<string> GetAllAlbumNames();

        IEnumerable<PhotoCategory> GetAllCategories();

        PhotoCategory GetCategoryById(int id);

        PhotoCategory GetCategoryByName(string categoryName);

        Image GetImage(int photoId, string size);

        Photo GetPhotoById(int id);

        string GetStoragePathForAlbum(int album);

        string GetStoragePathForAlbumName(string albumName, int categoryId);
    }
}