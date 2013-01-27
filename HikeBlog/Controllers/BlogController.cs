using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using PagedList;
using HikeBlog.Models;

namespace HikeBlog.Controllers
{
    public class BlogController : Controller
    {
        BlogDbContext db = new BlogDbContext();
        
        //
        // GET: /Blog/

        public ActionResult Index(int? page, string q)
        {
            var posts = from p in db.Posts
                        where p.IsPublished
                        select p;

            if (q == "Search") q = String.Empty;

            if (!String.IsNullOrWhiteSpace(q))
            {
                posts = posts.Where(p => p.Content.ToUpper().Contains(q.ToUpper())
                    || p.Title.ToUpper().Contains(q.ToUpper()));

                ViewBag.CurrentSearch = q;
            }
            
            int pageSize = 5;
            int pageNumber = (page ?? 1);

            return View(posts.OrderByDescending(p => p.DatePublished).ToPagedList(pageNumber, pageSize));
        }

        public ActionResult Post(int? id, string title)
        {
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }
    }
}
