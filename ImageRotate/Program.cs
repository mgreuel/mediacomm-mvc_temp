using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageRotate
{
    using System.Configuration;
    using System.Drawing;
    using System.Drawing.Imaging;
    using System.IO;

    class Program
    {
        static void Main(string[] args)
        {
            string imageBasePath = ConfigurationManager.AppSettings["ImageBasePath"];

            IEnumerable<string> images = Directory.EnumerateFiles(imageBasePath, "*.*", SearchOption.AllDirectories);


            List<string> originalImages = images.Where(
                (s) =>
                    {
                        string lowerFileName = Path.GetFileName(s).ToLower();
                        return !lowerFileName.Contains("small.j") && !lowerFileName.Contains("medium.j")
                               && !lowerFileName.Contains("large.j")
                               && (lowerFileName.EndsWith(".jpg") || lowerFileName.EndsWith(".jpeg"));
                    }).ToList();


            ImageGenerator imageGenerator = new ImageGenerator();

            foreach (string file in originalImages)
            {
                imageGenerator.GenerateImages(new FileInfo(file));
            }
        }
    }
}
