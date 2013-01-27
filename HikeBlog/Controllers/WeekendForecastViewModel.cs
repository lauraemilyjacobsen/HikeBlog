using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using HikeBlog.Models;
using System.Xml.Linq;
using System.Web.Caching;

namespace HikeBlog.Controllers
{
    public class WeekendForecastViewModel
    {
        public List<Location> Locations { get; set; }
        public List<string> ForecastDates { get; set; }

        /// <summary>
        /// Default constructor that gets weather forecast and date information.
        /// </summary>
        public WeekendForecastViewModel()
        {
            BlogDbContext db = new BlogDbContext();
            ForecastService svc = new ForecastService();

            // get locations from the database
            Locations = db.Locations.ToList();

            List<ForecastDay> forecastDays = null;

            XDocument doc;

            // get the weekend forecast for each location
            foreach (Location l in Locations)
            {
                if (HttpContext.Current.Cache["Forecast" + l.LocationID.ToString()] == null)
                {
                    doc = svc.GetForecastXml(l.Latitude, l.Longitude);  
                  
                    // determine if the document has an error.
                    bool hasError = false;
                    if (doc.Element("dwml") == null)
                    {
                        hasError = true;
                    }

                    if (forecastDays == null)
                    {
                        forecastDays = svc.GetForecastDays(doc, hasError);

                        // make sure the dates to be listed on top of the table are in line with the forecast returned.
                        ForecastDates = new List<string>();
                        foreach (ForecastDay day in forecastDays)
                        {
                            ForecastDates.Add(day.FormattedDate);
                        }
                        HttpContext.Current.Cache.Remove("ForecastDays");
                        HttpContext.Current.Cache.Insert("ForecastDays", ForecastDates);
                    }

                    // TODO: In GetForecastObjects, check the XML to see if the forecast came through or if there was an error.
                    // Don't put it in the cache if there is an error.
                    l.DailyForecasts = svc.GetForecastObjects(doc, forecastDays, hasError);
                    
                    HttpContext.Current.Cache.Insert("Forecast" + l.LocationID.ToString(), l.DailyForecasts, null, 
                        Cache.NoAbsoluteExpiration, new TimeSpan(1, 0, 0));
                }
                else
                {
                    l.DailyForecasts = (List<Forecast>)HttpContext.Current.Cache["Forecast" + l.LocationID.ToString()];
                }
            }

            // get the forecast dates if not in the cache
            if (HttpContext.Current.Cache["ForecastDays"] == null)
            {
                ForecastDates = new List<string>();
                foreach (ForecastDay day in forecastDays)
                {
                    ForecastDates.Add(day.FormattedDate);
                }
                HttpContext.Current.Cache.Insert("ForecastDays", ForecastDates);
            }
            else
            {
                ForecastDates = (List<string>)HttpContext.Current.Cache["ForecastDays"];
            }
        }

        //TODO: Make sure the thread is always running in the west coast USA time zone.
        //private DateTime GetStartDate()
        //{  
        //    DateTime startDate = DateTime.Now;

        //    // If today is Mon-Fri, set StartDate to Saturday.
        //    if (!(DateTime.Now.DayOfWeek == DayOfWeek.Sunday))
        //    {
        //        switch (DateTime.Now.DayOfWeek)
        //        {
        //            case DayOfWeek.Monday:
        //                startDate = DateTime.Now.AddDays(5);
        //                break;
        //            case DayOfWeek.Tuesday:
        //                startDate = DateTime.Now.AddDays(4);
        //                break;
        //            case DayOfWeek.Wednesday:
        //                startDate = DateTime.Now.AddDays(3);
        //                break;
        //            case DayOfWeek.Thursday:
        //                startDate = DateTime.Now.AddDays(2);
        //                break;
        //            case DayOfWeek.Friday:
        //                startDate = DateTime.Now.AddDays(1);
        //                break;
        //        }
        //    }
        //    // If today is Saturday after 2:00 PM, or Sunday up to 2:00 PM, get Sunday data.
        //    else if ((DateTime.Now.DayOfWeek == DayOfWeek.Saturday && DateTime.Now.Hour >=14)
        //        || (DateTime.Now.DayOfWeek == DayOfWeek.Sunday && DateTime.Now.Hour < 14))
        //    {
        //        switch (DateTime.Now.DayOfWeek)
        //        {
        //            case DayOfWeek.Saturday:
        //                startDate = DateTime.Now.AddDays(1);
        //                break;
        //        }
        //    }
        //    return startDate;
        //}

        //private int GetNumDays()
        //{
        //    // determine how many days of forecasts we want 
        //    // (2 unless today's date is Sunday, then show just the current date)
        //    int numDays = 2;
        //    if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
        //    {
        //        // to get conditions forecast for a week ahead, must request a full week of data.
        //        numDays = 7;
        //    }
        //    return numDays;
        //}
    }
}