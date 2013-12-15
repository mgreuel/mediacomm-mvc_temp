using System;
using System.Collections.Generic;
using System.Web.Mvc;

using MediaCommMvc.Web.Infrastructure;
using MediaCommMvc.Web.Infrastructure.DataInterfaces;
using MediaCommMvc.Web.Infrastructure.Nh;
using MediaCommMvc.Web.Models.Forums;
using MediaCommMvc.Web.Models.Photos;
using MediaCommMvc.Web.Models.Users;
using MediaCommMvc.Web.ViewModels;

namespace MediaCommMvc.Web.Controllers
{
    public partial class HomeController : Controller
    {
        private const int PostsPerTopicPage = 25;

        private readonly CurrentUserContainer currentUserContainer;

        private readonly IForumRepository forumRepository;

        private readonly IPhotoRepository photoRepository;

        private readonly IUserRepository userRepository;

        public HomeController(IForumRepository forumRepository, IPhotoRepository photoRepository, IUserRepository userRepository, CurrentUserContainer currentUserContainer)
        {
            this.forumRepository = forumRepository;
            this.photoRepository = photoRepository;
            this.userRepository = userRepository;
            this.currentUserContainer = currentUserContainer;
        }

        [SessionActionFilter]
        public virtual ActionResult Index()
        {
            IEnumerable<Topic> topicsWithNewestPosts =
                this.forumRepository.GetTopicsWithNewestPosts();

            IEnumerable<PhotoAlbum> newestPhotoAlbums = this.photoRepository.Get4NewestAlbums();

            MediaCommUser currentUser = this.currentUserContainer.User;
            currentUser.LastVisit = DateTime.UtcNow;

            this.userRepository.UpdateUser(currentUser);

            return this.View(new WhatIsNewViewModel { Topics = topicsWithNewestPosts, PostsPerTopicPage = PostsPerTopicPage, Albums = newestPhotoAlbums });
        }
    }
}