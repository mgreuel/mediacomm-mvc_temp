using System;
using System.Drawing;
using System.Drawing.Imaging;

namespace ImageRotate
{
    /// adapted from http://blog.csharphelper.com/2011/08/27/read-an-image-files-exif-orientation-data-in-c.aspx
    public static class ExifRotation
    {
        private const int OrientationId = 0x0112;

        public enum ExifOrientations : byte 
        {
            Unknown = 0, 
            TopLeft = 1, 
            TopRight = 2, 
            BottomRight = 3, 
            BottomLeft = 4, 
            LeftTop = 5, 
            RightTop = 6, 
            RightBottom = 7, 
            LeftBottom = 8, 
        }

        public static ExifOrientations DetermineImageRotation(Image img)
        {
            int orientationIndex = Array.IndexOf(img.PropertyIdList, OrientationId);

            if (orientationIndex < 0)
            {
                return ExifOrientations.Unknown;
            }

            return (ExifOrientations)img.GetPropertyItem(OrientationId).Value[0];
        }

        public static bool ImageNeedsRotation(Image img)
        {
            ExifOrientations imageRotation = DetermineImageRotation(img);
            return imageRotation != ExifOrientations.Unknown && imageRotation != ExifOrientations.TopLeft;
        }

        public static void RotateImageUsingExifOrientation(Image img, ExifOrientations orientation)
        {
            switch (orientation)
            {
                case ExifOrientations.Unknown:
                case ExifOrientations.TopLeft:
                    break;
                case ExifOrientations.TopRight:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipX);
                    break;
                case ExifOrientations.BottomRight:
                    img.RotateFlip(RotateFlipType.Rotate180FlipNone);
                    break;
                case ExifOrientations.BottomLeft:
                    img.RotateFlip(RotateFlipType.RotateNoneFlipY);
                    break;
                case ExifOrientations.LeftTop:
                    img.RotateFlip(RotateFlipType.Rotate90FlipX);
                    break;
                case ExifOrientations.RightTop:
                    img.RotateFlip(RotateFlipType.Rotate90FlipNone);
                    break;
                case ExifOrientations.RightBottom:
                    img.RotateFlip(RotateFlipType.Rotate90FlipY);
                    break;
                case ExifOrientations.LeftBottom:
                    img.RotateFlip(RotateFlipType.Rotate270FlipNone);
                    break;
            }

            SetImageOrientation(img, ExifOrientations.TopLeft);
        }

        public static void SetImageOrientation(Image img, ExifOrientations orientation)
        {
            int orientationIndex = Array.IndexOf(img.PropertyIdList, OrientationId);

            if (orientationIndex < 0)
            {
                return;
            }

            PropertyItem item = img.GetPropertyItem(OrientationId);
            item.Value[0] = (byte)orientation;
            img.SetPropertyItem(item);
        }
    }
}
