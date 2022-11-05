using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media.Imaging;
using QoiSharp;

namespace GaSpTK.Editor
{
    public static class BitmapLoader
    {
        public static Bitmap Convert(string path)
        {
            // qoi
            if (path.EndsWith(".qoi"))
            {
                // decode QOI image
                byte[] data = System.IO.File.ReadAllBytes(path);
                var imgData = QoiDecoder.Decode(data);

                if (imgData.Channels == QoiSharp.Codec.Channels.Rgb)
                {
                    // we have to transcode into rgba32
                    byte[] rgba32 = new byte[imgData.Width * imgData.Height * 4];

                    for (int i = 0; i < (imgData.Width * imgData.Height); i++)
                    {
                        int src = i * 3;
                        int dst = i * 4;

                        rgba32[dst] = imgData.Data[src];
                        rgba32[dst + 1] = imgData.Data[src + 1];
                        rgba32[dst + 2] = imgData.Data[src + 2];
                        rgba32[dst + 3] = 255;
                    }

                    unsafe
                    {
                        fixed (void* dataPtr = rgba32)
                        {
                            return new Bitmap(Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Opaque,
                                (IntPtr)dataPtr, new Avalonia.PixelSize(imgData.Width, imgData.Height), new Avalonia.Vector(96, 96), imgData.Width * 4);
                        }
                    }
                }
                else
                {
                    // no need to transcode
                    unsafe
                    {
                        fixed (void* dataPtr = imgData.Data)
                        {
                            return new Bitmap(Avalonia.Platform.PixelFormat.Rgba8888, Avalonia.Platform.AlphaFormat.Unpremul,
                                (IntPtr)dataPtr, new Avalonia.PixelSize(imgData.Width, imgData.Height), new Avalonia.Vector(96, 96), imgData.Width * 4);
                        }
                    }
                }
            }
            else
            {
                // just decode a regular bitmap (png, jpg, etc)
                return new Bitmap(path);
            }
        }
    }
}