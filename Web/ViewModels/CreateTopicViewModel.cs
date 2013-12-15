using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using MediaCommMvc.Web.Models.Forums;

using Resources;

namespace MediaCommMvc.Web.ViewModels
{
    public class CreateTopicViewModel
    {
        public IEnumerable<string> AllUserNames { get; set; }

        [Display(ResourceType = typeof(Forums), Name = "Hide")]
        public string ExcludedUsers { get; set; }

        public Forum Forum { get; set; }

        [Required(ErrorMessageResourceType = typeof(Forums), ErrorMessageResourceName = "SubjectRequired")]
        [Display(ResourceType = typeof(Forums), Name = "Subject")]
        public string Subject { get; set; }

        [Required(ErrorMessageResourceType = typeof(Forums), ErrorMessageResourceName = "TextRequired")]
        [Display(ResourceType = typeof(Forums), Name = "Message")]
        public string Text { get; set; }
    }
}