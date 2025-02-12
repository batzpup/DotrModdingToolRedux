using System.Numerics;
using System.Reflection;
using Raylib_cs;

namespace DotrModdingTool2IMGUI;

public static class ImageHelper
{
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
        string resourceName = $"{assembly.GetName().Name}.{resourcePath}";
        //Console.WriteLine($"Loading resource: {resourceName}");
        using (Stream stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream is null)
            {
                Console.Error.WriteLine($"No resource exists with the name {resourceName}");
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
}