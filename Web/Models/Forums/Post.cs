using System;

using MediaCommMvc.Web.Models.Users;

namespace MediaCommMvc.Web.Models.Forums
{
    public class Post
    {
        public virtual MediaCommUser Author { get; set; }

        public virtual DateTime Created { get; set; }

        public virtual int Id { get; protected set; }

        public virtual string Text { get; set; }

        public virtual Topic Topic { get; set; }

        public override string ToString()
        {
            string textStart = this.Text.Length > 20 ? this.Text.Substring(0, 20) : this.Text;

            return string.Format("ID: '{0}', Author: '{1}, Text: '{2}'", this.Id, this.Author, textStart);
        }
    }
}