using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using MediaCommMvc.Web.Infrastructure;
using MediaCommMvc.Web.Infrastructure.DataInterfaces;
using MediaCommMvc.Web.Infrastructure.Helpers;
using MediaCommMvc.Web.Infrastructure.Nh;
using MediaCommMvc.Web.Models.Forums;
using MediaCommMvc.Web.Models.Users;
using MediaCommMvc.Web.ViewModels;

using PagedList;

namespace MediaCommMvc.Web.Controllers
{
    public partial class ForumsController : Controller
    {
        private const int PostsPerTopicPage = 25;

        private const int TopicsPerForumPage = 25;

        private readonly CurrentUserContainer currentUserContainer;

        private readonly IForumRepository forumRepository;

        private readonly INotificationSender notificationSender;

        private readonly IUserRepository userRepository;

        public ForumsController(
            IForumRepository forumRepository, 
            IUserRepository userRepository, 
            CurrentUserContainer currentUserContainer, 
            INotificationSender notificationSender)
        {
            this.forumRepository = forumRepository;
            this.userRepository = userRepository;
            this.currentUserContainer = currentUserContainer;
            this.notificationSender = notificationSender;
        }

        [HttpGet]
        [SessionActionFilter]
        public virtual ActionResult CreateTopic(int id)
        {
            Forum forum = this.forumRepository.GetForumById(id);
            IEnumerable<string> userNames = this.userRepository.GetAllUsers().Select(u => u.UserName);

            return this.View(new CreateTopicViewModel { Forum = forum, AllUserNames = userNames });
        }

        [AcceptVerbs(HttpVerbs.Post)]
        [ValidateInput(false)]
        [SessionActionFilter]
        public virtual ActionResult CreateTopic(Topic topic, Post post, int id, bool sticky, string excludedUsers)
        {
            post.Author = this.userRepository.GetUserByName(this.User.Identity.Name);
            topic.Forum = this.forumRepository.GetForumById(id);
            var usersToExclude = new List<MediaCommUser>();

            if (sticky)
            {
                topic.DisplayPriority = TopicDisplayPriority.Sticky;
            }

            List<string> userNamesToExclude = excludedUsers.Split(new[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Select(s => s.Trim()).ToList();

            foreach (string userName in userNamesToExclude)
            {
                MediaCommUser user = this.userRepository.GetUserByName(userName);
                usersToExclude.Add(user);
            }

            topic.ExcludedUsers = usersToExclude;


            Topic createdTopic = this.forumRepository.AddTopic(topic, post);

            this.notificationSender.SendForumsNotification(createdTopic);

            return this.RedirectToAction("Topic", new { id = createdTopic.Id, name = this.Url.ToFriendlyUrl(createdTopic.Title) });
        }

        [HttpPost]
        [SessionActionFilter]
        public virtual ActionResult DeletePost(int id)
        {
            Post postToDelete = this.forumRepository.GetPostById(id);
            this.forumRepository.DeletePost(postToDelete);

            if (this.forumRepository.GetTopicById(postToDelete.Topic.Id) != null)
            {
                return this.RedirectToAction("Topic", new { id = postToDelete.Topic.Id, name = this.Url.ToFriendlyUrl(postToDelete.Topic.Title) });
            }

            return this.RedirectToAction(
                "Forum", new { id = postToDelete.Topic.Forum.Id, name = this.Url.ToFriendlyUrl(postToDelete.Topic.Forum.Title) });
        }

        [HttpGet]
        [SessionActionFilter]
        public virtual ActionResult EditPost(int id)
        {
            Post post = this.forumRepository.GetPostById(id);

            if (post.Author != this.currentUserContainer.User && !this.currentUserContainer.User.IsAdmin)
            {
                throw new UnauthorizedAccessException("Only Administrators can edit posts made by other users");
            }

            post.Text = post.Text.Replace("<br />", "<br />\n");
            post.Text = post.Text.Replace("</p>", "</p>\n");
            post.Text = post.Text.Replace("</ul>", "</ul>\n");
            post.Text = post.Text.Replace("</li>", "</li>\n");

            return this.View(new EditPostViewModel { PostId = post.Id, Text = post.Text });
        }

        [HttpPost]
        [ValidateInput(false)]
        [SessionActionFilter]
        public virtual ActionResult EditPost(int id, Post post)
        {
            Post postToUpdate = this.forumRepository.GetPostById(id);
            postToUpdate.Text = post.Text;

            if (postToUpdate.Author != this.currentUserContainer.User && !this.currentUserContainer.User.IsAdmin)
            {
                throw new UnauthorizedAccessException("Only Administrator can edit posts made by other users");
            }

            postToUpdate.Text = HtmlSanitizer.Sanitize(postToUpdate.Text);

            this.forumRepository.UpdatePost(postToUpdate);

            string url = this.GetPostUrl(postToUpdate.Topic.Id, postToUpdate);
            return this.Redirect(url);
        }

        [HttpGet]
        [SessionActionFilter]
        public virtual ActionResult FirstNewPostInTopic(int id)
        {
            Post post = this.forumRepository.GetFirstUnreadPostForTopic(id);

            string url = this.GetPostUrl(id, post);
            return this.Redirect(url);
        }

        [HttpGet]
        [SessionActionFilter]
        public virtual ActionResult Forum(int id, int page)
        {
            var pagingParameters = new PagingParameters { CurrentPage = page, PageSize = TopicsPerForumPage };

            Forum forum = this.forumRepository.GetForumById(id);
            pagingParameters.TotalCount = forum.TopicCount;

            IEnumerable<Topic> topics = this.forumRepository.GetTopicsForForum(id, pagingParameters);

            return
                this.View(
                    new ForumViewModel { Forum = forum, Topics = topics, PagingParameters = pagingParameters, PostsPerTopicPage = PostsPerTopicPage });
        }

        [HttpGet]
        [SessionActionFilter]
        public virtual ActionResult Index()
        {
            MediaCommUser currentUser = this.currentUserContainer.User;
            currentUser.LastVisit = DateTime.Now;

            this.userRepository.UpdateUser(currentUser);

            return this.View(new ForumsViewModel { Forums = this.forumRepository.GetAllForums() });
        }

        [HttpPost]
        [ValidateInput(false)]
        [SessionActionFilter]
        public virtual ActionResult Reply(ReplyViewModel viewModel)
        {
            Post post = new Post();
            post.Text = HtmlSanitizer.Sanitize(viewModel.Text);
            post.Topic = this.forumRepository.GetTopicById(viewModel.TopicId);
            post.Author = this.userRepository.GetUserByName(this.User.Identity.Name);
            post.Created = DateTime.UtcNow;

            this.forumRepository.AddPost(post);

            this.notificationSender.SendForumsNotification(post);

            return this.Redirect(this.GetPostUrl(viewModel.TopicId, post));
        }

        [HttpGet]
        [SessionActionFilter]
        public virtual ActionResult Topic(int id, int page)
        {
            var pagingParameters = new PagingParameters { CurrentPage = page, PageSize = PostsPerTopicPage };

            Topic topic = this.forumRepository.GetTopicById(id);

            if (topic == null)
            {
                throw new HttpException(404, "HTTP/1.1 404 Not Found");
            }

            pagingParameters.TotalCount = topic.PostCount;

            IPagedList<Post> posts = this.forumRepository.GetPostsForTopic(id, pagingParameters).ToPagedList(page, PostsPerTopicPage);

            return this.View(new TopicPageViewModel { Topic = topic, Posts = posts });
        }

        private string GetPostUrl(int topicId, Post post)
        {
            int page = this.forumRepository.GetPageNumberForPost(post.Id, topicId, PostsPerTopicPage);
            return this.Url.GetPostUrl(post, page);
        }
    }
}