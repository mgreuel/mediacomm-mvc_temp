using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

using MediaCommMvc.Web.Models.Forums;

namespace MediaCommMvc.Web.Infrastructure.Nh.Mapping
{
    public class TopicMapper : IAutoMappingOverride<Topic>
    {
        public void Override(AutoMapping<Topic> mapping)
        {
            mapping.Table("ForumTopics");
            mapping.References(t => t.Forum).Not.Nullable().Cascade.SaveUpdate();
            mapping.Map(t => t.PostCount).Formula("(SELECT COUNT(*) FROM forumPosts p where p.TopicID = Id)");
            mapping.Map(t => t.CreatedBy).Not.Nullable();
            mapping.Map(t => t.Title).Not.Nullable();
            mapping.IgnoreProperty(t => t.ReadByCurrentUser);
            mapping.IgnoreProperty(t => t.ExcludedUsernames);
            mapping.HasManyToMany(t => t.ExcludedUsers).Table("ForumTopicsExcludedUsers").Cascade.None();
        }
    }
}