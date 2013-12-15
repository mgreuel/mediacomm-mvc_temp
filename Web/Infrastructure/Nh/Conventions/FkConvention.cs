using System;

using FluentNHibernate;
using FluentNHibernate.Conventions;

namespace MediaCommMvc.Web.Infrastructure.Nh.Conventions
{
    public class FKConvention : ForeignKeyConvention
    {
        protected override string GetKeyName(Member property, Type type)
        {
            string fk = property == null ? type.Name + "ID" : property.Name + "ID";

            return fk;
        }
    }
}