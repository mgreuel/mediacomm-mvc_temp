using System;

namespace MediaCommMvc.Web.Models.Users
{
    public class MediaCommUser
    {
        public MediaCommUser(string userName, string emailAddress, string password)
        {
            this.UserName = userName;
            this.EMailAddress = emailAddress;
            this.Password = password;
        }

        protected MediaCommUser()
        {
        }

        public virtual string City { get; set; }

        public virtual DateTime? DateOfBirth { get; set; }

        public virtual string EMailAddress { get; set; }

        public virtual string FirstName { get; set; }

        public virtual string IcqUin { get; set; }

        public virtual int Id { get; protected set; }

        public virtual bool IsAdmin { get; set; }

        public virtual string LastName { get; set; }

        public virtual DateTime? LastVisit { get; set; }

        public virtual string MobilePhoneNumber { get; set; }

        public virtual string Password { get; set; }

        public virtual string PhoneNumber { get; set; }

        public virtual string SkypeNick { get; set; }

        public virtual string Street { get; set; }

        public virtual string UserName { get; protected set; }

        public virtual string ZipCode { get; set; }

        public virtual NotificationInterval ForumsNotificationInterval { get; set; }

        public virtual NotificationInterval PhotosNotificationInterval { get; set; }

        public virtual NotificationInterval VideosNotificationInterval { get; set; }

        public virtual DateTime? LastForumsNotification { get; set; }

        public virtual DateTime? LastPhotosNotification { get; set; }

        public virtual DateTime? LastVideosNotification { get; set; }

        public override string ToString()
        {
            return string.Format("Username: '{0}', Id: '{1}", this.UserName, this.Id);
        }
    }
}