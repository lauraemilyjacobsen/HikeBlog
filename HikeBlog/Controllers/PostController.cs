using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HikeBlog.Models;
using FlickrNet;
using System.Text;

namespace HikeBlog.Controllers
{
    public class PostController : Controller
    {
        private BlogDbContext db = new BlogDbContext();

        //
        // GET: /Post/

        [Authorize(Users="laurafyodorova")]
        public ActionResult India()
        {
            return View(db.Posts.ToList());
        }

        //
        // GET: /Post/Details/5
        [Authorize(Users = "laurafyodorova")]
        public ActionResult Denmark(int? id, string title)
        {
            Post post = db.Posts.Find(id);            
            if (post == null)
            {
                return HttpNotFound();
            }

            return View(post);
        }

        //
        // GET: /Post/Create
        [Authorize(Users = "laurafyodorova")]
        public ActionResult Croatia()
        {
            return View();
        }

        //
        // POST: /Post/Create

        [HttpPost]
        [Authorize(Users = "laurafyodorova")]
        public ActionResult Croatia(Post post)
        {
            if (ModelState.IsValid)
            {
                post.Slug = GetSlug(post.Title);
                db.Posts.Add(post);
                db.SaveChanges();
                return RedirectToAction("India");
            }

            return View(post);
        }

        //
        // GET: /Post/Edit/5
        [Authorize(Users = "laurafyodorova")]
        public ActionResult Egypt(int id = 0)
        {
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        //
        // POST: /Post/Edit/5

        [HttpPost]
        [Authorize(Users = "laurafyodorova")]
        public ActionResult Egypt(Post post)
        {
            if (ModelState.IsValid)
            {
                db.Entry(post).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("India");
            }
            return View(post);
        }

        //
        // GET: /Post/Delete/5
        [Authorize(Users = "laurafyodorova")]
        public ActionResult Djibouti(int id = 0)
        {
            Post post = db.Posts.Find(id);
            if (post == null)
            {
                return HttpNotFound();
            }
            return View(post);
        }

        //
        // POST: /Post/Delete/5

        [Authorize(Users = "laurafyodorova")]
        [HttpPost, ActionName("Delete")]
        public ActionResult DjiboutiConfirmed(int id)
        {
            Post post = db.Posts.Find(id);
            db.Posts.Remove(post);
            db.SaveChanges();
            return RedirectToAction("India");
        }

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }

        protected string GetSlug(string title)
        {
            title = (title ?? "").Trim().ToLower();

            StringBuilder url = new StringBuilder();

            foreach (char ch in title)
            {
                switch (ch)
                {
                    case ' ':
                        url.Append('-');
                        break;
                    case '&':
                        url.Append("and");
                        break;
                    case '\'':
                        break;
                    default:
                        if ((ch >= '0' && ch <= '9') ||
                            (ch >= 'a' && ch <= 'z'))
                        {
                            url.Append(ch);
                        }
                        else
                        {
                            url.Append('-');
                        }
                        break;
                }
            }
            return url.ToString();
        }
    }
}