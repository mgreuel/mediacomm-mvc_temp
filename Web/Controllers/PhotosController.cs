using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MediaCommMvc.Web.Infrastructure;
using MediaCommMvc.Web.Infrastructure.DataInterfaces;
using MediaCommMvc.Web.Infrastructure.Nh;
using MediaCommMvc.Web.Models.Photos;
using MediaCommMvc.Web.ViewModels;

using Newtonsoft.Json;

namespace MediaCommMvc.Web.Controllers
{
    public partial class PhotosController : Controller
    {
        private readonly CurrentUserContainer currentUserContainer;

        private readonly IPhotoRepository photoRepository;

        public PhotosController(IPhotoRepository photoRepository, CurrentUserContainer currentUserContainer)
        {
            this.photoRepository = photoRepository;
            this.currentUserContainer = currentUserContainer;
        }

        [SessionActionFilter]
        public virtual ActionResult Album(int id, string title)
        {
            PhotoAlbum album = this.photoRepository.GetAlbumById(id);
            var albumViewModel = new AlbumViewModel
                                                {
                                                    AlbumName = album.Name,
                                                    Photos = album.Photos.Select(p => new PhotoViewModel { Id = p.Id }).ToList()
                                                };
            return this.View(albumViewModel);
        }

        [SessionActionFilter]
        public virtual ActionResult Category(int id)
        {
            PhotoCategory category = this.photoRepository.GetCategoryById(id);

            IEnumerable<PhotoAlbumCoverViewModel> viewModels =
                category.Albums.Select(a => new PhotoAlbumCoverViewModel
                                                {
                                                    CoverImageUrl = this.Url.Action(MVC.Photos.Photo().AddRouteValues(new { id = a.CoverPhotoId, size = "small" })),
                                                    Id = a.Id,
                                                    Title = a.Name
                                                }).OrderBy(a => a.Title).ToList();

            return this.View(viewModels);
        }

        [SessionActionFilter]
        public virtual ActionResult Index()
        {
            return this.View();
        }

        [SessionActionFilter]
        public virtual ActionResult Photo(int id, string size)
        {
            Image image = this.photoRepository.GetImage(id, size);
            return new ImageResult { Image = image };
        }

        [SessionActionFilter]
        public virtual ActionResult Upload()
        {
            IEnumerable<string> albumNames = this.photoRepository.GetAllAlbumNames();
            return this.View(new UploadViewModel { AllAlbumNamesJson = JsonConvert.SerializeObject(albumNames) });
        }

        [HttpPost]
        [SessionActionFilter]
        public virtual ActionResult Upload(UploadViewModel uploadViewModel)
        {
            var reponseObjects = new List<dynamic>();

            for (int i = 0; i < this.Request.Files.Count; i++)
            {
                bool skipFile = false;

                HttpPostedFileBase file = this.Request.Files[i];

                string directoryPath = this.photoRepository.GetStoragePathForAlbumName(uploadViewModel.AlbumName, uploadViewModel.CategoryId);
                string targetPath = Path.Combine(directoryPath, file.FileName);

                if (System.IO.File.Exists(targetPath))
                {
                    var fileInfo = new FileInfo(targetPath);

                    if (fileInfo.Length == file.ContentLength)
                    {
                        skipFile = true;
                    }
                    else
                    {
                        targetPath = targetPath.Insert(targetPath.LastIndexOf("."), "_" + this.User.Identity.Name);

                        if (System.IO.File.Exists(targetPath))
                        {
                            var fileInfoUser = new FileInfo(targetPath);

                            if (fileInfoUser.Length == file.ContentLength)
                            {
                                skipFile = true;
                            }
                            else
                            {
                                targetPath = targetPath.Insert(targetPath.LastIndexOf("."), DateTime.UtcNow.ToString("_yyyyMMdd_HHmmss"));
                            }
                        }
                    }
                }

                if (!skipFile)
                {
                    file.SaveAs(targetPath);

                    this.photoRepository.AddPhoto(uploadViewModel.AlbumName, uploadViewModel.CategoryId, this.currentUserContainer.User, targetPath);
                }

                reponseObjects.Add(
                    new
                        {
                            name = file.FileName,
                            size = file.ContentLength,
                            delete_url = "http://none",
                            delete_type = "none",
                            url = "http://none",
                            thumbnail_url = "http://none"
                        });
            }

            return this.Json(new { files = reponseObjects });
        }
    }
}
