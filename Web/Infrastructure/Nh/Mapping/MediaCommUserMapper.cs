using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

using MediaCommMvc.Web.Models.Users;

namespace MediaCommMvc.Web.Infrastructure.Nh.Mapping
{
    public class MediaCommUserMapper : IAutoMappingOverride<MediaCommUser>
    {
        public void Override(AutoMapping<MediaCommUser> mapping)
        {
            mapping.Table("UserProfiles");
            mapping.Map(u => u.DateOfBirth).Default("null");
            mapping.Map(u => u.UserName).Not.Nullable().UniqueKey("UK_Username");
        }
    }
}