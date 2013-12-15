using System;
using System.Collections.Generic;

namespace MediaCommMvc.Web.Models.Photos
{
    public class PhotoAlbum
    {
        public virtual int CoverPhotoId { get; protected set; }

        public virtual int Id { get; protected set; }

        public virtual DateTime LastPicturesAdded { get; set; }

        public virtual string Name { get; set; }

        public virtual PhotoCategory PhotoCategory { get; set; }

        public virtual int PhotoCount { get; protected set; }

        public virtual IEnumerable<Photo> Photos { get; protected set; }

        public override string ToString()
        {
            string categoryName = this.PhotoCategory == null ? string.Empty : this.PhotoCategory.Name;

            return string.Format("Id: '{0}, Title: '{1}', Category: '{2}'", this.Id, this.Name, categoryName);
        }
    }
}