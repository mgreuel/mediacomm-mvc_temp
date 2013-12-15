using System.Collections.Generic;

using MediaCommMvc.Web.Models.Forums;

namespace MediaCommMvc.Web.ViewModels
{
    public class ForumsViewModel 
    {
        public IEnumerable<Forum> Forums { get; set; }
    }
}