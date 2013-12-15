using System;

using FluentNHibernate.Automapping;
using FluentNHibernate.Cfg;
using FluentNHibernate.Cfg.Db;

using NHibernate.Cfg;

using Environment = NHibernate.Cfg.Environment;

namespace MediaCommMvc.Web.Infrastructure.Nh
{
    public sealed class ConfigurationGenerator 
    {
        public FluentConfiguration Generate()
        {
            const string NamespaceToAdd = "MediaCommMvc.Web.Models";

            AutoPersistenceModel autoMapModel =
                AutoMap.AssemblyOf<ConfigurationGenerator>().Where(
                    t => t.Namespace != null && t.Namespace.StartsWith(NamespaceToAdd, StringComparison.Ordinal)).UseOverridesFromAssemblyOf
                    <ConfigurationGenerator>().Conventions.AddFromAssemblyOf<ConfigurationGenerator>();

            var configuration = new Configuration();

            configuration.SetProperty(
                Environment.ConnectionDriver, typeof(NHibernate.Driver.Sql2008ClientDriver).AssemblyQualifiedName);

            configuration.SetProperty(Environment.CurrentSessionContextClass, "web");

            FluentConfiguration fluentConfiguration = Fluently.Configure(configuration).Database(
                MsSqlConfiguration.MsSql2008.ConnectionString(c => c.FromConnectionStringWithKey("DefaultConnection")))
                .Mappings(m => m.AutoMappings.Add(autoMapModel));

            return fluentConfiguration;
        }
    }
}