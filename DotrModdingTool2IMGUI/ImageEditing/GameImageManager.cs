using System.Reflection;
using SkiaSharp;
namespace DotrModdingTool2IMGUI;

public static class GameImageManager
{
    
    public static byte[][] PictureBytes = new byte[871][];
    public static byte[][] PicPackBytes = new byte[223][];
    public static byte[][] PicMiniBytes = new byte[699][];
    public static byte[][] ModelTextureBytes = new byte[625][];
    public static byte[][] TexEtcBytes = new byte[DataAccess.TexEtcCount][];
    public static byte[][] TexEffBytes = new byte[DataAccess.TexEffCount][];
    public static byte[][] TexAnmBytes = new byte[DataAccess.TexAnmCount][];
    public static byte[][] TexEveBytes = new byte[DataAccess.TexEveCount][];
    public static byte[][] TexSysBytes = new byte[DataAccess.TexSysCount][];
    public static byte[][] MonsterModelBytes = new byte[DataAccess.MonsterModelCount][];
    public static byte[][] IconImageBytes = new byte[DataAccess.IconImageCount][];


    public static HashSet<int> MonsterModelExlusions = new HashSet<int>(); 
    public static Dictionary<int, int> PicPackImages = new Dictionary<int, int>();
    public static ModdedStringName[] PreloadDefaultImageNameList;
    public static GameTexture CurrentTexture = new();
    


    static GameImageManager()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string resourceName = $"{assembly.GetName().Name}.GameData.DefaultPreloadNames.txt";
        using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream is null)
            {
                Console.Error.WriteLine($"No resource exists with the name {resourceName}");
                throw new Exception($"Cannot find{resourceName} ");
            }
            using (StreamReader streamReader = new StreamReader(stream))
            {
                var defaultNameList = streamReader.ReadToEnd().ToString().Split(Environment.NewLine, StringSplitOptions.None);
                PreloadDefaultImageNameList = new ModdedStringName[defaultNameList.Length];
                for (var index = 0; index < defaultNameList.Length; index++)
                {
                    var name = defaultNameList[index];
                    PreloadDefaultImageNameList[index] = new ModdedStringName(name, name);
                }

            }
        }
    }

    public static bool ByteArraysEqual(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
    {
        return a1.SequenceEqual(a2);
    }


    public static int GetPicNumber(ReadOnlySpan<byte> PicPackBytes)
    {
        for (int i = 0; i < 871; i++)
        {
            byte[] bytes = ConvertPictureToPicPack(PictureBytes[i]);
            if (ByteArraysEqual(PicPackBytes, bytes))
            {
                return i;
            }
        }
        return 195;
    }


    public static byte[] ConvertPictureToPicPack(byte[] picture)
    {

        if (picture == null || picture.Length != DataAccess.PictureSize)
        {
            return null;
        }
        byte[] pictureBytes = new byte[DataAccess.PicPackSize];
        Array.Copy(picture, pictureBytes, DataAccess.PicPackSize);
        return pictureBytes;
    }
}

public class GameTexture
{
    public SKBitmap Bitmap = new SKBitmap();
    public uint[] Palette = Array.Empty<uint>();
    public ImageMetaData MetaData;
}

public struct ImageMetaData
{
    //Infile
    public int TotalImageSize;
    public int HeaderSize;
    public int PalleteSize;
    public int PalleteSize2;
    public int Width;
    public int Height;
    public int NumberOfSections;
    public int Padding;


    //Derived
    public int StartOfImage;
    public int PalleteOffset;
    
}