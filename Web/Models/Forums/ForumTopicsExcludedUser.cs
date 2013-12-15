using MediaCommMvc.Web.Models.Users;

namespace MediaCommMvc.Web.Models.Forums
{
    public class ForumTopicsExcludedUser
    {
        public virtual MediaCommUser MediaCommUser { get; set; }

        public virtual Topic Topic { get; set; }

        public override bool Equals(object obj)
        {
            ForumTopicsExcludedUser otherExclusion = obj as ForumTopicsExcludedUser;
            if (otherExclusion == null || otherExclusion.Topic == null || otherExclusion.MediaCommUser == null || this.MediaCommUser == null ||
                this.Topic == null)
            {
                return false;
            }

            return this.MediaCommUser.Id == otherExclusion.MediaCommUser.Id && this.Topic.Id == otherExclusion.Topic.Id;
        }

        public override int GetHashCode()
        {
            if (this.MediaCommUser == null || this.Topic == null)
            {
                return base.GetHashCode();
            }

            return this.MediaCommUser.Id | this.Topic.Id;
        }
    }
}