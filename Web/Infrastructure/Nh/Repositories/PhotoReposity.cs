using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using MediaCommMvc.Web.Infrastructure.DataInterfaces;
using MediaCommMvc.Web.Models.Photos;
using MediaCommMvc.Web.Models.Users;

using NHibernate;
using NHibernate.Linq;

namespace MediaCommMvc.Web.Infrastructure.Nh.Repositories
{
    public class PhotoRepository : IPhotoRepository
    {
        private readonly IConfigAccessor configAccessor;

        private readonly IImageGenerator imageGenerator;

        private readonly ILogger logger;

        private readonly ISessionContainer sessionContainer;

        public PhotoRepository(ISessionContainer sessionContainer, IImageGenerator imageGenerator, IConfigAccessor configAccessor, ILogger logger)
        {
            this.sessionContainer = sessionContainer;
            this.imageGenerator = imageGenerator;
            this.configAccessor = configAccessor;
            this.logger = logger;
        }

        protected ISession Session
        {
            get
            {
                return this.sessionContainer.CurrentSession;
            }
        }

        public void AddCategory(PhotoCategory category)
        {
            this.Session.SaveOrUpdate(category);
        }

        public void AddPhoto(int albumId, MediaCommUser uploader, string filePath)
        {
            PhotoAlbum album = this.Session.Get<PhotoAlbum>(albumId);

            FileInfo photoFile = new FileInfo(filePath);
            this.AddPictureToDB(photoFile, album, uploader);

            this.imageGenerator.GenerateImages(photoFile);
        }

        public void AddPhoto(string albumName, int categoryId, MediaCommUser uploader, string filePath)
        {
            PhotoAlbum album = this.Session.Query<PhotoAlbum>().Single(a => a.Name == albumName && a.PhotoCategory.Id == categoryId);

            FileInfo photoFile = new FileInfo(filePath);
            this.AddPictureToDB(photoFile, album, uploader);

            this.imageGenerator.GenerateImages(photoFile);
        }

        public IEnumerable<PhotoAlbum> Get4NewestAlbums()
        {
            return this.Session.Query<PhotoAlbum>().OrderByDescending(a => a.LastPicturesAdded).Take(4).ToList();
        }

        public PhotoAlbum GetAlbumById(int albumId)
        {
            return this.Session.Query<PhotoAlbum>().FetchMany(a => a.Photos).Single(a => a.Id == albumId);
        }

        public IEnumerable<PhotoAlbum> GetAlbumsForCategoryIdStartingWith(int catId, string term)
        {
            return this.Session.Query<PhotoAlbum>().Where(a => a.PhotoCategory.Id == catId && a.Name.StartsWith(term)).ToList();
        }

        public IEnumerable<string> GetAllAlbumNames()
        {
            return this.Session.Query<PhotoAlbum>().Select(a => a.Name).ToList();
        }

        public IEnumerable<PhotoCategory> GetAllCategories()
        {
            return this.Session.Query<PhotoCategory>().ToList();
        }

        public PhotoCategory GetCategoryById(int id)
        {
            return this.Session.Query<PhotoCategory>().FetchMany(c => c.Albums).Single(c => c.Id == id);
        }

        public PhotoCategory GetCategoryByName(string categoryName)
        {
            return this.Session.Query<PhotoCategory>().FetchMany(c => c.Albums).Single(c => c.Name == categoryName);
        }

        public Image GetImage(int photoId, string size)
        {
            Photo photo = this.GetPhotoById(photoId);

            string fileName = photo.FileName.Insert(photo.FileName.LastIndexOf("."), size);

            string directoryPath = Path.Combine(
                this.GetValidDirectoryName(photo.PhotoAlbum.PhotoCategory.Name), 
                Path.Combine(this.GetValidDirectoryName(photo.PhotoAlbum.Name), fileName));
            string imagePath = Path.Combine(this.configAccessor.GetConfigValue("PhotoRootDir"), directoryPath);

            Image image;
            try
            {
                image = Image.FromFile(imagePath);
            }
            catch (IOException exception)
            {
                this.logger.Error(string.Format("Unable to load Image from file '{0}'", imagePath), exception);
                return new Bitmap(32, 32);
            }

            // Increase viewcount if the image was not loaded as thumbnail
            if (!size.Equals("small", StringComparison.OrdinalIgnoreCase))
            {
                photo.ViewCount++;
                this.Session.Update(photo);
            }

            return image;
        }

        public Photo GetPhotoById(int id)
        {
            return this.Session.Get<Photo>(id);
        }

        public string GetStoragePathForAlbum(int albumId)
        {
            PhotoAlbum album = this.Session.Get<PhotoAlbum>(albumId);
            string storagePathForAlbum = this.GetTargetPath(album);

            return storagePathForAlbum;
        }

        public string GetStoragePathForAlbumName(string albumName, int categoryId)
        {
            PhotoAlbum album = this.Session.Query<PhotoAlbum>().SingleOrDefault(a => a.Name == albumName);

            if (album == null)
            {
                PhotoCategory category = this.GetCategoryById(categoryId);
                album = new PhotoAlbum { Name = albumName, LastPicturesAdded = DateTime.UtcNow, PhotoCategory = category };
                this.Session.Save(album);
            }

            return this.GetTargetPath(album);
        }

        private void AddPictureToDB(FileInfo photoFile, PhotoAlbum album, MediaCommUser uploader)
        {
            PhotoAlbum photoAlbum = this.Session.Query<PhotoAlbum>().SingleOrDefault(a => a.Name.Equals(album.Name)) ?? album;

            photoAlbum.LastPicturesAdded = DateTime.UtcNow;

            Bitmap bmp = new Bitmap(photoFile.FullName);
            int height = bmp.Height;
            int width = bmp.Width;
            bmp.Dispose();

            Photo photo = new Photo
            {
                PhotoAlbum = photoAlbum, 
                FileName = photoFile.Name, 
                FileSize = photoFile.Length, 
                Height = height, 
                Uploader = uploader, 
                Width = width
            };

            this.Session.Save(photo);
        }

        private string GetTargetPath(PhotoAlbum album)
        {
            string targetPath = Path.Combine(
                this.configAccessor.GetConfigValue("PhotoRootDir"), 
                Path.Combine(this.GetValidDirectoryName(album.PhotoCategory.Name), this.GetValidDirectoryName(album.Name)));

            if (!Directory.Exists(targetPath))
            {
                Directory.CreateDirectory(targetPath);
            }

            return targetPath;
        }

        private string GetValidDirectoryName(string directoryName)
        {
            char[] invalidChars = Path.GetInvalidFileNameChars().Concat(Path.GetInvalidPathChars()).Distinct().ToArray();
            string invalidCharsRegexString = string.Format(@"[{0}]", Regex.Escape(new string(invalidChars) + " $.§ß%^&;=,'^´`#"));
            string validName = Regex.Replace(directoryName, invalidCharsRegexString, "_");

            return validName;
        }
    }
}