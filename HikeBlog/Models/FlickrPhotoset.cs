using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;

namespace HikeBlog.Models
{
    public class FlickrPhotoset
    {
        [Key]
        public int ID { get; set; }

        public string FlickrID { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime DateCreated
        {
            // default to DateTime.Now
            get
            {
                return (this.dateCreated == default(DateTime))
                   ? DateTime.Now
                   : this.dateCreated;
            }

            set { this.dateCreated = value; }
        }
        private DateTime dateCreated = default(DateTime);

        public virtual Post Post { get; set; }
    }
}