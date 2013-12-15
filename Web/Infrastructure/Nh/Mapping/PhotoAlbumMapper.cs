using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

using MediaCommMvc.Web.Models.Photos;

namespace MediaCommMvc.Web.Infrastructure.Nh.Mapping
{
    public class PhotoAlbumMapper : IAutoMappingOverride<PhotoAlbum>
    {
        public void Override(AutoMapping<PhotoAlbum> mapping)
        {
            mapping.Table("PhotoAlbums");
            mapping.Map(a => a.Name).Not.Nullable().UniqueKey("uk_nameCat");
            mapping.References(a => a.PhotoCategory).Not.Nullable().UniqueKey("uk_nameCat").ForeignKey("PhotoCategoryID").Not.LazyLoad().Cascade.SaveUpdate();
            mapping.Map(a => a.PhotoCount).Formula("(SELECT COUNT(*) FROM photos WHERE photos.PhotoAlbumID = Id)");
            mapping.Map(a => a.CoverPhotoId).Formula("(SELECT TOP 1 photos.Id FROM photos WHERE photos.PhotoAlbumID = Id ORDER BY photos.ViewCount)");
            mapping.HasMany(a => a.Photos).Inverse();
        }
    }
}