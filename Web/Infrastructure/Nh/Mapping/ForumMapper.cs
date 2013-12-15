using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

using MediaCommMvc.Web.Models.Forums;

namespace MediaCommMvc.Web.Infrastructure.Nh.Mapping
{
    public class ForumMapper : IAutoMappingOverride<Forum>
    {
        public void Override(AutoMapping<Forum> mapping)
        {
            mapping.Table("Forums");
            mapping.IgnoreProperty(f => f.HasUnreadTopics);
            mapping.Map(f => f.Title).Not.Nullable();
            mapping.Map(f => f.DisplayOrderIndex).Default("0");
            mapping.Map(f => f.TopicCount).Formula("(SELECT COUNT(*) FROM forumTopics p where p.ForumID = Id)");
            mapping.Map(f => f.PostCount).Formula("(select COUNT(*) from forumPosts p JOIN forumTopics t ON (t.Id = p.TopicID) WHERE t.ForumID = Id)");
        }
    }
}