using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Entity;

namespace HikeBlog.Models
{
    public class DbInitializer : CreateDatabaseIfNotExists<BlogDbContext>
    {
        protected override void Seed(BlogDbContext context)
        {            
            var locations = new List<Location>
            {
                new Location { Name="Artist Point", Latitude=48.84898M, Longitude=-121.72169M, MoreWeatherInformationUrl="http://forecast.weather.gov/MapClick.php?lat=48.84777305764469&lon=-121.69366836547852&site=sew&unit=0&lg=en&FcstType=text" },
                new Location { Name="Rainy Pass (Hwy 20)", Latitude=48.5195798M, Longitude=-120.7339946M, MoreWeatherInformationUrl="http://forecast.weather.gov/MapClick.php?lat=48.505687108189775&lon=-120.71949005126953&site=sew&unit=0&lg=en&FcstType=text" },
                new Location { Name="Verlot (Mtn Loop Hwy)", Latitude=48.0970481M, Longitude=-121.7890158M, MoreWeatherInformationUrl="http://forecast.weather.gov/MapClick.php?CityName=Verlot&state=WA&site=SEW&textField1=48.0906&textField2=-121.776&e=0" },
                new Location { Name="Leavenworth", Latitude=47.5941143M, Longitude=-120.66374M, MoreWeatherInformationUrl="http://forecast.weather.gov/MapClick.php?CityName=Leavenworth&state=WA&site=OTX&textField1=47.5964&textField2=-120.66&e=0" },
                new Location { Name="Stevens Pass", Latitude=47.7447M, Longitude=-121.0889M, MoreWeatherInformationUrl="http://forecast.weather.gov/MapClick.php?lat=47.75225138174104&lon=-121.124267578125&site=sew&unit=0&lg=en&FcstType=text" },
                new Location { Name="Snoqualmie Pass", Latitude=47.4100M, Longitude=-121.4058M, MoreWeatherInformationUrl="http://forecast.weather.gov/MapClick.php?CityName=Snoqualmie+Pass&state=WA&site=SEW&textField1=47.4226&textField2=-121.411&e=0" },
                new Location { Name="Paradise", Latitude=46.73M, Longitude=-121.68M, MoreWeatherInformationUrl="http://forecast.weather.gov/MapClick.php?CityName=Paradise&state=WA&site=SEW&textField1=46.7864&textField2=-121.734&e=1" }
            };

            locations.ForEach(l => context.Locations.Add(l));

            context.SaveChanges();
        }
    }
}