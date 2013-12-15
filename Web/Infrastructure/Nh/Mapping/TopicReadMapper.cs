using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

using MediaCommMvc.Web.Models.Forums;

namespace MediaCommMvc.Web.Infrastructure.Nh.Mapping
{
    public class TopicReadMapper : IAutoMappingOverride<TopicRead>
    {
        public void Override(AutoMapping<TopicRead> mapping)
        {
            mapping.Table("ForumTopicsRead");
            mapping.References(tr => tr.ReadByUser).Not.LazyLoad();
            mapping.References(tr => tr.ReadTopic).Not.LazyLoad();
        }
    }
}