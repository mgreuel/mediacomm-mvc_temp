using FluentNHibernate.Automapping;
using FluentNHibernate.Automapping.Alterations;

using MediaCommMvc.Web.Models;

namespace MediaCommMvc.Web.Infrastructure.Nh.Mapping
{
    public class ApprovalMapper : IAutoMappingOverride<Approval>
    {
        public void Override(AutoMapping<Approval> mapping)
        {
            mapping.Table("Approvals");
            mapping.References(a => a.ApprovedBy).Not.LazyLoad();
        }
    }
}