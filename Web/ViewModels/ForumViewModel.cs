using System.Collections.Generic;

using MediaCommMvc.Web.Infrastructure;
using MediaCommMvc.Web.Models.Forums;

namespace MediaCommMvc.Web.ViewModels
{
    public class ForumViewModel
    {
        public Forum Forum { get; set; }
        public PagingParameters PagingParameters { get; set; }
        public int PostsPerTopicPage { get; set; }
        public IEnumerable<Topic> Topics { get; set; }
    }
}