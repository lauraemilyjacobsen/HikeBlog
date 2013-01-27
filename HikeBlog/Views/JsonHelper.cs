using System;
using System.Web.Script.Serialization;
using System.Web;

namespace HikeBlog.Views
{
    public static class JsonHelper
    {
        public static HtmlString ToJSON(this object obj)
        {
            JavaScriptSerializer serializer = new JavaScriptSerializer();
            HtmlString serialized = new HtmlString(serializer.Serialize(obj));
            return serialized;
        }
    }
}