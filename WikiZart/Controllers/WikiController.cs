using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using WikiZart.Models;

namespace WikiZart.Controllers
{
    public class WikiController : Controller
    {
        //
        // GET: /Wiki/
        public ActionResult Index()
        {
            return View(Article.GetAllRootArticles());
        }
        [Authorize]
        public ActionResult ArticleEdit(string id, string subArticle, string returnUrl)
        {
            if (!String.IsNullOrEmpty(subArticle))
            {
                var article = new Article()
                {
                    ParentID = "articles/" + subArticle
                };
                return View(article);
            }
            if (!String.IsNullOrEmpty(id))
            {
                var article = Article.Get("articles/" + id);
                return View(article);
            }
            return View(new Article()); 
        }
        [HttpPost]
        [Authorize]
        [ValidateInput(false)]
        public ActionResult ArticleEdit(Article article, string returnUrl)
        {
            Article.Add(article,User.Identity.Name,Request.UserHostAddress, Request.Url.Authority.ToString());
            return RedirectToAction("Index");
        }
        public ActionResult ArticleView(string id)
        {
            var article = Article.Get("articles/" + id);
            return View(article);
        }
        public ActionResult Search(string searchTerm)
        {
            var articles = Article.Search(searchTerm);
            return Content(articles);
        }

        
    }
}
