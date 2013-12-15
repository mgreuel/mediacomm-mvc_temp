using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

using MediaCommMvc.Web.Models.Forums;

namespace MediaCommMvc.Web.Infrastructure.Nh.Mapping
{
    public class ForumTopicsExcludedUserMapper : IAutoMappingOverride<ForumTopicsExcludedUser>
    {
        public void Override(AutoMapping<ForumTopicsExcludedUser> mapping)
        {
            mapping.Table("ForumTopicsExcludedUsers");
            mapping.CompositeId().KeyReference(ex => ex.MediaCommUser, "MediaCommUserID").KeyReference(ex => ex.Topic, "TopicID");
            mapping.References(ex => ex.Topic);
            mapping.References(ex => ex.MediaCommUser);
        }
    }
}