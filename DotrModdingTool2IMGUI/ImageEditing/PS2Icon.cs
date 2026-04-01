using DotrModdingTool2IMGUI;
using SkiaSharp;

public static class PS2Icon
{
    const int TEX_WIDTH  = 128;
    const int TEX_HEIGHT = 64;
    const int TEX_BYTES  = TEX_WIDTH * TEX_HEIGHT * 2; 

    public static readonly int[] TextureOffsets =
    {
        0x62a8,   
        0x14550,  
        0x22be8,  
    };

    public static int CurrentTextureIndex = 0;

    static bool[][] stpMasks = new bool[3][];

    static byte Expand5To8(int v5) => (byte)((v5 << 3) | (v5 >> 2));

    public static SKBitmap ExtractIconTexture(byte[] iconData)
    {
        if (iconData.Length < TEX_BYTES * 2)
            throw new InvalidDataException($"Icon buffer too small: 0x{iconData.Length:X}, expected 0x{TEX_BYTES * 2:X}.");

        int topOffset = 0;
        int botOffset = TEX_BYTES;

        stpMasks[CurrentTextureIndex] = new bool[TEX_WIDTH * TEX_HEIGHT * 2];

        var bitmap = new SKBitmap(TEX_WIDTH, TEX_HEIGHT * 2, SKColorType.Bgra8888, SKAlphaType.Unpremul);

        unsafe
        {
            uint* pixels = (uint*)bitmap.GetPixels().ToPointer();

            for (int section = 0; section < 2; section++)
            {
                int dataOffset = section == 0 ? topOffset : botOffset;
                int pixelBase  = section == 0 ? 0 : TEX_WIDTH * TEX_HEIGHT;

                for (int i = 0; i < TEX_WIDTH * TEX_HEIGHT; i++)
                {
                    ushort p = BitConverter.ToUInt16(iconData, dataOffset + i * 2);

                    int r5 = (p & 0x1F);
                    int g5 = (p >> 5)  & 0x1F;
                    int b5 = (p >> 10) & 0x1F;

                    byte r = Expand5To8(r5);
                    byte g = Expand5To8(g5);
                    byte b = Expand5To8(b5);

                    bool stp = (p & 0x8000) != 0;
                    stpMasks[CurrentTextureIndex][pixelBase + i] = stp;

                    byte a = (r == 0 && g == 0 && b == 0) ? (byte)0 : (byte)255;

                    pixels[pixelBase + i] =
                        (uint)(a << 24) | ((uint)r << 16) | ((uint)g << 8) | b;
                }
            }
        }

        return bitmap;
    }

    public static void BuildPaletteFromBitmap(SKBitmap bitmap)
    {
        const int maxColors = 256;

        unsafe
        {
            uint* px    = (uint*)bitmap.GetPixels().ToPointer();
            int   count = bitmap.Width * bitmap.Height;

            var uniqueColors = new HashSet<uint>();

            for (int i = 0; i < count; i++)
            {
                uint p   = px[i];
                uint rgb = p & 0x00FFFFFF;
                uniqueColors.Add(rgb);

                if (uniqueColors.Count > maxColors)
                    break;
            }

            if (uniqueColors.Count <= maxColors)
            {
                var palette = new uint[maxColors];
                int i = 0;

                foreach (var rgb in uniqueColors)
                    palette[i++] = 0xFF000000 | rgb;

                for (; i < maxColors; i++)
                    palette[i] = 0;

                GameImageManager.CurrentTexture.Palette = palette;
            }
        }
    }

    public static byte[] SaveIconToByte(byte[] originalData, SKBitmap bitmap)
    {
        if (bitmap.Width != TEX_WIDTH || bitmap.Height != TEX_HEIGHT * 2)
            throw new Exception($"Icon bitmap must be {TEX_WIDTH}x{TEX_HEIGHT * 2}.");

        if (originalData.Length < TEX_BYTES * 2)
            throw new Exception($"Original icon data too small: 0x{originalData.Length:X}, expected 0x{TEX_BYTES * 2:X}.");

        bool[] stpMask = stpMasks[CurrentTextureIndex] ?? new bool[TEX_WIDTH * TEX_HEIGHT * 2];

        byte[] output = (byte[])originalData.Clone();

        int topOffset = 0;
        int botOffset = TEX_BYTES;

        unsafe
        {
            uint* pixels = (uint*)bitmap.GetPixels().ToPointer();

            for (int section = 0; section < 2; section++)
            {
                int dataOffset = section == 0 ? topOffset : botOffset;
                int pixelBase  = section == 0 ? 0 : TEX_WIDTH * TEX_HEIGHT;

                for (int i = 0; i < TEX_WIDTH * TEX_HEIGHT; i++)
                {
                    uint p = pixels[pixelBase + i];

                    byte r8 = (byte)((p >> 16) & 0xFF);
                    byte g8 = (byte)((p >> 8)  & 0xFF);
                    byte b8 = (byte)( p        & 0xFF);

                    ushort r5 = (ushort)((r8 * 31 + 127) / 255);
                    ushort g5 = (ushort)((g8 * 31 + 127) / 255);
                    ushort b5 = (ushort)((b8 * 31 + 127) / 255);

                    bool stp = stpMask[pixelBase + i];

                    ushort packed = (ushort)(b5 | (g5 << 5) | (r5 << 10) | (stp ? 0x8000 : 0));

                    int byteIndex = dataOffset + i * 2;
                    output[byteIndex]     = (byte)(packed & 0xFF);
                    output[byteIndex + 1] = (byte)(packed >> 8);
                }
            }
        }

        return output;
    }
}