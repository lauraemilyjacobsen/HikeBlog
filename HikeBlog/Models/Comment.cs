using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace HikeBlog.Models
{
    public class Comment
    {
        [Key]
        public int ID { get; set; }

        public int PostID { get; set; }

        [Display(Name="Your Name")]
        [Required(ErrorMessage="Please enter your name.")]
        public string Name { get; set; }

        [DataType(DataType.MultilineText)]
        [Display(Name = "Comment")]
        [Required(ErrorMessage = "Please enter a comment.")]
        public string Content { get; set; }

        [Required(AllowEmptyStrings=false, ErrorMessage="Please enter your email address.")]
        [DataType(DataType.EmailAddress)]
        [Display(Name="Your Email (not displayed)")]
        public string EmailAddress { get; set; }

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