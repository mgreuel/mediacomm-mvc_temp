using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

using MediaCommMvc.Web.Models.Photos;

namespace MediaCommMvc.Web.Infrastructure.Nh.Mapping
{
    public class PhotoCategoryMapper : IAutoMappingOverride<PhotoCategory>
    {
        public void Override(AutoMapping<PhotoCategory> mapping)
        {
            mapping.Table("PhotoCategories");
            mapping.Map(c => c.Name).Not.Nullable().Unique();
            mapping.Map(c => c.PhotoCount).Formula(
                "(SELECT COUNT(*) FROM photos p JOIN photoAlbums a ON (p.PhotoAlbumID = a.Id) WHERE a.PhotoCategoryId = Id)");
            mapping.Map(c => c.AlbumCount).Formula("(SELECT COUNT(*) FROM photoAlbums a WHERE a.PhotoCategoryId = Id)");
            mapping.HasMany(c => c.Albums).Inverse();
        }
    }
}