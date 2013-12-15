using System.ComponentModel.DataAnnotations;

using Resources;

namespace MediaCommMvc.Web.ViewModels
{
    public class UploadViewModel
    {
        [Display(ResourceType = typeof(Photos), Name = "Album")]
        [Required(ErrorMessageResourceType = typeof(Photos), ErrorMessageResourceName = "AlbumNameRequiredErrorMessage")]
        public string AlbumName { get; set; }

        public string AllAlbumNamesJson { get; set; }

        public int CategoryId { get; set; }
    }
}