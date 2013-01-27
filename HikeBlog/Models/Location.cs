using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HikeBlog.Models
{
    public class Location
    {
        public int LocationID { get; set; }
        public string Name { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string MoreWeatherInformationUrl { get; set; }
        
        public virtual List<Forecast> DailyForecasts { get; set; }
    }
}