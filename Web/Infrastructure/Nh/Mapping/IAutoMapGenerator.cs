using FluentNHibernate.Automapping;

namespace MediaCommMvc.Web.Infrastructure.Nh.Mapping
{
    public interface IAutoMapGenerator
    {
        AutoPersistenceModel Generate();
    }
}