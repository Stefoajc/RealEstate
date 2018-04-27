using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Web;
using Microsoft.AspNet.Identity;
using Ninject;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public class ImageServices : BaseService
    {

        [Inject]
        public ImageServices(IUnitOfWork unitOfWork, IPrincipal user, ApplicationUserManager userMgr) : base(unitOfWork, user, userMgr)
        {
        }

        /// <summary>
        /// Get image by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public string GetImage(int id)
        {
            return UnitOfWork.ImagesRepository.FindBy(i => i.ImageId == id).Select(i => i.ImagePath).FirstOrDefault();
        }

        /// <summary>
        /// Get all images path, based on their path
        /// </summary>
        /// <param name="type">Type of the images</param>
        /// <returns></returns>
        public List<string> GetImages(Type type)
        {
            var queryImages = UnitOfWork.ImagesRepository.GetAll();
            switch (type.Name)
            {
                case "PropertyImages":
                    queryImages = queryImages.Where(i => i is PropertyImages);
                    break;
                case "SightImages":
                    queryImages = queryImages.Where(i => i is SightImages);
                    break;
                case "UserImages":
                    queryImages = queryImages.Where(i => i is UserImages);
                    break;
                case "CityImages":
                    queryImages = queryImages.Where(i => i is CityImages);
                    break;
                case "Images":
                    // do not filter by anything, this will get all images
                    break;
                default:
                    throw new ArgumentException("No such image type");
            }

            return queryImages.Select(i => i.ImagePath).ToList();
        }

        /// <summary>
        /// Factory for creating Images
        /// </summary>
        /// <param name="imagesToAdd">images model to be added</param>
        /// <param name="imageType">PropertyImages/SightImages/UserImages/CityImages</param>
        public ISet<Images> CreateImagesFactory(IEnumerable<ImageCreateViewModel> imagesToAdd, string imageType)
        {
            ISet<Images> images = new HashSet<Images>();
            foreach (ImageCreateViewModel imageToAdd in imagesToAdd)
            {
                Images image;
                string callerName;
                switch (imageType)
                {
                    case "PropertyImages":
                        callerName = UnitOfWork.PropertiesRepository
                            .FindBy(p => p.PropertyId == (int)imageToAdd.ForeignKey).Select(p => p.PropertyName)
                            .FirstOrDefault();
                        //If property not found
                        if (string.IsNullOrEmpty(callerName)) throw new ArgumentException("No such Property");

                        image = new PropertyImages
                        {
                            ImageType = imageToAdd.ImageFile.ContentType,
                            ImagePath = PathManager.CreateUserPropertyImagePath(callerName, imageToAdd.ImageFile.FileName),
                            PropertyId = (int)imageToAdd.ForeignKey
                        };
                        break;
                    case "SightImages":
                        callerName = UnitOfWork.SightsRepository
                            .FindBy(p => p.SightId == (int)imageToAdd.ForeignKey).Select(p => p.SightName)
                            .FirstOrDefault();
                        //If property not found
                        if (string.IsNullOrEmpty(callerName)) throw new ArgumentException("No such Sight");

                        image = new SightImages()
                        {
                            ImageType = imageToAdd.ImageFile.ContentType,
                            ImagePath = PathManager.CreateSightImagePath(callerName, imageToAdd.ImageFile.FileName),
                            SightId = (int)imageToAdd.ForeignKey
                        };
                        break;
                    case "UserImages":
                        callerName = User.Identity.GetUserName();
                        //If property not found
                        if (string.IsNullOrEmpty(callerName)) throw new ArgumentException("No such user");

                        image = new UserImages()
                        {
                            ImageType = imageToAdd.ImageFile.ContentType,
                            ImagePath = PathManager.CreateUserProfileImagePath(callerName, imageToAdd.ImageFile.FileName),
                            UserId = (string)imageToAdd.ForeignKey
                        };
                        break;
                    case "CityImages":
                        callerName = UnitOfWork.CitiesRepository
                            .FindBy(p => p.CityId == (int)imageToAdd.ForeignKey).Select(p => p.CityName)
                            .FirstOrDefault();
                        //If property not found
                        if (string.IsNullOrEmpty(callerName)) throw new ArgumentException("No such city");

                        image = new CityImages()
                        {
                            ImageType = imageToAdd.ImageFile.ContentType,
                            ImagePath = PathManager.CreateCityImagePath(callerName, imageToAdd.ImageFile.FileName),
                            CityId = (int)imageToAdd.ForeignKey
                        };
                        break;
                    default:
                        throw new ArgumentException("No such image type");
                }

                var physicalPath = Path.Combine(HttpRuntime.AppDomainAppPath.TrimEnd('\\'), image.ImagePath.TrimStart('\\'));
                imageToAdd.ImageFile.SaveAs(physicalPath);

                Image img = Image.FromStream(imageToAdd.ImageFile.InputStream);
                image.ImageRatio = (float)img.Width / img.Height;
                images.Add(image);
            }

            return images;
        }


        /// <summary>
        /// Method for adding images into Property (house,hotel..)
        /// Directly into its navigation property
        /// </summary>
        /// <param name="imagesToAdd"></param>
        /// <param name="isForSlider">determines the image type to resize to</param>
        public ISet<PropertyImages> CreatePropertyImages(IEnumerable<HttpPostedFileBase> imagesToAdd, bool isForSlider = false)
        {
            ISet<PropertyImages> images = new HashSet<PropertyImages>();
            foreach (var imageToAdd in imagesToAdd)
            {
                if (imageToAdd == null)
                {
                    continue;
                }
                var propertyImage = new PropertyImages()
                {
                    ImagePath = PathManager.CreateUserPropertyImagePath(User.Identity.Name, imageToAdd.FileName),
                    ImageType = imageToAdd.ContentType,
                };


                Image img = Image.FromStream(imageToAdd.InputStream);
                Image imgResized = isForSlider ? ResizeImage(img, new Size(2200, 800)) : ResizeImage(img, new Size(870, 580));

                propertyImage.ImageRatio = (float)img.Width / img.Height;

                var physicalPath = Path.Combine(HttpRuntime.AppDomainAppPath.TrimEnd('\\'), propertyImage.ImagePath.TrimStart('\\'));
                imgResized.Save(physicalPath,ImageFormat.Bmp);
                images.Add(propertyImage);
            }

            return images;
        }



        /// <summary>
        /// Delete images from db and fileSystem
        /// </summary>
        /// <param name="id"></param>
        public void DeleteImage(int id)
        {
            var imageToDelete = UnitOfWork.ImagesRepository.FindBy(i => i.ImageId == id).FirstOrDefault() ?? throw new ArgumentNullException();
            switch (imageToDelete.GetType().Name)
            {
                case "UserImages":
                    IsImageOfCurrentUser(imageToDelete);
                    break;
                case "PropertyImages":
                    IsCurrentUserOwnerOfTheProperty(imageToDelete);
                    break;
                case "SightImages":
                    IsCurrentUserMaintanance();
                    break;
                case "CityImages":
                    IsCurrentUserMaintanance();
                    break;
            }
            //Delete file from File system
            File.Delete(imageToDelete.ImagePath);
            //Delete file from Database
            UnitOfWork.ImagesRepository.Delete(imageToDelete);
            UnitOfWork.Save();
        }

        #region DeleteImage Helpers

        private void IsImageOfCurrentUser(Images imageToDelete)
        {
            if (((UserImages)imageToDelete).UserId != User.Identity.GetUserId())
            {
                throw new AccessViolationException();
            }
        }

        private void IsCurrentUserMaintanance()
        {
            if (!User.IsInRole("Maintanance"))
            {
                throw new AccessViolationException();
            }
        }

        private void IsCurrentUserOwnerOfTheProperty(Images imageToDelete)
        {
            //Check if the logged user is one of the owners of the properties then he is allowed to delete the image
            var property = UnitOfWork.PropertiesRepository
                               .FindBy(p => p.PropertyId == ((PropertyImages)imageToDelete).PropertyId)
                               .FirstOrDefault() ?? throw new ArgumentNullException();
            var currUserId = User.Identity.GetUserId();
            if (property.Owner.Id == currUserId)
            {
                throw new AccessViolationException();
            }
        }

        #endregion

        #region Helpers

        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        static Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(image, width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }


        static Image ResizeImage(Image imgToResize, Size size)
        {
            return (Image) (new Bitmap(imgToResize, size));
        }
        #endregion
    }
}
