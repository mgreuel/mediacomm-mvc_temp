using System.Web.Mvc;

using MediaCommMvc.Web.Models.Forums;

namespace MediaCommMvc.Web.Infrastructure.Helpers
{
    public static class Icons
    {
        public static string TopicIconClass(this UrlHelper helper, Topic topic)
        {
            if (!topic.ReadByCurrentUser)
            {
                return "icon-eye-open";
            }

            if (topic.DisplayPriority == TopicDisplayPriority.Sticky)
            {
                return "icon-exclamation-sign";
            }

            return "icon-eye-close";
        }
    }
}