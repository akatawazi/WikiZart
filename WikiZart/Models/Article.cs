using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Raven.Client;
using Raven.Client.Document;
using System.Text;

namespace WikiZart.Models
{
    public class Article
    {
        public string Id { get; set; }
        public string ParentID { get; set; }
        public string Title { get; set; }
        public string Body { get; set; }
        public string wmdMarkup { get; set; }
        public string IPAddress { get; set; }
        public string URL { get; set; }
        public DateTime LastModified { get; set; }
        public string LastModifiedBy { get; set; }
        public List<Article> ArticleHistories { get; set; }
        
        public static void Add(Article article, string username, string ip, string url)
        {
            using (var documentStore = new DocumentStore { Url = "http://localhost:8080/" })
            {
                documentStore.Initialize();
                using (var session = documentStore.OpenSession())
                {
                    article.LastModified = DateTime.Now;
                    article.LastModifiedBy = username;
                    article.IPAddress = ip;
                    article.URL = url;
                    Repository<Article> repository = new Repository<Article>(session);
                    if (String.IsNullOrEmpty(article.Id))
                    {
                        repository.Add(article);
                        session.SaveChanges();
                    }
                    else
                    {
                        var exsistingArticle = session.Query<Article>().FirstOrDefault(p => p.Id == article.Id);
                        var legacyArticle = new Article()
                        {
                            Id = exsistingArticle.Id,
                            Body = exsistingArticle.Body,
                            IPAddress = exsistingArticle.IPAddress,
                            LastModified = exsistingArticle.LastModified,
                            LastModifiedBy = exsistingArticle.LastModifiedBy,
                            Title = exsistingArticle.Title
                        };
                        if (exsistingArticle.ArticleHistories == null)
                        {
                            exsistingArticle.ArticleHistories = new List<Article>();
                        }
                        exsistingArticle.ArticleHistories.Add(legacyArticle);
                        exsistingArticle.Body = article.Body;
                        exsistingArticle.IPAddress = article.IPAddress;
                        exsistingArticle.LastModified = article.LastModified;
                        exsistingArticle.LastModifiedBy = article.LastModifiedBy;
                        exsistingArticle.Title = article.Title;
                        session.SaveChanges();
                    }
                }
            }
        }
        public static string Search(string searchTerm)
        {
            using (var documentStore = new DocumentStore { Url = "http://localhost:8080/" })
            {
                documentStore.Initialize();
                var stringBuilder = new StringBuilder();
                using (var session = documentStore.OpenSession())
                {
                    Repository<Article> repository = new Repository<Article>(session);
                    var articles = repository.Where(p => p.ParentID == null).ToList();

                    foreach (var article in articles.Where(p => (p.Title.ToLower().Contains(searchTerm.ToLower()) || p.Body.ToLower().Contains(searchTerm.ToLower())) && p.ParentID == null))
                    {
                        
                        var headerLength = article.Title.ToString().Length;
                        var headerEnding = "";
                        var bodyLength = article.Body.ToString().Length;
                        var bodyEnding = "";
                        if (headerLength > 20) { 
                            headerLength = 20; 
                            headerEnding = "..."; 
                        }
                        if (bodyLength > 72) { 
                            bodyLength = 72; 
                            bodyEnding = "..."; 
                        }
                        stringBuilder.Append("<div class='Articles' style='margin:10px; width:161px; height:106px; float:left; background:#009999;'>");
                        stringBuilder.Append("<div style=' padding:3px; vertical-align:middle; text-align:center; padding-top:2px; background:#FF7400;'>");
                        stringBuilder.Append("<a href='/Wiki/ArticleView/" + article.Id.Replace("articles/", "")  + "' >" + article.Title.Substring(0, headerLength) + headerEnding + "</a></div>");
                        stringBuilder.Append("<div style=' padding:3px; vertical-align:top; background:#009999;'>" + article.Body.Substring(0, bodyLength) + bodyEnding + "</div></div>");
                    }
                }
                return stringBuilder.ToString();
            }
        }
        public static List<Article> GetAllRootArticles()
        {
            using (var documentStore = new DocumentStore { Url = "http://localhost:8080/" })
            {
                documentStore.Initialize();
                using (var session = documentStore.OpenSession())
                {
                    Repository<Article> repository = new Repository<Article>(session);
                    return repository.Where(p => p.ParentID == null).ToList();
                }
            }
        }
        public static List<Article> GetAllSubArticles(string id)
        {
            using (var documentStore = new DocumentStore { Url = "http://localhost:8080/" })
            {
                documentStore.Initialize();
                using (var session = documentStore.OpenSession())
                {
                    Repository<Article> repository = new Repository<Article>(session);
                    return repository.Where(p => p.ParentID == id).ToList();
                }
            }
        }


        
        public static Article Get(string id)
        {
            using (var documentStore = new DocumentStore { Url = "http://localhost:8080/" })
            {
                documentStore.Initialize();
                using (var session = documentStore.OpenSession())
                {
                    return session.Load<Article>(id);
                }
            }
        }
    }
    public class SMSMessageRepository : Repository<Article>
    {
        public SMSMessageRepository(IDocumentSession session)
            : base(session) { }

        public override void Add(Article entity)
        {
            base.Add(entity);
        }

        public Article Get(string id)
        {
            return DocumentSession.Load<Article>(id);
        }

    }

}