using System;
using System.Collections.Generic;
using System.Linq;

using MediaCommMvc.Web.Infrastructure.DataInterfaces;
using MediaCommMvc.Web.Models.Forums;
using MediaCommMvc.Web.Models.Users;

using NHibernate;
using NHibernate.Linq;

namespace MediaCommMvc.Web.Infrastructure.Nh.Repositories
{
    public class ForumRepository : IForumRepository
    {
        private readonly CurrentUserContainer currentUserContainer;

        private readonly ISessionContainer sessionContainer;

        private readonly TimeSpan topicUnreadValidity = new TimeSpan(30, 0, 0, 0);

        public ForumRepository(ISessionContainer sessionContainer, CurrentUserContainer currentUserContainer)
        {
            this.sessionContainer = sessionContainer;
            this.currentUserContainer = currentUserContainer;
        }

        private MediaCommUser MediaCommUser
        {
            get
            {
                return this.currentUserContainer.User;
            }
        }

        private ISession Session
        {
            get
            {
                return this.sessionContainer.CurrentSession;
            }
        }

        public void AddForum(Forum forum)
        {
            this.Session.Save(forum);
        }

        public void AddPost(Post post)
        {
            this.Session.Save(post);
            this.UpdateLastPostInfo(post);

            this.SetTopicReadDate(post.Topic.Id);
        }

        public Topic AddTopic(Topic topic, Post post)
        {
            topic.LastPostAuthor = post.Author.UserName;
            topic.LastPostTime = DateTime.UtcNow;
            topic.CreatedBy = post.Author.UserName;
            topic.Created = DateTime.UtcNow;
            post.Created = DateTime.UtcNow;
            post.Topic = topic;

            this.Session.Save(topic);
            this.Session.Save(post);

            this.UpdateLastPostInfo(post);

            this.SetTopicReadDate(topic.Id);

            return topic;
        }

        public void DeleteForum(Forum forum)
        {
            this.Session.Query<Topic>().Where(t => t.Forum == forum).ToList().ForEach(this.DeleteTopic);
        }

        public void DeletePost(Post post)
        {
            if (post.Author != this.MediaCommUser && !this.MediaCommUser.IsAdmin)
            {
                throw new UnauthorizedAccessException("Only Admins can delete posts made by other users");
            }

            this.Session.Delete(post);

            // delete the topic, if the post is the first one.
            if (this.PostWasTheFirstInTopic(post))
            {
                this.DeleteTopic(post.Topic);

                Post lastPost =
                    this.Session.Query<Post>().Where(p => p.Topic.Forum.Id == post.Topic.Forum.Id).OrderByDescending(p => p.Created).ThenByDescending(
                        p => p.Id).First();

                this.UpdateLastPostInfo(lastPost);
            }
            else
            {
                Post lastPost =
                    this.Session.Query<Post>().Where(p => p.Topic.Id == post.Topic.Id).OrderByDescending(p => p.Created).ThenByDescending(p => p.Id).
                        First();

                this.UpdateLastPostInfo(lastPost);
            }
        }

        public IEnumerable<Forum> GetAllForums()
        {
            List<Forum> allForums = this.Session.Query<Forum>().ToList();

            // FutureValue does not work, because "Sql queries in MultiQuery are currently not supported."
            foreach (Forum forum in allForums)
            {
                forum.HasUnreadTopics =
                    bool.Parse(
                        this.Session.CreateSQLQuery(
                            @"select case when COUNT(id) = 0 then 'False' else 'True' end
						from ForumTopics 
						where ForumID = :forumId
							and LastPostTime > DATEADD(day, -30, GETDATE())
							and Id not in 
								(select ReadTopicID from ForumTopicsRead where ReadByUserID = :userId and LastVisit > DATEADD(day, -30, GETDATE()) and LastVisit > LastPostTime)
                            and Id not in
                                (SELECT TopicId FROM ForumTopicsExcludedUsers WHERE MediaCommUserId = :userId)")
                            .SetParameter("forumId", forum.Id).SetParameter("userId", this.MediaCommUser.Id).UniqueResult<string>());
            }

            return allForums;
        }

        public Post GetFirstUnreadPostForTopic(int id)
        {
            /* joins are not supported by linq to nhibernate
             * Post post = (from p in this.Session.Query<Post>()
                       join tr in this.Session.Query<TopicRead>() on p.Topic.Id equals tr.ReadTopic.Id
                       where p.Topic.Id == id && tr.ReadByUser.UserName == user.UserName && p.Created > tr.LastVisit
                       orderby p.Created descending
                       select p).FirstOrDefault();*/
            DateTime date =
                (this.Session.Query<TopicRead>().SingleOrDefault(
                    tr => tr.ReadByUser.UserName == this.currentUserContainer.UserName && tr.ReadTopic.Id == id) ??
                 new TopicRead { LastVisit = DateTime.UtcNow.AddMonths(-1) }).LastVisit;

            // Get First unread post or the newest one if all are read
            Post post = this.Session.Query<Post>().Where(p => p.Topic.Id == id && p.Created > date).OrderBy(p => p.Created).FirstOrDefault() ??
                        this.Session.Query<Post>().Where(p => p.Topic.Id == id).OrderByDescending(p => p.Created).First();

            return post;
        }

        public Forum GetForumById(int id)
        {
            Forum forum = this.Session.Get<Forum>(id);

            return forum;
        }

        public int GetLastPageNumberForTopic(int topicId, int pageSize)
        {
            int totalPostCount = this.Session.Query<Post>().Count(p => p.Topic.Id == topicId);
            int lastPage = ((totalPostCount - 1) / pageSize) + 1;

            return lastPage;
        }

        public int GetPageNumberForPost(int postId, int topicId, int pageSize)
        {
            List<Post> posts = this.Session.Query<Post>().Where(p => p.Topic.Id == topicId).OrderBy(p => p.Created).ToList();
            Post post = posts.Single(p => p.Id == postId);

            int index = posts.IndexOf(post);

            int page = (index / pageSize) + 1;

            return page;
        }

        public Post GetPostById(int id)
        {
            Post post = this.Session.Query<Post>().Fetch(p => p.Topic).ThenFetch(t => t.Forum).Single(p => p.Id == id);

            return post;
        }

        public IEnumerable<Post> GetPostsForTopic(int topicId, PagingParameters pagingParameters)
        {
            int postsToSkip = (pagingParameters.CurrentPage - 1) * pagingParameters.PageSize;

            List<Post> posts =
                this.Session.Query<Post>().Where(p => p.Topic.Id == topicId).OrderBy(p => p.Created).Skip(postsToSkip).Take(pagingParameters.PageSize)
                    .ToList();

            int lastPage = ((pagingParameters.TotalCount - 1) / pagingParameters.PageSize) + 1;
            bool isLastPage = pagingParameters.CurrentPage == lastPage;

            if (isLastPage)
            {
                this.SetTopicReadDate(topicId);
            }

            return posts;
        }

        public Topic GetTopicById(int id)
        {
            Topic topic =
                this.Session.Query<Topic>().Fetch(t => t.Forum).SingleOrDefault(
                    t => t.Id == id && !t.ExcludedUsers.Contains(this.MediaCommUser));

            return topic;
        }

        public IEnumerable<Topic> GetTopicsForForum(int forumId, PagingParameters pagingParameters)
        {
            List<Topic> topics =
                this.Session.Query<Topic>().Where(t => t.Forum.Id == forumId && !t.ExcludedUsers.Contains(this.MediaCommUser)).OrderByDescending(
                    t => t.DisplayPriority).ThenByDescending(t => t.LastPostTime).ThenByDescending(t => t.Id).Skip(
                        (pagingParameters.CurrentPage - 1) * pagingParameters.PageSize).Take(pagingParameters.PageSize).ToList();

            List<int> topicIds = topics.Select(t => t.Id).ToList();

            ILookup<int, string> excludedUsernames =
                this.Session.Query<ForumTopicsExcludedUser>().Where(ex => topicIds.Contains(ex.Topic.Id)).ToLookup(
                    ex => ex.Topic.Id, ex => ex.MediaCommUser.UserName);

            foreach (Topic topic in topics)
            {
                topic.ExcludedUsernames = excludedUsernames.FirstOrDefault(ex => ex.Key == topic.Id);
            }

            this.UpdateTopicReadStatus(topics.Where(t => t.LastPostTime > DateTime.UtcNow - this.topicUnreadValidity));

            return topics;
        }

        public IEnumerable<Topic> GetTopicsWithNewestPosts()
        {
            List<Topic> topics =
                this.Session.Query<Topic>().Where(t => !t.ExcludedUsers.Contains(this.MediaCommUser)).OrderByDescending(t => t.LastPostTime).Take(6).
                    ToList();

            this.UpdateTopicReadStatus(topics.Where(t => t.LastPostTime > DateTime.UtcNow - this.topicUnreadValidity));

            return topics;
        }

        public void UpdatePost(Post post)
        {
            this.Session.Update(post);
        }

        private void DeleteTopic(Topic topic)
        {
            this.Session.Query<Post>().Where(p => p.Topic.Id == topic.Id).ToList().ForEach(this.Session.Delete);
            this.Session.Query<TopicRead>().Where(tr => tr.ReadTopic.Id == topic.Id).ForEach(this.Session.Delete);

            this.Session.Delete(topic);
        }

        private bool PostWasTheFirstInTopic(Post post)
        {
            bool topicHasOlderPosts = this.Session.Query<Post>().Where(p => p.Topic.Id == post.Topic.Id && p.Id < post.Id).Any();

            return !topicHasOlderPosts;
        }

        private void SetTopicReadDate(int topicId)
        {
            TopicRead topicRead =
                this.Session.Query<TopicRead>().SingleOrDefault(r => r.ReadByUser.Id == this.MediaCommUser.Id && r.ReadTopic.Id == topicId) ??
                new TopicRead { ReadByUser = this.MediaCommUser, ReadTopic = this.Session.Load<Topic>(topicId) };

            // Add one second to prevent false unread results for own posts 
            topicRead.LastVisit = DateTime.UtcNow.AddSeconds(1);

            this.Session.SaveOrUpdate(topicRead);
        }

        private void UpdateLastPostInfo(Post post)
        {
            post.Topic.LastPostTime = post.Created;
            post.Topic.LastPostAuthor = post.Author.UserName;

            post.Topic.Forum.LastPostAuthor = post.Author.UserName;
            post.Topic.Forum.LastPostTime = post.Created;

            this.Session.Update(post);
        }

        private void UpdateTopicReadStatus(IEnumerable<Topic> topics)
        {
            List<TopicRead> readTopics =
                this.Session.Query<TopicRead>().Fetch(tr => tr.ReadTopic).Fetch(tr => tr.ReadByUser).Where(
                    tr => tr.LastVisit > DateTime.UtcNow - this.topicUnreadValidity && tr.ReadByUser.Id == this.MediaCommUser.Id).ToList();

            foreach (Topic topic in topics)
            {
                topic.ReadByCurrentUser =
                    readTopics.Any(r => r.ReadByUser.Id == this.MediaCommUser.Id && r.ReadTopic.Id == topic.Id && topic.LastPostTime < r.LastVisit);
            }
        }
    }
}