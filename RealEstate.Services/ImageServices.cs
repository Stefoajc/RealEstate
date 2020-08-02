using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Security.Principal;
using System.Threading.Tasks;
using System.Web;
using Microsoft.AspNet.Identity;
using Ninject;
using RealEstate.Model;
using RealEstate.Repositories.Interfaces;
using RealEstate.Services.Helpers;
using RealEstate.ViewModels.WebMVC;

namespace RealEstate.Services
{
    public class ImageServices : BaseService
    {

        [Inject]
        public ImageServices(IUnitOfWork unitOfWork, ApplicationUserManager userMgr) 
            : base(unitOfWork, userMgr){}

        /// <summary>
        /// Factory for creating Images
        /// </summary>
        /// <param name="imagesToAdd">images model to be added</param>
        /// <param name="imageType">PropertyImages/SightImages/UserImages/CityImages</param>
        public ISet<Images> CreateImagesFactory(ImageCreateViewModel imagesToAdd, string imageType, string userName)
        {            
            string callerName;
            switch (imageType) // Caller Determining
            {
                case "PropertyImages":
                    int propertyId = int.Parse(imagesToAdd.ForeignKey);
                    callerName = unitOfWork.PropertiesRepository
                        .Where(p => p.Id == propertyId).Select(p => p.Owner.UserName)
                        .FirstOrDefault();
                    //If property not found
                    if (string.IsNullOrEmpty(callerName)) throw new ArgumentException("No such Property");
                    break;
                case "SightImages":
                    int sightId = int.Parse(imagesToAdd.ForeignKey);
                    callerName = unitOfWork.SightsRepository
                        .Where(p => p.SightId == sightId).Select(p => p.SightName)
                        .FirstOrDefault();
                    //If property not found
                    if (string.IsNullOrEmpty(callerName)) throw new ArgumentException("No such Sight");
                    break;
                case "UserImages":
                    callerName = userName;
                    //If property not found
                    if (string.IsNullOrEmpty(callerName)) throw new ArgumentException("No such user");
                    break;
                case "CityImages":
                    int cityId = int.Parse(imagesToAdd.ForeignKey);
                    callerName = unitOfWork.CitiesRepository
                        .Where(p => p.CityId == cityId).Select(p => p.CityName)
                        .FirstOrDefault();
                    //If property not found
                    if (string.IsNullOrEmpty(callerName)) throw new ArgumentException("No such city");
                    break;
                default:
                    throw new ArgumentException("No such image type");
            }

            ISet<Images> images = new HashSet<Images>();
            foreach (HttpPostedFileBase imageToAdd in imagesToAdd.ImageFiles)
            {
                Images image = null;
                switch (imageType)
                {
                    case "PropertyImages":
                        image = new PropertyImages
                        {
                            ImagePath = PathManager.CreateUserPropertyImagePath(callerName, imageToAdd.FileName),
                            PropertyId = int.Parse(imagesToAdd.ForeignKey)
                        };
                        break;
                    case "SightImages":
                        image = new SightImages
                        {
                            ImagePath = PathManager.CreateSightImagePath(callerName, imageToAdd.FileName),
                            SightId = int.Parse(imagesToAdd.ForeignKey)
                        };
                        break;
                    case "UserImages":
                        image = new UserImages
                        {
                            ImagePath = PathManager.CreateUserProfileImagePath(callerName, imageToAdd.FileName),
                            UserId = (string)imagesToAdd.ForeignKey
                        };
                        break;
                    case "CityImages":
                        image = new CityImages
                        {
                            ImagePath = PathManager.CreateCityImagePath(callerName, imageToAdd.FileName),
                            CityId = int.Parse(imagesToAdd.ForeignKey)
                        };
                        break;
                    default:
                        throw new ArgumentException("No such image type");
                }
                image.ImageType = imageToAdd.ContentType;

                var physicalPath = Path.Combine(HttpRuntime.AppDomainAppPath.TrimEnd('\\'), image.ImagePath.TrimStart('\\'));
                var physicalDirPath = Path.GetDirectoryName(physicalPath);
                if (!Directory.Exists(physicalDirPath))
                {
                    DirectoryHelpers.CreateDirectory(physicalDirPath);
                }
                imageToAdd.SaveAs(physicalPath);

                //Add Image to FileSystem
                Image img = Image.FromStream(imageToAdd.InputStream);
                image.ImageRatio = (float)img.Width / img.Height;

                //Adding image to DataBase
                unitOfWork.ImagesRepository.Add(image);
                unitOfWork.Save();

                //Add Image to returned set
                images.Add(image);
            }

            return images;
        }


        public async Task<List<Images>> CreateUserImages(List<HttpPostedFileBase> images, string userId)
        {
            string callerName = await userManager.Users
                .Where(u => u.Id == userId)
                .Select(u => u.UserName)
                .FirstOrDefaultAsync()
                ?? throw new ArgumentException("Не е намерен потребителят.");


            float GetSizeRatio(HttpPostedFileBase imageFile)
            {
                using (Image img = Image.FromStream(imageFile.InputStream))
                {
                    return (float)img.Width / img.Height;
                }
            }

            var imagesToAdd = images
                .Select(i => (Images)new UserImages
                {
                    ImagePath = PathManager.CreateUserProfileImagePath(callerName, i.FileName),
                    UserId = userId,
                    ImageRatio = GetSizeRatio(i),
                    ImageType = i.ContentType
                })
                .ToList();

            unitOfWork.ImagesRepository.AddRange(imagesToAdd);
            await unitOfWork.SaveAsync();

            //Add images to FileSystem
            foreach (HttpPostedFileBase imageToAdd in images)
            {
                var physicalPath = Path.Combine(HttpRuntime.AppDomainAppPath.TrimEnd('\\'), PathManager.CreateUserProfileImagePath(callerName, imageToAdd.FileName).TrimStart('\\'));
                var physicalDirPath = Path.GetDirectoryName(physicalPath);
                if (!Directory.Exists(physicalDirPath))
                {
                    DirectoryHelpers.CreateDirectory(physicalDirPath);
                }
                imageToAdd.SaveAs(physicalPath);

                using (Image img = Image.FromStream(imageToAdd.InputStream))
                {
                    ImageHelpers.SaveAsWebP(img, physicalPath, img.Width, img.Height);
                }
            }

            return imagesToAdd;
        }

        /// <summary>
        /// Method for adding images into Property (house,hotel..)
        /// Directly into its navigation property
        /// </summary>
        /// <param name="imagesToAdd"></param>
        /// <param name="isForSlider">determines the image type to resize to</param>
        public IList<PropertyImages> CreatePropertyImagesSet(IEnumerable<HttpPostedFileBase> imagesToAdd, string userName, bool isForSlider = false)
        {
            var propertyImages = imagesToAdd as HttpPostedFileBase[] ?? imagesToAdd.ToArray();

            var imagesForFileSystem = propertyImages
                .Select(i => new ImageFileSystemDTO
                {
                    ImageRelPath = PathManager.CreateUserPropertyImagePath(userName, i.FileName, isForSlider: isForSlider),
                    ImageFile = i
                })
                .ToList();

            SaveToFileSystem(imagesForFileSystem, isForSlider);

            var images = propertyImages
                .Select(p => new PropertyImages
                {
                    ImagePath = PathManager.CreateUserPropertyImagePath(userName, p.FileName),
                    ImageType = p.ContentType,
                    ImageRatio = isForSlider ? (float)2200 / 800 : (float)870 / 580,
                })
                .ToList();

            return images;
        }

        public void SaveToFileSystem(List<ImageFileSystemDTO> images, bool isForSlider)
        {
            var physicalDirPath = Path.GetDirectoryName(Path.Combine(HttpRuntime.AppDomainAppPath.TrimEnd('\\'), images[0].ImageRelPath.TrimStart('\\')));
            DirectoryHelpers.CreateDirectory(physicalDirPath);

            foreach (var image in images)
            {
                var physicalImagePath =
                    Path.Combine(HttpRuntime.AppDomainAppPath.TrimEnd('\\'), image.ImageRelPath.TrimStart('\\'));
                using (Image img = Image.FromStream(image.ImageFile.InputStream))
                {
                    //Resize image for usage in the site
                    var width = isForSlider ? 2200 : 870;
                    var height = isForSlider ? 800 : 580;

                    //SaveOriginal Image
                    ImageHelpers.SaveOriginal(img, physicalImagePath);
                    //Save image in webP format
                    ImageHelpers.SaveAsWebP(img, physicalImagePath, width, height);
                    //Resize the image and Save It
                    ImageHelpers.SaveToFileSystem(img, physicalImagePath, height, width, ImageFormat.Jpeg);
                }
            }
        }


        /// <summary>
        /// Delete images from db and fileSystem
        /// </summary>
        /// <param name="id"></param>
        /// <param name="userId"></param>
        public async Task DeleteImage(int id, string userId)
        {
            var imageToDelete = await unitOfWork.ImagesRepository
                .Where(i => i.ImageId == id)
                .FirstOrDefaultAsync() 
                ?? throw new ArgumentNullException();

            switch (imageToDelete.GetType().Name)
            {
                case "UserImages":
                    IsImageOfCurrentUser(imageToDelete, userId);
                    break;
                case "PropertyImages":
                    IsCurrentUserOwnerOfTheProperty(imageToDelete, userId);
                    break;
                case "SightImages":
                case "CityImages":
                    await IsCurrentUserMaintanance(userId);
                    break;
            }
            //Delete file from File system
            var physicalPath = Path.Combine(HttpRuntime.AppDomainAppPath.TrimEnd('\\'), imageToDelete.ImagePath.TrimStart('\\'));
            File.Delete(physicalPath);
            //Delete file from Database
            unitOfWork.ImagesRepository.Delete(imageToDelete);
            await unitOfWork.SaveAsync();
        }

        #region DeleteImage Helpers

        private void IsImageOfCurrentUser(Images imageToDelete, string userId)
        {
            if (((UserImages)imageToDelete).UserId != userId)
            {
                throw new AccessViolationException();
            }
        }

        private async Task IsCurrentUserMaintanance(string userId)
        {
            if (!await userManager.IsInRoleAsync(userId, "Maintanance"))
            {
                throw new AccessViolationException();
            }
        }

        private void IsCurrentUserOwnerOfTheProperty(Images imageToDelete, string userId)
        {
            //Check if the logged user is one of the owners of the properties then he is allowed to delete the image
            var property = unitOfWork.PropertiesRepository
                               .Where(p => p.Id == ((PropertyImages)imageToDelete).PropertyId)
                               .FirstOrDefault() ?? throw new ArgumentNullException();

            if (property.Owner.Id == userId)
            {
                throw new AccessViolationException();
            }
        }

        #endregion

    }
}
