using System;
using System.Web;

using NHibernate;

namespace MediaCommMvc.Web.Infrastructure.Nh
{
    public sealed class HttpContextSessionContainer : ISessionContainer
    {
        public ISession CurrentSession
        {
            get
            {
                if (InternalSession == null)
                {
                    throw new SessionNotInitializedException(
                        "The NH session has not been initialized. Make sure all actions accessing the DB have the NHibernateActionFilter attribute.");
                }

                return InternalSession;
            }

            set
            {
                InternalSession = value;
            }
        }

        private static ISession InternalSession
        {
            get
            {
                return HttpContext.Current.Items["NHibernateSession"] as ISession;
            }

            set
            {
                HttpContext.Current.Items["NHibernateSession"] = value;
            }
        }
    }

    public class SessionNotInitializedException : Exception
    {
        public SessionNotInitializedException(string message) : base(message)
        {
        }
    }
}