using System.Web.Mvc;

using MediaCommMvc.Web.Models.Forums;

namespace MediaCommMvc.Web.Infrastructure.Helpers
{
    public static class ForumsUrl
    {
        public static string GetPostUrl(this UrlHelper helper, Post post, int page)
        {
            string postAnker = string.Format("#{0}", post.Id);

            return helper.RouteUrl("ViewTopic", new { id = post.Topic.Id, page, name = helper.ToFriendlyUrl(post.Topic.Title) }) + postAnker;
        }
    }
}