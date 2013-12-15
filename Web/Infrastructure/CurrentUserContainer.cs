using System.Security.Principal;

using MediaCommMvc.Web.Infrastructure.DataInterfaces;
using MediaCommMvc.Web.Models.Users;

namespace MediaCommMvc.Web.Infrastructure
{
    public class CurrentUserContainer
    {
        private readonly IIdentity mediaCommIdentity;

        private readonly IUserRepository userRepository;

        private MediaCommUser user;

        public CurrentUserContainer(IUserRepository userRepository, IIdentity mediaCommIdentity)
        {
            this.userRepository = userRepository;
            this.mediaCommIdentity = mediaCommIdentity;
        }

        public MediaCommUser User
        {
            get
            {
                return this.user ?? (this.user = this.userRepository.GetUserByName(this.UserName));
            }
        }

        public string UserName
        {
            get
            {
                return this.mediaCommIdentity.Name;
            }
        }
    }
}