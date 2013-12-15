using System.IO;

namespace MediaCommMvc.Web.Infrastructure
{
    public interface IImageGenerator
    {
        void GenerateImages(FileInfo sourceImage);
    }
}