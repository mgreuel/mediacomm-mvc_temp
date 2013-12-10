using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ImageRotate
{
    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Imaging;
    using System.IO;


    public class ImageGenerator
    {
        private const long JpegQuality = 80;

        private const float MaxLargeHeight = 1067;
       
        private const float MaxLargeWidth = 1600;

        private const float MaxMediumHeight = 534;

        private const float MaxMediumWidth = 800;

        private const float MaxThumbnailHeight = 160;

        private const float MaxThumbnailWidth = 160;

        private static readonly ImageCodecInfo JpegEncoder = ImageCodecInfo.GetImageDecoders().SingleOrDefault(c => c.FormatID == ImageFormat.Jpeg.Guid);

        public void GenerateImages(FileInfo sourceImage)
        {
            using (Image originalImage = Image.FromFile(sourceImage.FullName))
            {
                using (Image resizedImage = this.GetResizedImage(originalImage, MaxThumbnailWidth, MaxThumbnailHeight))
                {
                    string targetFilename = string.Format(
                        "{0}small{1}", sourceImage.Name.Replace(sourceImage.Extension, string.Empty), sourceImage.Extension);

                    string targetFilePath = Path.Combine(sourceImage.DirectoryName, targetFilename);
                    this.SaveResizedImage(resizedImage, targetFilePath);
                }

                using (Image resizedImage = this.GetResizedImage(originalImage, MaxMediumWidth, MaxMediumHeight))
                {
                    string targetFilename = string.Format(
                        "{0}medium{1}", sourceImage.Name.Replace(sourceImage.Extension, string.Empty), sourceImage.Extension);

                    string targetFilePath = Path.Combine(sourceImage.DirectoryName, targetFilename);
                    this.SaveResizedImage(resizedImage, targetFilePath);
                }

                using (Image resizedImage = this.GetResizedImage(originalImage, MaxLargeWidth, MaxLargeHeight))
                {
                    string targetFilename = string.Format(
                        "{0}large{1}", sourceImage.Name.Replace(sourceImage.Extension, string.Empty), sourceImage.Extension);

                    string targetFilePath = Path.Combine(sourceImage.DirectoryName, targetFilename);
                    this.SaveResizedImage(resizedImage, targetFilePath);
                }
            }
        }

        private Image GetResizedImage(Image originalImage, float maxWidth, float maxHeight)
        {
            if (ExifRotation.ImageNeedsRotation(originalImage))
            {
                ExifRotation.ExifOrientations imageRotation = ExifRotation.DetermineImageRotation(originalImage);
                ExifRotation.RotateImageUsingExifOrientation(originalImage, imageRotation);
            }

            float originalHeight = Convert.ToSingle(originalImage.Height);
            float originalWidth = Convert.ToSingle(originalImage.Width);

            float scale = Math.Max(originalHeight / maxHeight, originalWidth / maxWidth);
            int height = Convert.ToInt32(originalHeight / scale);
            int width = Convert.ToInt32(originalWidth / scale);

            Image resizedImage = new Bitmap(width, height);
            using (Graphics graphics = Graphics.FromImage(resizedImage))
            {
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                graphics.DrawImage(originalImage, 0, 0, width, height);
            }

            return resizedImage;
        }

        private void SaveResizedImage(Image resizedImage, string targetFilePath)
        {
            using (EncoderParameters encoderParameters = new EncoderParameters(1))
            {
                using (EncoderParameter qualityEncoderParameter = new EncoderParameter(Encoder.Quality, JpegQuality))
                {
                    encoderParameters.Param[0] = qualityEncoderParameter;
                    resizedImage.Save(targetFilePath, JpegEncoder, encoderParameters);
                }
            }
        }
    }
}
