using DotrModdingTool2IMGUI;
using Raylib_cs;
using SkiaSharp;

public static class ImageCreator
{
    const int paddingSize = 0x60;
    static readonly byte[] startOfImageSig = { 0x52, 0x48, 0x32, 0x00, 0x00, 0x00, 0x00, 0x00 };
    static readonly byte[] endOfSectionSig = { 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };
    public static SKBitmap savedImage = new SKBitmap();
    public static Texture2D texture;

    static int ConvertPlace(int place, int paletteSize)
    {
        int placeType = (place % 32) / 8;
        if (placeType == 1) place += 8;
        else if (placeType == 2) place -= 8;
        return place % paletteSize;
    }

    static uint ExpandColor(byte p)
    {
        int red = (p >> 5) & 0x07;
        int green = (p >> 2) & 0x07;
        int blue = p & 0x03;

        red = (red << 5) | (red << 2) | (red >> 1);
        green = (green << 5) | (green << 2) | (green >> 1);
        blue = (blue << 6) | (blue << 4) | (blue << 2) | blue;

        // Pack as BGRA for SKBitmap's native format
        return (uint)(0xFF000000 | (red << 16) | (green << 8) | blue);
    }

    static (int paletteOffset, int paletteSize) FindPaletteStartAndSize(byte[] data, int totalImageSize)
    {
        var span = data.AsSpan(0, totalImageSize);
        var pattern = endOfSectionSig.AsSpan();
        int? paletteOffset = null;
        int pos = 0;

        while (pos < span.Length)
        {
            int idx = span[pos..].IndexOf(pattern);
            if (idx == -1) break;
            idx += pos;

            if (paletteOffset == null)
                paletteOffset = idx + endOfSectionSig.Length;
            else
            {
                int paletteEnd = idx + endOfSectionSig.Length;
                return (paletteOffset.Value, paletteEnd - paletteOffset.Value - paddingSize);
            }
            pos = idx + endOfSectionSig.Length;
        }

        return (-1, -1);
    }

    static int GetPartialImageSize(byte[] data, int totalImageSize, int startOfImage)
    {
        var span = data.AsSpan(startOfImage, totalImageSize - startOfImage);
        var pattern = endOfSectionSig.AsSpan();
        int pos = 0;

        while (pos < span.Length)
        {
            int idx = span[pos..].IndexOf(pattern);
            if (idx == -1) break;
            idx += pos;

            int address = startOfImage + idx;
            if (address % 16 == 6)
                return address + endOfSectionSig.Length - startOfImage - paddingSize;

            pos = idx + endOfSectionSig.Length;
        }

        return totalImageSize - startOfImage;
    }

    static uint[] ReadPalette(byte[] data, int paletteOffset, int paletteSize)
    {
        if (paletteSize == 1024)
        {
            var palette = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                int p = BitConverter.ToInt32(data, paletteOffset + i * 4);
                byte r = (byte)(p & 0xFF);
                byte g = (byte)((p >> 8) & 0xFF);
                byte b = (byte)((p >> 16) & 0xFF);
                palette[i] = (uint)(0xFF000000 | (r << 16) | (g << 8) | b);
            }
            return palette;
        }
        else if (paletteSize == 32)
        {
            var palette = new uint[32];
            for (int i = 0; i < 32; i++)
                palette[i] = ExpandColor(data[paletteOffset + i]);
            return palette;
        }
        else
        {
            int entryCount = paletteSize / 2;
            var palette = new uint[entryCount];
            for (int i = 0; i < entryCount; i++)
            {
                ushort p = BitConverter.ToUInt16(data, paletteOffset + i * 2);
                byte r = (byte)((p & 0x1F) * 255 / 31);
                byte g = (byte)(((p >> 5) & 0x1F) * 255 / 31);
                byte b = (byte)(((p >> 10) & 0x1F) * 255 / 31);
                palette[i] = (uint)(0xFF000000 | (r << 16) | (g << 8) | b);
            }
            return palette;
        }
    }

    static unsafe void LoadImgSection(byte[] data, uint[] palette, uint* pixels,
        int sectionNumber, int startOfImage, int partialImageSize, int paletteSize,
        int imageWidth, int imageHeight)
    {
        int sectionOffset = startOfImage + (partialImageSize + paddingSize) * sectionNumber;
        int blockWidth = Math.Min(imageWidth, 128);
        int blockHeight = Math.Min(imageHeight, 64);
        int subBlocksPerRow = blockWidth / 16;

        
        var colours = new uint[partialImageSize];
        for (int i = 0; i < partialImageSize; i++)
            colours[i] = palette[ConvertPlace(data[sectionOffset + i], paletteSize)];

        for (int i = 0; i < partialImageSize; i++)
        {
            int block = i / 32;
            int blockHoriz = 16 * (block % subBlocksPerRow);
            int blockRow = block / subBlocksPerRow;
            int blockVert = (2 * blockRow) - (blockRow % 2);
            int subBlock = i % 4;
            int blockOffset = (block / 16) % 2;
            int subBlockInd = (4 * (blockOffset + subBlock) + (i / 4)) % 8;

            int x = blockHoriz + 8 * (subBlock / 2) + subBlockInd + blockWidth * (sectionNumber % (imageWidth / blockWidth));
            int y = blockVert + 2 * (subBlock % 2) + blockHeight * (sectionNumber / (imageWidth / blockWidth));

            if ((uint)x < (uint)imageWidth && (uint)y < (uint)imageHeight)
                pixels[x + y * imageWidth] = colours[i];
        }
    }

    public static void CreateImageFromBytes(byte[] picture, string outputPath)
    {
        int totalImageSize = BitConverter.ToInt32(picture, 0x8);
        int imageWidth = BitConverter.ToUInt16(picture, 0x54);
        int imageHeight = BitConverter.ToUInt16(picture, 0x56);
        int numberOfSections = BitConverter.ToUInt16(picture, 0x58);

        var (paletteOffset, paletteSize) = FindPaletteStartAndSize(picture, totalImageSize);
        if (paletteOffset == -1) return;
        if (paletteSize != 0x200 && paletteSize != 0x400) return;

        int startOfImage = paletteOffset + paletteSize + paddingSize;
        int partialImageSize = GetPartialImageSize(picture, totalImageSize, startOfImage);
        var palette = ReadPalette(picture, paletteOffset, paletteSize);

        if (numberOfSections > 100) numberOfSections = 100;

         var bmp = new SKBitmap(imageWidth, imageHeight, SKColorType.Bgra8888, SKAlphaType.Premul);
        unsafe
        {
            uint* pixels = (uint*)bmp.GetPixels().ToPointer();
            for (int i = 0; i < numberOfSections - 1; i++)
                LoadImgSection(picture, palette, pixels, i, startOfImage, partialImageSize, paletteSize, imageWidth, imageHeight);
        }
        savedImage?.Dispose();
        savedImage = bmp;
        texture = ImageHelper.SKBitmapToRaylibTexture(savedImage);
        using SKImage image = SKImage.FromBitmap(bmp);
        using SKData skData = image.Encode(SKEncodedImageFormat.Png, 100);

        using var stream = File.OpenWrite(outputPath);
        skData.SaveTo(stream);
    }
}

public static class ImageSaver
{
    
    static (int x, int y)[] BuildSectionMap(
        int sectionNumber, int partialImageSize,
        int imageWidth, int imageHeight)
    {
        int blockWidth  = Math.Min(imageWidth,  128);
        int blockHeight = Math.Min(imageHeight,  64);
        int subBlocksPerRow = blockWidth / 16;

        var map = new (int x, int y)[partialImageSize];

        for (int i = 0; i < partialImageSize; i++)
        {
            int block       = i / 32;
            int blockHoriz  = 16 * (block % subBlocksPerRow);
            int blockRow    = block / subBlocksPerRow;
            int blockVert   = (2 * blockRow) - (blockRow % 2);
            int subBlock    = i % 4;
            int blockOffset = (block / 16) % 2;
            int subBlockInd = (4 * (blockOffset + subBlock) + (i / 4)) % 8;

            int x = blockHoriz + 8 * (subBlock / 2) + subBlockInd
                    + blockWidth  * (sectionNumber % (imageWidth  / blockWidth));
            int y = blockVert   + 2 * (subBlock % 2)
                    + blockHeight * (sectionNumber / (imageWidth  / blockWidth));

            map[i] = ((uint)x < (uint)imageWidth && (uint)y < (uint)imageHeight)
                ? (x, y)
                : (-1, -1);   
        }

        return map;
    }
    
    static int FindNearestPaletteIndex(uint targetArgb, uint[] palette,
        Dictionary<uint, int> cache)
    {
        if (cache.TryGetValue(targetArgb, out int cached))
            return cached;

        byte tr = (byte)(targetArgb >> 16);
        byte tg = (byte)(targetArgb >>  8);
        byte tb = (byte) targetArgb;

        int best      = 0;
        long bestDist = long.MaxValue;

        for (int i = 0; i < palette.Length; i++)
        {
            long dr = tr - (byte)(palette[i] >> 16);
            long dg = tg - (byte)(palette[i] >>  8);
            long db = tb - (byte) palette[i];
            long dist = dr * dr + dg * dg + db * db;

            if (dist == 0) { best = i; break; }  
            if (dist < bestDist) { bestDist = dist; best = i; }
        }

        cache[targetArgb] = best;
        return best;
    }

    
    public static byte[] SaveImageToBytes(byte[] originalData, string pngPath)
    {
       
        const int paddingSize = 0x60;

        int totalImageSize   = BitConverter.ToInt32 (originalData, 0x8);
        int imageWidth       = BitConverter.ToUInt16(originalData, 0x54);
        int imageHeight      = BitConverter.ToUInt16(originalData, 0x56);
        int numberOfSections = BitConverter.ToUInt16(originalData, 0x58);
        if (numberOfSections > 100) numberOfSections = 100;

        var (paletteOffset, paletteSize) =
            FindPaletteStartAndSize(originalData, totalImageSize, paddingSize);
        if (paletteOffset == -1)
            throw new InvalidDataException("Could not locate palette in source data.");
        if (paletteSize != 0x200 && paletteSize != 0x400)
            throw new InvalidDataException($"Unsupported palette size 0x{paletteSize:X}.");

        int startOfImage    = paletteOffset + paletteSize + paddingSize;
        int partialImageSize = GetPartialImageSize(
            originalData, totalImageSize, startOfImage, paddingSize);

        uint[] palette = ReadPalette(originalData, paletteOffset, paletteSize);

        
        int paletteEntryCount = palette.Length;

    
        using var bmp = SKBitmap.Decode(pngPath);
        if (bmp == null)
            throw new FileNotFoundException($"Could not load PNG: {pngPath}");
        if (bmp.Width != imageWidth || bmp.Height != imageHeight)
            throw new InvalidDataException(
                $"PNG size {bmp.Width}×{bmp.Height} doesn't match " +
                $"target {imageWidth}×{imageHeight}.");

  
        var pixels = new uint[imageWidth * imageHeight];
        unsafe
        {
            uint* src = (uint*)bmp.GetPixels().ToPointer();
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = src[i];
        }

    
        var colourCache = new Dictionary<uint, int>();

      
        byte[] output = (byte[])originalData.Clone();

        for (int sec = 0; sec < numberOfSections - 1; sec++)
        {
            int sectionOffset = startOfImage + (partialImageSize + paddingSize) * sec;
            var map = BuildSectionMap(sec, partialImageSize, imageWidth, imageHeight);

            for (int i = 0; i < partialImageSize; i++)
            {
                var (px, py) = map[i];
                if (px == -1) continue;   

                uint argb        = pixels[px + py * imageWidth];
                int  nearestIdx  = FindNearestPaletteIndex(argb, palette, colourCache);

              
                int rawByte = ReverseConvertPlace(nearestIdx, paletteEntryCount);
                output[sectionOffset + i] = (byte)rawByte;
            }
        }

        return output;
    }


    static readonly byte[] endOfSectionSig =
        { 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

    static (int paletteOffset, int paletteSize) FindPaletteStartAndSize(
        byte[] data, int totalImageSize, int paddingSize)
    {
        var span    = data.AsSpan(0, totalImageSize);
        var pattern = endOfSectionSig.AsSpan();
        int? paletteOffset = null;
        int  pos = 0;

        while (pos < span.Length)
        {
            int idx = span[pos..].IndexOf(pattern);
            if (idx == -1) break;
            idx += pos;

            if (paletteOffset == null)
                paletteOffset = idx + endOfSectionSig.Length;
            else
            {
                int paletteEnd = idx + endOfSectionSig.Length;
                return (paletteOffset.Value, paletteEnd - paletteOffset.Value - paddingSize);
            }
            pos = idx + endOfSectionSig.Length;
        }
        return (-1, -1);
    }

    static int GetPartialImageSize(
        byte[] data, int totalImageSize, int startOfImage, int paddingSize)
    {
        var span    = data.AsSpan(startOfImage, totalImageSize - startOfImage);
        var pattern = endOfSectionSig.AsSpan();
        int pos = 0;

        while (pos < span.Length)
        {
            int idx = span[pos..].IndexOf(pattern);
            if (idx == -1) break;
            idx += pos;

            int address = startOfImage + idx;
            if (address % 16 == 6)
                return address + endOfSectionSig.Length - startOfImage - paddingSize;

            pos = idx + endOfSectionSig.Length;
        }
        return totalImageSize - startOfImage;
    }

    static uint[] ReadPalette(byte[] data, int paletteOffset, int paletteSize)
    {
        if (paletteSize == 1024)   // 0x400 — 4-byte RGBA
        {
            var palette = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                int p = BitConverter.ToInt32(data, paletteOffset + i * 4);
                byte r = (byte)(p & 0xFF);
                byte g = (byte)((p >> 8) & 0xFF);
                byte b = (byte)((p >> 16) & 0xFF);
                palette[i] = (uint)(0xFF000000 | (r << 16) | (g << 8) | b);
            }
            return palette;
        }
        else   // 0x200 — 2-byte RGB555
        {
            int entryCount = paletteSize / 2;
            var palette = new uint[entryCount];
            for (int i = 0; i < entryCount; i++)
            {
                ushort p = BitConverter.ToUInt16(data, paletteOffset + i * 2);
                byte r = (byte)((p & 0x1F) * 255 / 31);
                byte g = (byte)(((p >> 5) & 0x1F) * 255 / 31);
                byte b = (byte)(((p >> 10) & 0x1F) * 255 / 31);
                palette[i] = (uint)(0xFF000000 | (r << 16) | (g << 8) | b);
            }
            return palette;
        }
    }

  
    static int ReverseConvertPlace(int targetIndex, int paletteSize)
    {

        for (int raw = 0; raw < paletteSize; raw++)
        {
            int placeType = (raw % 32) / 8;
            int adjusted  = raw;
            if (placeType == 1) adjusted += 8;
            else if (placeType == 2) adjusted -= 8;
            if (adjusted % paletteSize == targetIndex)
                return raw;
        }
        return targetIndex;
    }
}