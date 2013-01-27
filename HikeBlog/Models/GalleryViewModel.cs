using FlickrNet;
using PagedList;

namespace HikeBlog.Models
{
    public class GalleryViewModel
    {
        public Photoset Photoset { get; set; }
        public IPagedList<FlickrNet.Photo> Photos { get; set; }
        public int PostID { get; set; }
        public string PhotosetID { get; set; }

        public GalleryViewModel(Photoset set, IPagedList<FlickrNet.Photo> collection, int post, string id)
        {
            this.Photoset = set;
            this.Photos = collection;
            this.PostID = post;
            this.PhotosetID = id;
        }
    }
}