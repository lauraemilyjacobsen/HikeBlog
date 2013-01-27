using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Text;

namespace HikeBlog.Models
{
    public class Post
    {
        [Key]
        public int ID { get; set; }

        public string Title { get; set; }

        public string Slug
        {
            get { return this.slug; }
            set
            {
                value = (Title ?? "").Trim().ToLower();

                StringBuilder url = new StringBuilder();

                foreach (char ch in value)
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
                value = url.ToString();
                this.slug = value;
            }
        }
        private string slug = String.Empty;

        private string snip = "<!--snip-->";

        [DataType(DataType.MultilineText)]
        [AllowHtml]
        public string Content
        { get; set; }

        [NotMapped]
        public string TruncatedContent
        {
            get
            {
                if (this.Content == null || this.Content.IndexOf(snip) < 0)
                {
                    return this.Content;
                }
                else
                {
                    return this.Content.Substring(0, this.Content.IndexOf(snip));
                }
            }
        }

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

        public string FlickrPhotosetID { get; set; }

        public bool IsPublished
        {
            get { return this.isPublished; }
            set
            {
                bool wasPublished = this.isPublished;
                this.isPublished = value;
                if (this.isPublished && !wasPublished)
                {
                    this.DatePublished = DateTime.Now;
                }
            }
        }
        private bool isPublished;

        [DisplayFormat(DataFormatString = "{0:d}")]
        public DateTime? DatePublished { get; set; }

        public virtual ICollection<Comment> Comments { get; set; }

        //public virtual FlickrPhotoset Photoset { get; set; }
    }
}