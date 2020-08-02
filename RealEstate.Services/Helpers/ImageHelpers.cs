using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;
using ImageProcessor;
using ImageProcessor.Imaging;
using ImageProcessor.Imaging.Formats;
using ImageProcessor.Plugins.WebP.Imaging.Formats;

namespace RealEstate.Services.Helpers
{
    public static class ImageHelpers
    {
        public static string ImageFileToBase64(string imagePath)
        {
            using (Image image = Image.FromFile(imagePath))
            {
                using (MemoryStream m = new MemoryStream())
                {
                    image.Save(m, image.RawFormat);
                    byte[] imageBytes = m.ToArray();

                    // Convert byte[] to Base64 String
                    string base64String = Convert.ToBase64String(imageBytes);
                    return base64String;
                }
            }
        }

        public static void ToWebP(string inputFile, string outputFile)
        {
            byte[] photoBytes = File.ReadAllBytes(inputFile);
            // Format is automatically detected though can be changed.
            ISupportedImageFormat format = new WebPFormat();
            using (MemoryStream inStream = new MemoryStream(photoBytes))
            {
                using (FileStream outStream = new FileStream(outputFile, FileMode.Create))
                {
                    // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                    using (ImageFactory imageFactory = new ImageFactory(true))
                    {
                        // Load, resize, set the format and quality and save an image.
                        imageFactory.Load(inStream)
                            .Format(format)
                            .Save(outStream);
                    }
                    // Do something with the stream.
                }
            }
        }

        public static void ToWebP(HttpPostedFileBase inputImage, string outputImage, Size? size = null)
        {
            if (inputImage == null) throw new ArgumentNullException();

            using (Image img = Image.FromStream(inputImage.InputStream))
            {
                ToWebP(img, outputImage, size);
            }
        }
        public static void ToWebP(Image inputImage, string outputImage, Size? size = null)
        {
            if (inputImage == null)
            {
                return;
            }

            size = size ?? new Size(inputImage.Width, inputImage.Height);

            // Format is automatically detected though can be changed.
            ISupportedImageFormat format = new WebPFormat();
            using (FileStream outStream = new FileStream(outputImage, FileMode.Create))
            {
                // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                using (ImageFactory imageFactory = new ImageFactory(true))
                {
                    var resizeLayer = new ResizeLayer((Size) size, ResizeMode.Stretch);

                    // Load, resize, set the format and quality and save an image.
                    imageFactory.Load(inputImage)
                        .Format(format)
                        .Resize(resizeLayer)
                        .Save(outStream);
                }
                // Do something with the stream.
            }
        }


        public static void SaveImage(HttpPostedFileBase inputImage, string outputImage, Size? size = null,
            int qualityInPercentage = 100)
        {
            if (inputImage == null) throw new ArgumentNullException();

            using (Image img = Image.FromStream(inputImage.InputStream))
            {
                SaveImage(img, outputImage, size);
            }
        }

        public static void SaveImage(Image inputImage, string outputImage, Size? size = null, int qualityInPercentage = 100)
        {
            if (inputImage == null)
            {
                return;
            }

            qualityInPercentage = qualityInPercentage < 1 || qualityInPercentage > 100 ? 100 : qualityInPercentage;
            size = size ?? new Size(inputImage.Width, inputImage.Height);

            using (FileStream outStream = new FileStream(outputImage, FileMode.Create))
            {
                // Initialize the ImageFactory using the overload to preserve EXIF metadata.
                using (ImageFactory imageFactory = new ImageFactory(true))
                {
                    // Load, resize, set the format and quality and save an image.
                    imageFactory.Load(inputImage)
                        .Quality(qualityInPercentage)
                        .Resize((Size)size)
                        .Save(outStream);
                }
                // Do something with the stream.
            }
        }

        public static void SaveToFileSystem(Image photo, string fileNameOutput, double resizeHeight,
            double resizeWidth, ImageFormat outputFormat)
        {
            double aspectRatio = (double)photo.Width / photo.Height;
            double boxRatio = resizeWidth / resizeHeight;
            double scaleFactor = 0;

            if (photo.Width < resizeWidth && photo.Height < resizeHeight)
            {
                // keep the image the same size since it is already smaller than our max width/height
                scaleFactor = 1.0;
            }
            else
            {
                if (boxRatio > aspectRatio)
                    scaleFactor = resizeHeight / photo.Height;
                else
                    scaleFactor = resizeWidth / photo.Width;
            }

            int newWidth = (int)(photo.Width * scaleFactor);
            int newHeight = (int)(photo.Height * scaleFactor);

            using (Bitmap bmp = new Bitmap(newWidth, newHeight))
            {
                using (Graphics g = Graphics.FromImage(bmp))
                {
                    g.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    g.SmoothingMode = SmoothingMode.HighQuality;
                    g.CompositingQuality = CompositingQuality.HighQuality;
                    g.PixelOffsetMode = PixelOffsetMode.HighQuality;

                    g.DrawImage(photo, 0, 0, newWidth, newHeight);
                }

                if (ImageFormat.Png.Equals(outputFormat))
                {
                    bmp.Save(fileNameOutput, outputFormat);
                }
                else if (ImageFormat.Jpeg.Equals(outputFormat))
                {
                    ImageCodecInfo[] info = ImageCodecInfo.GetImageEncoders();
                    EncoderParameters encoderParameters;
                    using (encoderParameters = new EncoderParameters(1))
                    {
                        // use jpeg info[1] and set quality to 90
                        encoderParameters.Param[0] =
                            new EncoderParameter(Encoder.Quality, 90L);
                        bmp.Save(fileNameOutput, info[1], encoderParameters);
                    }
                }
            }
        }


        public static void SaveOriginal(HttpPostedFileBase inputImage, string originalImageFullPath)
        {
            if (inputImage == null) throw new ArgumentNullException();

            using (Image img = Image.FromStream(inputImage.InputStream))
            {
                SaveOriginal(img, originalImageFullPath);
            }
        }
        public static void SaveOriginal(Image photo, string originalImageFullPath)
        {
            var fileExtention = Path.GetExtension(originalImageFullPath);
            var directoryName = Path.GetDirectoryName(originalImageFullPath);
            var fileName = Path.GetFileNameWithoutExtension(originalImageFullPath);
            var originalPostfix = "_Original";

            var finalFileName = Path.Combine(directoryName ?? "", fileName + originalPostfix + fileExtention);

            photo.Save(finalFileName);
        }


        public static void SaveAsWebP(HttpPostedFileBase inputImage, string originalImageFullPath, int? width = null, int? height = null)
        {
            if (inputImage == null) throw new ArgumentNullException();

            using (Image img = Image.FromStream(inputImage.InputStream))
            {
                SaveAsWebP(img, originalImageFullPath, width, height);
            }
        }
        public static void SaveAsWebP(Image photo, string originalImageFullPath, int? width, int? height)
        {
            var webPFileExtention = ".webp";
            var directoryName = Path.GetDirectoryName(originalImageFullPath);
            var fileName = Path.GetFileNameWithoutExtension(originalImageFullPath);
            var newFileNameOutput = Path.Combine(directoryName ?? "", fileName + webPFileExtention);

            Size size = width != null && height != null ? new Size((int)width, (int)height) : new Size(photo.Width,photo.Height); 
            ImageHelpers.ToWebP(photo, newFileNameOutput, size);
        }
    }
}