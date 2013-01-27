using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FlickrNet;
using HikeBlog.Models;
using PagedList;

namespace HikeBlog.Controllers
{
    public class PhotoController : Controller
    {
        string flickrKey = "861472f32b49bd11cdb752ab7c81cc37";
        string sharedSecret = "124aa8def86b4d51";
        string userID = "65476859@N08";

        //
        // GET: /Photo/

        public ActionResult Set(string photosetID)
        {
            Flickr flickr = new Flickr(flickrKey, sharedSecret);
            PhotosetPhotoCollection photos = GetPhotosetPhotos(photosetID, flickr);

            return View(photos);
        }

        public ActionResult Gallery(string id, int postID, int? page)
        {
            IPagedList<Photo> photos = null;
            Photoset set = new Photoset();

            int pageNumber = (page ?? 1);

            if (id != null)
            {
                Flickr flickr = new Flickr(flickrKey, sharedSecret);
                photos = GetPhotosetPhotos(id, flickr).ToPagedList<Photo>(pageNumber, 30);
                set = flickr.PhotosetsGetInfo(id);
            }

            GalleryViewModel vm = new GalleryViewModel(set, photos, postID, id);
            return View(vm);
        }

        private PhotosetPhotoCollection GetPhotosetPhotos(string photosetID, Flickr flickr, int page, int perPage)
        {
            PhotosetPhotoCollection photos = new PhotosetPhotoCollection();

            PhotoSearchOptions options = GetPhotoSearchOptions();

            Flickr.CacheDisabled = true;

            photos = flickr.PhotosetsGetPhotos(photosetID, page, perPage);

            return photos;
        }

        private PhotosetPhotoCollection GetPhotosetPhotos(string photosetID, Flickr flickr)
        {
            PhotosetPhotoCollection photos = new PhotosetPhotoCollection();

            PhotoSearchOptions options = GetPhotoSearchOptions();

            Flickr.CacheDisabled = true;

            photos = flickr.PhotosetsGetPhotos(photosetID);

            return photos;
        }

        private PhotoSearchOptions GetPhotoSearchOptions()
        {
            PhotoSearchOptions options = new PhotoSearchOptions();
            options.PerPage = 12;
            options.SortOrder = PhotoSearchSortOrder.DatePostedDescending;
            options.MediaType = MediaType.Photos;
            options.Extras = PhotoSearchExtras.All;
            options.UserId = userID;

            return options;
        }
    }
}
