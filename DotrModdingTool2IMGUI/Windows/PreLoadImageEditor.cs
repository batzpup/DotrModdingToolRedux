using System.Reflection;
namespace DotrModdingTool2IMGUI;

public static class PreLoadImageEditor
{
    public static byte[][] CardArtBytes = new byte[871][];
    public static byte[][] PreloadCardArtBytes = new byte[223][];
    public static Dictionary<int, int> Images = new Dictionary<int, int>();
    public static ModdedStringName[] PreloadDefaultImageNameList;


    static PreLoadImageEditor()
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
            byte[] bytes = ConvertPictureToPicPack(CardArtBytes[i]);
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