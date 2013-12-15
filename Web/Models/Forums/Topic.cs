using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

using MediaCommMvc.Web.Models.Users;

namespace MediaCommMvc.Web.Models.Forums
{
    public class Topic
    {
        public Topic()
        {
            this.ReadByCurrentUser = true;
        }

        public virtual DateTime Created { get; set; }

        public virtual string CreatedBy { get; set; }

        public virtual TopicDisplayPriority DisplayPriority { get; set; }

        public virtual IEnumerable<string> ExcludedUsernames { get; set; }

        public virtual IEnumerable<MediaCommUser> ExcludedUsers { get; set; }

        public virtual Forum Forum { get; set; }

        public virtual int Id { get; set; }

        public virtual string LastPostAuthor { get; set; }

        public virtual DateTime LastPostTime { get; set; }

        public virtual int PostCount { get; set; }

        public virtual bool ReadByCurrentUser { get; set; }

        [Required]
        [StringLength(255)]
        public virtual string Title { get; set; }

        public override string ToString()
        {
            return string.Format("Id: '{0}', Title: '{1}", this.Id, this.Title);
        }
    }
}