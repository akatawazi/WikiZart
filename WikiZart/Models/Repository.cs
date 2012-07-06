using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Linq.Expressions;
using Raven.Client;
using Raven.Client.Document;

namespace WikiZart.Models
{
    public interface IRepository<TEntity>
    {
        IQueryable<TEntity> All();
        IQueryable<TEntity> Where(Expression<Func<TEntity, bool>> expression);

        void Add(TEntity entity);
        void Remove(TEntity entity);
    }

    public class RepositorySession
    {
        private IDocumentSession session;

        public RepositorySession(IDocumentSession documentSession) { session = documentSession; }
        public T Load<T>(string id) { return session.Load<T>(id); }
        public void SaveChanges() { session.SaveChanges(); }
    }

    public class Repository<T> : IRepository<T> where T : class
    {
        protected IDocumentSession DocumentSession { get; private set; }
        public RepositorySession Session { get; private set; }


        public Repository(IDocumentSession session)
        {
            if (session == null)
                throw new ArgumentNullException("session");

            DocumentSession = session;
            Session = new RepositorySession(DocumentSession);
        }

        public virtual IQueryable<T> All()
        {
            var item = DocumentSession.Query<T>();
            return item;
        }

        public virtual IQueryable<T> Get(string id)
        {
            return DocumentSession.Query<T>(id);
        }

        public virtual IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return All().Where(predicate);
        }

        public virtual void Add(T entity)
        {
            DocumentSession.Store(entity);
        }

        public virtual void Remove(T entity)
        {
            DocumentSession.Delete(entity);
        }
    }

}