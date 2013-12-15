using System;

using MediaCommMvc.Web.Models.Users;

namespace MediaCommMvc.Web.Models.Forums
{
    public class TopicRead
    {
        public virtual int Id { get; protected set; }

        public virtual DateTime LastVisit { get; set; }

        public virtual MediaCommUser ReadByUser { get; set; }

        public virtual Topic ReadTopic { get; set; }
    }
}