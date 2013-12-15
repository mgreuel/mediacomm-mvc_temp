using System.Collections.Generic;

using MediaCommMvc.Web.Models.Forums;
using MediaCommMvc.Web.Models.Photos;

namespace MediaCommMvc.Web.ViewModels
{
    public class WhatIsNewViewModel
    {
        public IEnumerable<PhotoAlbum> Albums { get; set; }

        public int PostsPerTopicPage { get; set; }

        public IEnumerable<Topic> Topics { get; set; }
    }
}