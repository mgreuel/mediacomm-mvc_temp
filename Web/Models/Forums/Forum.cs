using System;
using System.ComponentModel.DataAnnotations;

namespace MediaCommMvc.Web.Models.Forums
{
    public class Forum
    {
        [StringLength(100)]
        public virtual string Description { get; set; }

        public virtual int DisplayOrderIndex { get; set; }

        public virtual bool HasUnreadTopics { get; set; }

        public virtual int Id { get; protected set; }

        public virtual string LastPostAuthor { get; set; }

        public virtual DateTime? LastPostTime { get; set; }

        public virtual int PostCount { get; protected set; }

        [Required]
        [StringLength(50)]
        public virtual string Title { get; set; }

        public virtual int TopicCount { get; protected set; }

        public override string ToString()
        {
            return string.Format("ID: '{0}', Title: '{1}'", this.Id, this.Title);
        }
    }
}