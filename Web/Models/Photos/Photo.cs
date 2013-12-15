using MediaCommMvc.Web.Models.Users;

namespace MediaCommMvc.Web.Models.Photos
{
    public class Photo
    {
        public virtual string FileName { get; set; }

        public virtual long FileSize { get; set; }

        public virtual int Height { get; set; }

        public virtual int Id { get; protected set; }

        public virtual PhotoAlbum PhotoAlbum { get; set; }

        public virtual MediaCommUser Uploader { get; set; }

        public virtual int ViewCount { get; set; }

        public virtual int Width { get; set; }

        public override string ToString()
        {
            return string.Format("Id: '{0}', Filename: '{1}', Album: '{2}'", this.Id, this.FileName, this.PhotoAlbum);
        }
    }
}