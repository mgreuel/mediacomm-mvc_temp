using System.Collections.Generic;

using MediaCommMvc.Web.Models.Forums;

namespace MediaCommMvc.Web.Infrastructure.DataInterfaces
{
    public interface IForumRepository
    {
        void AddForum(Forum forum);

        void AddPost(Post post);

        Topic AddTopic(Topic topic, Post post);

        void DeleteForum(Forum forum);

        void DeletePost(Post post);

        IEnumerable<Forum> GetAllForums();

        Post GetFirstUnreadPostForTopic(int id);

        Forum GetForumById(int id);

        int GetLastPageNumberForTopic(int topicId, int pageSize);

        int GetPageNumberForPost(int postId, int topicId, int pageSize);

        Post GetPostById(int id);

        IEnumerable<Post> GetPostsForTopic(int topicId, PagingParameters pagingParameters);

        Topic GetTopicById(int id);

        IEnumerable<Topic> GetTopicsForForum(int forumId, PagingParameters pagingParameters);

        IEnumerable<Topic> GetTopicsWithNewestPosts();

        void UpdatePost(Post post);
    }
}