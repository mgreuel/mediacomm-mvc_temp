using NHibernate;

namespace MediaCommMvc.Web.Infrastructure.Nh
{
    public interface ISessionContainer
    {
        ISession CurrentSession { get; set; }
    }
}