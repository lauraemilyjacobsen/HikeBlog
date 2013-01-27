using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using HikeBlog.Models;
using System.Xml.Linq;

namespace HikeBlog.Controllers
{
    public class ForecastController : Controller
    {
        //
        // GET: /Forecast/

        private BlogDbContext db = new BlogDbContext();

        public ActionResult Index()
        {            
            // get the viewModel, which gets the forecast data
            //WeekendForecastViewModel viewModel = new WeekendForecastViewModel();
            //return View(viewModel);
            return View();
        }

        //[OutputCache(Duration=1800)]
        public ActionResult WeekendForecast()
        {
            WeekendForecastViewModel viewModel = new WeekendForecastViewModel();
            return View(viewModel);
        }

        //
        // GET: /Forecast/Details/5

        public ActionResult Details(int id)
        {
            return View();
        }

        //
        // GET: /Forecast/Create

        public ActionResult Create()
        {
            return View();
        }

        //
        // POST: /Forecast/Create

        [HttpPost]
        public ActionResult Create(FormCollection collection)
        {
            try
            {
                // TODO: Add insert logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Forecast/Edit/5

        public ActionResult Edit(int id)
        {
            return View();
        }

        //
        // POST: /Forecast/Edit/5

        [HttpPost]
        public ActionResult Edit(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add update logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }

        //
        // GET: /Forecast/Delete/5

        public ActionResult Delete(int id)
        {
            return View();
        }

        //
        // POST: /Forecast/Delete/5

        [HttpPost]
        public ActionResult Delete(int id, FormCollection collection)
        {
            try
            {
                // TODO: Add delete logic here

                return RedirectToAction("Index");
            }
            catch
            {
                return View();
            }
        }
    }
}
