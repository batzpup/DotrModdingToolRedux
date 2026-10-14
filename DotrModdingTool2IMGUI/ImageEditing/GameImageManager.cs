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
    public static Dictionary<int, (int PictureIndex, byte[] PictureBytes)> InexactPicPackSources = new();
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


    

    public static bool IsValidCardIndex(int index)
    {
        return index >= 0 && index < Card.TotalCardCount;
    }

    public static bool PicPackMatchesPicture(ReadOnlySpan<byte> PicPackBytes, int pictureIndex)
    {
        byte[] bytes = ConvertPictureToPicPack(PictureBytes[pictureIndex]);
        if (bytes != null && ByteArraysEqual(PicPackBytes, bytes))
        {
            return true;
        }
        // Known exception: PicPack index 75 ("Performance of Sword") doesnt exact-matches Picture[195]
        return pictureIndex == 195 && PicPackBytes.SequenceEqual(ImageHelper.PerformanceOfSwordSignature);
    }

    public static int GetPicNumber(ReadOnlySpan<byte> PicPackBytes)
    {
        for (int i = 0; i < 871; i++)
        {
            if (PicPackMatchesPicture(PicPackBytes, i))
            {
                return i;
            }
        }

        return -1;
    }

    /// <summary>
    /// Resolves which picture a PicPack slot shows. The SLUS card index is authoritative; byte matching is only a
    /// fallback for builds where that table holds an invalid index. Entries that aren't an exact copy of their picture
    /// (e.g. translation patches that edit one but not the other) are recorded in InexactPicPackSources so saving
    /// doesn't overwrite them unless the slot is reassigned or its picture is edited.
    /// </summary>
    public static void MapPicPackSlot(int slot, int slusCardIndex)
    {
        byte[] picPack = PicPackBytes[slot];
        int pictureIndex = IsValidCardIndex(slusCardIndex) ? slusCardIndex : GetPicNumber(picPack);
        PicPackImages[slot] = pictureIndex;
        if (pictureIndex == -1)
        {
            return;
        }

        byte[] exactPicPack = ConvertPictureToPicPack(PictureBytes[pictureIndex]);
        if (exactPicPack == null || !exactPicPack.AsSpan().SequenceEqual(picPack))
        {
            InexactPicPackSources[slot] = (pictureIndex, (byte[])PictureBytes[pictureIndex].Clone());
        }
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