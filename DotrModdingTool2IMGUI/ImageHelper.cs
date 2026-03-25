using System.Numerics;
using System.Reflection;
using ImGuiNET;
using Raylib_cs;
using SkiaSharp;

namespace DotrModdingTool2IMGUI;

public static class ImageHelper
{
    public static Vector2 DefaultImageSize;

    public static Vector4 ColorToVec4(Color color)
    {
        return new Vector4(color.R, color.G, color.B, color.A);
    }

    public static Vector4 ColorToVec4Normalised(Color color)
    {
        return new Vector4(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
    }

    public static IntPtr LoadImageImgui(string resourcePath)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        //Needed for linux
        string sanitizedPath = resourcePath
            .Trim()
            .Replace("\r", "")
            .Replace("\n", "");
        string resourceName = $"{assembly.GetName().Name}.{sanitizedPath}";
        // Console.WriteLine($"Loading resource: {resourceName}");
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream is null)
            {
                Console.WriteLine($"No resource exists with the name {resourceName}");
                return -1;
            }
            byte[] imageData = new byte[stream.Length];
            int bytesRead = stream.Read(imageData);
            if (bytesRead == stream.Length)
            {
                Image raylibImage = Raylib.LoadImageFromMemory(".png", imageData);
                Texture2D texture = Raylib.LoadTextureFromImage(raylibImage);
                Raylib.UnloadImage(raylibImage);
                IntPtr textureId = (IntPtr)texture.Id;
                return textureId;
            }

            Console.WriteLine($"Not All bytes were read for {resourceName}");
            return IntPtr.Zero;


        }
    }


    public static Image LoadImageRaylib(string resourcePath)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string resourceName = $"{assembly.GetName().Name}.{resourcePath}";
        //Console.WriteLine($"Loading resource: {resourceName}");
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream is null)
            {
                Console.Error.WriteLine($"No resource exists with the name {resourceName}");
                return new Image();
            }
            byte[] imageData = new byte[stream.Length];
            int bytesRead = stream.Read(imageData);
            if (bytesRead == stream.Length)
            {
                Image raylibImage = Raylib.LoadImageFromMemory(".png", imageData);
                return raylibImage;
            }

            Console.WriteLine($"Not All bytes were read for {resourceName}");
            return new Image();
        }

    }

    public static Texture2D SKBitmapToRaylibTexture(SKBitmap bitmap)
    {
        Image raylibImage = new Image {
            Width = bitmap.Width,
            Height = bitmap.Height,
            Format = PixelFormat.UncompressedR8G8B8A8,
            Mipmaps = 1
        };

        // SKBitmap is BGRA, Raylib expects RGBA so we need to swizzle
        byte[] pixels = new byte[bitmap.Width * bitmap.Height * 4];
        for (int i = 0; i < bitmap.Width * bitmap.Height; i++)
        {
            int src = i * 4;
            pixels[src + 0] = bitmap.Bytes[src + 2]; // R (from B)
            pixels[src + 1] = bitmap.Bytes[src + 1]; // G
            pixels[src + 2] = bitmap.Bytes[src + 0]; // B (from R)
            pixels[src + 3] = bitmap.Bytes[src + 3]; // A
        }

        unsafe
        {
            fixed (byte* ptr = pixels)
                raylibImage.Data = ptr;

            return Raylib.LoadTextureFromImage(raylibImage);
        }
    }
}