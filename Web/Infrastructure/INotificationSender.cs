using MediaCommMvc.Web.Models.Forums;
using MediaCommMvc.Web.Models.Photos;

namespace MediaCommMvc.Web.Infrastructure
{
    public interface INotificationSender
    {
        void SendForumsNotification(Post newPost);

        void SendForumsNotification(Topic newTopic);

        void SendPhotosNotification(PhotoAlbum albumContainingNewPhotos, string uploaderName);
    }
}