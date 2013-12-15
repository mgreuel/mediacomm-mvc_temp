using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

using MediaCommMvc.Web.Models.Photos;

namespace MediaCommMvc.Web.Infrastructure.Nh.Mapping
{
    public class PhotoMapper : IAutoMappingOverride<Photo>
    {
        public void Override(AutoMapping<Photo> mapping)
        {
            mapping.Table("Photos");
            mapping.Map(p => p.FileName).Not.Nullable().UniqueKey("uk_nameAlbum");
            mapping.References(p => p.PhotoAlbum).Not.Nullable().UniqueKey("uk_nameAlbum").Cascade.SaveUpdate();
            mapping.References(p => p.Uploader).Not.Nullable().Cascade.SaveUpdate();
        }
    }
}