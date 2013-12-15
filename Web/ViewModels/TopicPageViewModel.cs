using MediaCommMvc.Web.Models.Forums;

using PagedList;

namespace MediaCommMvc.Web.ViewModels
{
    public class TopicPageViewModel 
    {
        public IPagedList<Post> Posts { get; set; }
        public Topic Topic { get; set; }
    }
}