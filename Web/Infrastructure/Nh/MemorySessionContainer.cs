using System;

using NHibernate;

namespace MediaCommMvc.Web.Infrastructure.Nh
{
    public class MemorySessionContainer : ISessionContainer, IDisposable
    {
        public ISession CurrentSession { get; set; }

        public void BeginSessionAndTransaction()
        {
            ISession session = SessionFactoryContainer.SessionFactory.OpenSession();
            session.BeginTransaction();
            this.CurrentSession = session;
        }

        public void Dispose()
        {
            this.EndSessionAndRollbackTransaftion();
        }

        public void EndSessionAndCommitTransaftion()
        {
            using (this.CurrentSession)
            {
                if (this.CurrentSession == null || this.CurrentSession.Transaction == null || !this.CurrentSession.Transaction.IsActive)
                {
                    return;
                }

                this.CurrentSession.Transaction.Commit();
            }
        }

        public void EndSessionAndRollbackTransaftion()
        {
            using (this.CurrentSession)
            {
                if (this.CurrentSession == null || this.CurrentSession.Transaction == null || !this.CurrentSession.Transaction.IsActive)
                {
                    return;
                }

                this.CurrentSession.Transaction.Rollback();
            }
        }
    }
}