using System.Collections.Generic;

namespace MediaCommMvc.Web.ViewModels
{
    public class AlbumViewModel
    {
        public string AlbumName { get; set; }

        public IEnumerable<PhotoViewModel> Photos { get; set; }
    }
}