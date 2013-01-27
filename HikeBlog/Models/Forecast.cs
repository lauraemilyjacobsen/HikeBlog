using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
// added
using System.Net;
using System.Xml;
using System.Xml.Linq;
using System.Xml.XPath;
using System.ComponentModel.DataAnnotations;
using System.IO;

namespace HikeBlog.Models
{
    public class Forecast
    {
        [Key]
        public int ForecastID { get; set; }
        public int LocationID { get; set; }
        public string StartDate { get; set; }
        public string ImageFile { get; set; }
        public string WeatherSummary { get; set; }
        public string TextForecast { get; set; }
        public string PeriodName { get; set; }

        public Forecast() { }

        public Forecast(string startDate, string imageFile, string weatherSummary,
            string textForecast, string periodName)
        {
            StartDate = startDate;
            ImageFile = imageFile;
            WeatherSummary = weatherSummary;
            TextForecast = textForecast;
            PeriodName = periodName;
        }
    }

    public class ForecastService
    {
        //public List<Forecast> GetForecastsForLocation(decimal lat, decimal lon)
        //{
        //    // get the forecast XML from the web service
        //    XDocument doc = GetForecastXml(lat, lon);

        //    // determine if the data came in or if there was an error
        //    bool hasError = false;
        //    XElement dwmlElement = doc.Element("dwml");
        //    if (dwmlElement == null)
        //    {
        //        hasError = true;
        //    }

        //    // get the ordinal position of the Saturday and Sunday data in the XML
        //    List<ForecastDay> forecastDays = GetForecastDays(doc, hasError);

        //    // create the Forecast objects
        //    List<Forecast> forecasts = GetForecastObjects(doc, forecastDays, hasError);
        //    return forecasts;
        //}

        public List<Forecast> GetForecastObjects(XDocument doc, List<ForecastDay> forecastDays, bool hasError)
        {
            List<Forecast> forecasts = new List<Forecast>();            

            // read the XML to see if it contains an error message. If it does, set the text to "unavailable" 
            // and the image file to a question mark.           
            if (hasError)
            {
                // create a single Forecast with error data.
                string imageFile = "/Content/images/NoaaIcons/question.jpg";

                Forecast forecast = new Forecast("N/A", imageFile, "Error retrieving forecast",
                        "Error retrieving forecast", "N/A");

                forecasts.Add(forecast);
            }
            // there was no error, so we can expect to read the forecast data.
            else
            {
                XElement dataElement = doc
                    .Element("dwml")
                .Element("data");
                
                // get the start-valid-time elements
                var timelayouts = dataElement
                    .Descendants("time-layout");

                XElement timeLayout = null;
                foreach (var currentNode in timelayouts)
                {
                    if (currentNode.Element("layout-key") != null &&
                        (currentNode.Element("layout-key").Value == "k-p12h-n13-1"
                        || currentNode.Element("layout-key").Value == "k-p12h-n14-1"
                        || currentNode.Element("layout-key").Value == "k-p12h-n15-1"))
                    {
                        timeLayout = currentNode;
                    }
                }

                // get the contents of the <time-layout> element where the first child is a layout-key with value "k-p12h-n14-1" or "k-p12h-n13-1"
                List<XElement> startValidTimes = timeLayout
                   .Descendants("start-valid-time").ToList();

                // get the weather conditions elements
                var weatherConditions = dataElement
                    .Element("parameters")
                    .Element("weather")
                    .Descendants("weather-conditions");

                // get the forecast text elements
                var textForecasts = dataElement
                    .Element("parameters")
                    .Element("wordedForecast")
                    .Descendants("text");

                // get the image file elements
                var imageFiles = dataElement
                    .Element("parameters")
                    .Element("conditions-icon")
                    .Descendants("icon-link");

                // build as many Forecast objects as needed
                foreach (ForecastDay day in forecastDays)
                {
                    XElement elem = startValidTimes.ElementAt(day.PositionInList);
                    // period name
                    string periodName = elem.Attribute("period-name").Value;

                    // start date 
                    DateTime dtStartDate = DateTime.MinValue;
                    DateTime.TryParse(elem.Value.Substring(0, 10), out dtStartDate);
                    // TODO: Trim leading 0 from month
                    string startDate = dtStartDate.ToString("ddd MM/dd");

                    // weather summary
                    string weatherSummary = null;
                    if (weatherConditions.ElementAt(day.PositionInList)
                        .Attribute("weather-summary").Value != null)
                    {
                        weatherSummary = weatherConditions.ElementAt(day.PositionInList)
                            .Attribute("weather-summary").Value;
                    }

                    // text forecast
                    string textForecast = null;
                    if (textForecasts.ElementAt(day.PositionInList).Value != null)
                    {
                        textForecast = textForecasts.ElementAt(day.PositionInList).Value;
                    }

                    // image file
                    var iconElem = imageFiles.ElementAt(day.PositionInList);
                    string imageFile = null;
                    if (iconElem.Value != null)
                    {
                        imageFile = "/Content/images/NoaaIcons/" +
                                 iconElem.Value.Substring(iconElem.Value.LastIndexOf('/') + 1).Replace(".png", ".jpg");
                    };

                    Forecast forecast = new Forecast(startDate, imageFile, weatherSummary,
                        textForecast, periodName);
                    forecasts.Add(forecast);
                }
            }

            return forecasts;
        }

        public XDocument GetForecastXml(decimal lat, decimal lon)
        {
            // set up the URI to the REST service
            string uri = "http://forecast.weather.gov/MapClick.php?lat=" +
            lat.ToString() +
            "&lon=" + lon.ToString() +
            "&FcstType=dwml";
            string dwml = String.Empty;

            WebRequest request = WebRequest.Create(uri);
            request.Timeout = 7000;
            try
            {
                WebResponse response = request.GetResponse();
                StreamReader loResponseStream = new
                    StreamReader(response.GetResponseStream());

                dwml = loResponseStream.ReadToEnd();

                // get the XML string into an XDocument
                return XDocument.Parse(dwml);
            }
            catch (Exception ex)
            {
                XDocument errorDoc = new XDocument(
                    new XElement("error", ex.Message)
                    );
                return errorDoc;
                // error node
                // PeriodName element of forecast should come out as "Not available" -- this is on the tooltip.
            }            
        }

        public List<ForecastDay> GetForecastDays(XDocument doc, bool hasError)
        {
            List<ForecastDay> forecastDays = new List<ForecastDay>();

            if (!hasError)
            {
                var timelayouts = doc
                    .Element("dwml")
                    .Element("data")
                    .Descendants("time-layout");

                XElement timeLayout = null;
                foreach (var currentNode in timelayouts)
                {
                    if (currentNode.Element("layout-key") != null &&
                        (currentNode.Element("layout-key").Value == "k-p12h-n13-1"
                        || currentNode.Element("layout-key").Value == "k-p12h-n14-1")
                        || currentNode.Element("layout-key").Value == "k-p12h-n15-1")
                    {
                        timeLayout = currentNode;
                    }
                }

                if (timeLayout != null)
                {
                    // get the contents of the <time-layout> element where the first child is a layout-key with value "k-p12h-n14-1" or "k-p12h-n13-1"
                    List<XElement> startValidTimes = timeLayout
                       .Descendants("start-valid-time").ToList();

                    // keep track of the position of the current element
                    int position = 0;

                    // the ordinal position of the Saturday element
                    int saturdayPosition = -1;

                    // the ordinal position of the Sunday element
                    int sundayPosition = -1;

                    // enumerate through the times
                    foreach (XElement timeNode in startValidTimes)
                    {
                        // get the element value as a DateTime
                        DateTime dt = DateTime.MinValue;
                        DateTime.TryParse(timeNode.Value.Substring(0, 10) + " " + timeNode.Value.Substring(11, 8), out dt);
                        if (dt != DateTime.MinValue)
                        {
                            // if it's a Saturday or Sunday, investigate further
                            // if it's Saturday
                            if (dt.DayOfWeek == DayOfWeek.Saturday)
                            {
                                // if we don't already have a Saturday time period and a Sunday time period
                                // if we already have a Sunday index, that means today is Sunday and we are only showing today's forecast.
                                if (saturdayPosition == -1 && sundayPosition == -1 && timeNode.Attribute("period-name").Value != "Overnight")
                                {
                                    saturdayPosition = position;
                                    forecastDays.Add(new ForecastDay(dt.ToString("ddd M/dd"), saturdayPosition));
                                }
                            }
                            // if it's Sunday
                            else if (dt.DayOfWeek == DayOfWeek.Sunday && timeNode.Attribute("period-name") != null
                                && timeNode.Attribute("period-name").Value != "Overnight" && timeNode.Attribute("period-name").Value != "Tonight")
                            {
                                // if we don't already have a Sunday time period
                                // if the first item is a Sunday evening, don't count it.
                                // this will have the effect of giving us a Saturday forecast only.
                                if (sundayPosition == -1 && dt.TimeOfDay < TimeSpan.FromHours(18))
                                {
                                    sundayPosition = position;
                                    forecastDays.Add(new ForecastDay(dt.ToString("ddd M/dd"), sundayPosition));
                                    break; // quit since the Sunday forecast is the last one needed
                                }
                            }
                        }
                        if (forecastDays.Count > 1)
                        {
                            break;
                        }
                        position++;
                    }
                }
            }
            else
            {
                // if there is an error, return a single ForecastDay with "N/A" as the text
                forecastDays.Add(new ForecastDay("N/A", 0));
            }
            return forecastDays;
        }

        //public List<Forecast> GetDailyForecast(XDocument doc, int numDays)
        //{
        //    List<Forecast> lst = new List<Forecast>();

        //    // get the data element
        //    XElement dataElement = doc
        //        .Element("dwml")
        //        .Element("data");

        //    // get the <parameters> element that contains the weather information.
        //    XElement parametersElement = dataElement
        //           .Element("parameters");

        //    // get each day's max temperature
        //    var maxTemps = parametersElement
        //        .XPathSelectElement("temperature[@type='maximum']")
        //        .Descendants("value");

        //    // get each day's weather conditions
        //    var weatherConditions = parametersElement
        //        .Element("weather")
        //        .Descendants("weather-conditions");

        //    // get each day's image file
        //    var icons = parametersElement
        //        .Element("conditions-icon")
        //        .Descendants("icon-link");

        //    // find out how many 24-hour weather periods came back, and the start date of each
        //    string layoutKey = numDays == 7 ? "k-p24h-n7-1" : "k-p24h-n1-1";

        //    var startTimes = dataElement
        //        .XPathSelectElements("time-layout[layout-key/text() = '" + layoutKey + "']")
        //        .Descendants("start-valid-time");

        //    int numForecasts = startTimes.Count();

        //    // for all cases except when it is Sunday after 14:00
        //    if (numDays != 7)
        //    {
        //        for (int i = 0; i < numForecasts; i++)
        //        {
        //            Forecast forecast = new Forecast();

        //            //forecast.StartDate = DateTime.Parse(startTimes.ElementAt(i).Value.Substring(0, 10));

        //            //forecast.MinTemperature = Convert.ToInt32(minTemps.ElementAt(i).Value);
        //            if (weatherConditions.ElementAt(i).Attribute("weather-summary") != null)
        //            {
        //                forecast.WeatherSummary = weatherConditions.ElementAt(i).Attribute("weather-summary").Value;
        //            }

        //            if (icons.ElementAt(i) != null)
        //            {
        //                forecast.ImageFile = "../../Content/images/NoaaIcons/" +
        //                    icons.ElementAt(i).Value.Substring(icons.ElementAt(i).Value.LastIndexOf('/') + 1);
        //            }

        //            lst.Add(forecast);
        //        }
        //    }
        //    // for Sundays after 14:00, we expect 7 days of weather data, but only use the last day.
        //    else
        //    {
        //        Forecast forecast = new Forecast();

        //        //forecast.StartDate = DateTime.Parse(startTimes.Last().Value.Substring(0, 10));

        //        //forecast.MinTemperature = Convert.ToInt32(minTemps.ElementAt(i).Value);
        //        if (weatherConditions.Last().Attribute("weather-summary") != null)
        //        {
        //            forecast.WeatherSummary = weatherConditions.Last().Attribute("weather-summary").Value;
        //        }

        //        if (icons.Last() != null)
        //        {
        //            forecast.ImageFile = "../../Content/images/NoaaIcons/" +
        //                icons.Last().Value.Substring(icons.Last().Value.LastIndexOf('/') + 1);
        //        }

        //        lst.Add(forecast);
        //    }

        //    return lst;
        //}

        //public List<DateTime> GetForecastStartDates(XDocument doc)
        //{
        //    List<DateTime> startDates = new List<DateTime>();

        //    // find out how many 24-hour weather periods came back, and the start date of each
        //    var startTimes = doc
        //        .Element("dwml")
        //        .Element("data")
        //        .XPathSelectElements("time-layout[layout-key/text() = 'k-p24h-n1-1']")
        //        .Descendants("start-valid-time");

        //    foreach (XElement elem in startTimes)
        //    {
        //        DateTime dt = DateTime.MinValue;
        //        DateTime.TryParse(elem.Value.Substring(0, 10), out dt);
        //        startDates.Add(dt);
        //    }

        //    return startDates;
        //}
    }

    public class ForecastDay
    {
        public string FormattedDate { get; set; }
        public int PositionInList { get; set; }

        public ForecastDay() { }

        public ForecastDay(string formattedDate, int positionInList)
        {
            FormattedDate = formattedDate;
            PositionInList = positionInList;
        }
    }
}