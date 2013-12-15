using NHibernate;

namespace MediaCommMvc.Web.Infrastructure.Nh
{
    public static class SessionFactoryContainer
    {
        private static readonly object sessionFactoryLock = new object();

        private static ISessionFactory sessionFactory;

        public static ISessionFactory SessionFactory
        {
            get
            {
                if (sessionFactory != null)
                {
                    return sessionFactory;
                }

                lock (sessionFactoryLock)
                {
                    return sessionFactory ?? (sessionFactory = CreateSessionFactory());
                }
            }
        }

        private static ISessionFactory CreateSessionFactory()
        {
            ConfigurationGenerator configurationGenerator = new ConfigurationGenerator();
            return configurationGenerator.Generate().BuildSessionFactory();
        }
    }
}