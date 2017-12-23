#r "System.Drawing"

using ImageResizer;
using System.Drawing;
using System.IO;
using System.Drawing.Imaging;

public static void Run(Stream inputImage, string imageName, Stream resizedImage, TraceWriter log)
{
    log.Info($"C# Blob trigger function Processed blob\n Name:{imageName} \n Size: {inputImage.Length} Bytes");

    var extension = Path.GetExtension(imageName);
    var settings = new ImageResizer.ResizeSettings{
        MaxWidth = 40,
        MaxHeight = 40,
        Format = extension
    };

    ImageResizer.ImageBuilder.Current.Build(inputImage, resizedImage, settings);

}