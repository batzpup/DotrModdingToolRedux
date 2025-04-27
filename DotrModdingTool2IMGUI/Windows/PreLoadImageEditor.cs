using DiscUtils.Iso9660;
namespace DotrModdingTool2IMGUI;

public static class PreLoadImageEditor
{
    public static byte[][] CardArtBytes = new byte[871][];
    public static byte[][] PreloadCardArtBytes = new byte[223][];

    public static string[] fileEntries;


    public static bool ByteArraysEqual(ReadOnlySpan<byte> a1, ReadOnlySpan<byte> a2)
    {
        return a1.SequenceEqual(a2);
    }
/*
    public void LoadAllImages2(string filePath)
    {
        string newIsoDirectory = Path.GetDirectoryName(filePath);
        string newIsoName = Path.GetFileNameWithoutExtension(filePath) + "AllPreloaded.iso";
        string newIsoPath = Path.Combine(newIsoDirectory, newIsoName);
        string mrgDirectory = "C:\\Users\\Batzpup\\Documents\\Ps2Games";
        string mrgPath = Path.Combine(mrgDirectory, "PICPACK.MRG");

        using (var editor = new UdfEditor(filePath, newIsoPath))
        {
            var fileId = editor.GetFileByName("PICPACK.MRG");
            if (fileId is not null)
            {
                editor.RemoveFile(fileId);

                using (FileStream fs = new FileStream(mrgPath, FileMode.Create, FileAccess.ReadWrite))
                {
                    for (int i = 0; i < 871; i++)
                    {
                        byte[] picture = CardArt[i];
                        if (picture.Length == PictureSize)
                        {
                            lock (FileStreamLock)
                            {
                                fs.Seek(i * PicPackSize, SeekOrigin.Begin);
                                fs.Write(ConvertPictureToPicPack(picture), 0, PicPackSize);
                            }
                        }
                        else
                        {
                            MessageBox.Show($"Card: #{i}'s picture length is not 0x4800");
                        }
                    }

                    // Reset the file stream's position before adding
                    fs.Seek(0, SeekOrigin.Begin);

                    // Add the modified PICPACK.MRG to the DATA directory in the ISO
                    editor.AddFile("DATA\\PICPACK.MRG", fs);

                    // After adding, you can delete the temporary PICPACK.MRG if needed
                    // File.Delete(mrgPath);

                    editor.Rebuild(newIsoPath);
                }
            }
        }
    }

*/


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