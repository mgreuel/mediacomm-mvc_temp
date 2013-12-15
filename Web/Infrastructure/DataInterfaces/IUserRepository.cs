using System;
using System.Collections.Generic;

using MediaCommMvc.Web.Models.Users;

namespace MediaCommMvc.Web.Infrastructure.DataInterfaces
{
    public interface IUserRepository
    {
        IEnumerable<MediaCommUser> GetAllUsers();

        IEnumerable<string> GetMailAddressesToNotifyAboutNewPhotos();

        IEnumerable<string> GetMailAddressesToNotifyAboutNewPost();

        MediaCommUser GetUserByName(string userName);

        void UpdateLastForumsNotification(IEnumerable<string> notifiedMailAddresses, DateTime notificationTime);

        void UpdateLastPhotosNotification(IEnumerable<string> notifiedMailAddresses, DateTime notificationTime);

        void UpdateUser(MediaCommUser user);
    }
}