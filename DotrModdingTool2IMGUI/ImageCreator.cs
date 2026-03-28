using DotrModdingTool2IMGUI;
using Raylib_cs;
using SkiaSharp;

public static class ImageCreator
{
    const int paddingSize = 0x60;
    public static readonly byte[] startOfImageSig = { 0x52, 0x48, 0x32, 0x00, 0x00, 0x00, 0x00, 0x00 };
    public static readonly byte[] endOfSectionSig = { 0x00, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };


    public static List<SKColor> ExtractPalette()
    {

        return GameImageManager.CurrentTexture.Palette
            .Select(argb => new SKColor(
                (byte)(argb >> 16),
                (byte)(argb >> 8),
                (byte)(argb),
                (byte)(argb >> 24)))
            .ToList();
    }

    //AI assisted to use AI generated OctreeQuantiser
    public static void BuildPaletteFromBitmap(SKBitmap bitmap, byte[] originalData)
    {
        int totalImageSize = BitConverter.ToInt32(originalData, 0x8);
        var (paletteOffset, paletteSize) = FindPaletteStartAndSize(originalData, totalImageSize);
        if (paletteOffset == -1) throw new InvalidDataException("Could not locate palette.");

        int maxColors = paletteSize == 0x400 ? 256 : paletteSize / 2;

        var colorCounts = new Dictionary<(byte R, byte G, byte B), int>();
        unsafe
        {
            uint* px = (uint*)bitmap.GetPixels().ToPointer();
            int count = bitmap.Width * bitmap.Height;
            for (int i = 0; i < count; i++)
            {
                uint p = px[i];
                byte r = (byte)p;
                byte g = (byte)(p >> 8);
                byte b = (byte)(p >> 16);
                var key = (r, g, b);
                colorCounts.TryGetValue(key, out int c);
                colorCounts[key] = c + 1;
            }
        }

        // Build octree with sqrt frequency weighting
        var quantizer = new OctreeQuantizer(maxColors);
        foreach (var ((r, g, b), count) in colorCounts)
        {
            int weight = Math.Max(1, (int)Math.Sqrt(count));
            for (int i = 0; i < weight; i++)
                quantizer.AddColor(r, g, b);
        }

        var palette = quantizer.GetPalette();

        // Build remap cache: original colour -> nearest palette colour
        var remapCache = new Dictionary<(byte R, byte G, byte B), (byte R, byte G, byte B)>();
        foreach (var (r, g, b) in colorCounts.Keys)
            remapCache[(r, g, b)] = FindNearestColor(palette, r, g, b);

        unsafe
        {
            uint* px = (uint*)bitmap.GetPixels().ToPointer();
            int count = bitmap.Width * bitmap.Height;
            for (int i = 0; i < count; i++)
            {
                uint p = px[i];
                byte r = (byte)p;
                byte g = (byte)(p >> 8);
                byte b = (byte)(p >> 16);
                var (nr, ng, nb) = remapCache[(r, g, b)];
                // Write back as Bgra8888: B@16, G@8, R@0
                px[i] = (uint)(0xFF000000 | ((uint)nb << 16) | ((uint)ng << 8) | nr);
            }
        }

        // Write palette to texture (as ARGB: 0xFF_RR_GG_BB)
        var finalPalette = new uint[maxColors];
        for (int i = 0; i < palette.Count; i++)
        {
            var (r, g, b) = palette[i];
            finalPalette[i] = (uint)(0xFF000000 | ((uint)r << 16) | ((uint)g << 8) | b);
        }

        GameImageManager.CurrentTexture.Palette = finalPalette;
    }

    private static (byte R, byte G, byte B) FindNearestColor(
        List<(byte R, byte G, byte B)> palette, byte r, byte g, byte b)
    {
        var best = palette[0];
        long bestDist = long.MaxValue;
        foreach (var (pr, pg, pb) in palette)
        {
            long dr = r - pr, dg = g - pg, db = b - pb;
            long dist = dr * dr + dg * dg + db * db;
            if (dist < bestDist)
            {
                bestDist = dist;
                best = (pr, pg, pb);
            }
        }
        return best;
    }

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

        // SKBitmap uses BGRA 
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
        if (paletteSize == 0x400)
        {
            var palette = new uint[256];
            for (int i = 0; i < 256; i++)
            {
                int p = BitConverter.ToInt32(data, paletteOffset + i * 4);
                byte r = (byte)(p & 0xFF);
                byte g = (byte)((p >> 8) & 0xFF);
                byte b = (byte)((p >> 16) & 0xFF);
                palette[i] = (uint)(0xFF000000 | (r << 16) | (uint)(g << 8) | b);
            }
            return palette;
        }
        else if (paletteSize == 0x20)
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
                palette[i] = (uint)(0xFF000000 | (r << 16) | (uint)(g << 8) | b);
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

    public static void CreateImageFromBytes(byte[] bytes, ImageMrgFile mrgFile, string outputPath, bool saveToPath)
    {
        ImageMetaData metaData = new ImageMetaData();
        switch (mrgFile)
        {

            case ImageMrgFile.Picture:
                LoadPicture(bytes, ref metaData);
                break;
            case ImageMrgFile.PicMini:
                LoadPicMini(bytes, ref metaData);
                break;
            case ImageMrgFile.TexSys:
                break;
            case ImageMrgFile.Model:
                break;
            case ImageMrgFile.TexAnm:
                break;
            case ImageMrgFile.TexEff:
                break;
            case ImageMrgFile.TexEtc:
                break;
            case ImageMrgFile.TexEve:
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(mrgFile), mrgFile, null);
        }


        if (!saveToPath)
        {
            return;
        }
        using SKImage image = SKImage.FromBitmap(GameImageManager.CurrentTexture.Bitmap);
        using SKData skData = image.Encode(SKEncodedImageFormat.Png, 100);
        using var stream = File.OpenWrite(outputPath);
        skData.SaveTo(stream);

    }

    static void LoadPicMini(byte[] bytes, ref ImageMetaData metaData)
    {
        metaData.TotalImageSize = 0x880;
        metaData.Width = 40;
        metaData.Height = 32;
        metaData.NumberOfSections = 1;
        metaData.PalleteOffset = 0x120;
        metaData.StartOfImage = 0x380;

        var (paletteOffset, paletteSize) = FindPaletteStartAndSize(bytes, metaData.TotalImageSize);
        if (paletteOffset == -1) return;
        metaData.PalleteOffset = paletteOffset;
        metaData.PalleteSize = paletteSize;
        if (!IsValidPalletSize(metaData.PalleteSize))
        {
            return;
        }

        GameImageManager.CurrentTexture.Palette = ReadPalette(bytes, paletteOffset, paletteSize);

        GameImageManager.CurrentTexture.Bitmap?.Dispose();
        GameImageManager.CurrentTexture.Bitmap = new SKBitmap(metaData.Width, metaData.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
        unsafe
        {
            uint* pixels = (uint*)GameImageManager.CurrentTexture.Bitmap.GetPixels().ToPointer();
            LoadPicMiniSection(bytes, GameImageManager.CurrentTexture.Palette, pixels, metaData.StartOfImage, metaData.Width, metaData.Height);

        }
        GameImageManager.CurrentTexture.MetaData = metaData;
    }

    unsafe static void LoadPicMiniSection(byte[] bytes, uint[] palette, uint* pixels, int startOfImage, int width, int height)
    {
        int pixelCount = width * height;
        for (int i = 0; i < pixelCount; i++)
        {
            int rawIndex = bytes[startOfImage + i];
            int mappedIndex = ConvertPlace(rawIndex, palette.Length);
            pixels[i] = palette[mappedIndex];
        }
    }

    static void LoadPicture(byte[] picture, ref ImageMetaData metaData)
    {
        metaData.TotalImageSize = BitConverter.ToInt32(picture, 0x8);
        metaData.Width = BitConverter.ToUInt16(picture, 0x54);
        metaData.Height = BitConverter.ToUInt16(picture, 0x56);
        metaData.NumberOfSections = BitConverter.ToUInt16(picture, 0x58);

        //Todo Look at this function may be a better way to do this
        var (paletteOffset, paletteSize) = FindPaletteStartAndSize(picture, metaData.TotalImageSize);
        if (paletteOffset == -1) return;
        metaData.PalleteOffset = paletteOffset;
        metaData.PalleteSize = paletteSize;
        if (!IsValidPalletSize(metaData.PalleteSize))
        {
            return;
        }

        metaData.StartOfImage = paletteOffset + paletteSize + paddingSize;

        int partialImageSize = GetPartialImageSize(picture, metaData.TotalImageSize, metaData.StartOfImage);
        GameImageManager.CurrentTexture.Palette = ReadPalette(picture, paletteOffset, paletteSize);

        //Todo Check if this is still necessary
        if (metaData.NumberOfSections > 100) metaData.NumberOfSections = 100;

        GameImageManager.CurrentTexture.Bitmap?.Dispose();

        GameImageManager.CurrentTexture.Bitmap = new SKBitmap(metaData.Width, metaData.Height, SKColorType.Bgra8888, SKAlphaType.Premul);
        unsafe
        {
            uint* pixels = (uint*)GameImageManager.CurrentTexture.Bitmap.GetPixels().ToPointer();
            for (int i = 0; i < metaData.NumberOfSections - 1; i++)
            {
                LoadImgSection(picture, GameImageManager.CurrentTexture.Palette, pixels, i, metaData.StartOfImage, partialImageSize, paletteSize, metaData.Width, metaData.Height);
            }

        }
        GameImageManager.CurrentTexture.MetaData = metaData;
    }

    static bool IsValidPalletSize(int palleteSize)
    {
        return palleteSize == 0x400 || palleteSize == 0x200 || palleteSize == 0x20;
    }

    public static void RemapColourInBitmap(SKBitmap bitmap, SKColor oldCol, SKColor newCol)
    {
        unsafe
        {
            uint* pixels = (uint*)bitmap.GetPixels().ToPointer();
            int count = bitmap.Width * bitmap.Height;

            uint oldBgra = (uint)(0xFF000000 | (oldCol.Blue << 16) | (oldCol.Green << 8) | oldCol.Red);
            uint newBgra = (uint)(0xFF000000 | (newCol.Blue << 16) | (newCol.Green << 8) | newCol.Red);

            for (int i = 0; i < count; i++)
            {
                if ((pixels[i] & 0x00FFFFFF) == (oldBgra & 0x00FFFFFF))
                {
                    pixels[i] = newBgra;
                }

            }
        }
    }

    public static SKBitmap LoadAndPrepareImage(string path, int targetWidth, int targetHeight, bool cropToFit)
    {
        var bitmap = SKBitmap.Decode(path);

        if (bitmap.Width != targetWidth || bitmap.Height != targetHeight)
        {
            var resized = new SKBitmap(targetWidth, targetHeight);
            using var canvas = new SKCanvas(resized);

            SKRect srcRect, dstRect;
            if (cropToFit)
            {
                float srcRatio = (float)bitmap.Width / bitmap.Height;
                float dstRatio = (float)targetWidth / targetHeight;

                int srcX, srcY, srcW, srcH;
                if (Math.Abs(srcRatio - dstRatio) < 0.01f)
                {
                    srcX = 0;
                    srcY = 0;
                    srcW = bitmap.Width;
                    srcH = bitmap.Height;
                }
                else if (srcRatio > dstRatio)
                {
                    srcH = bitmap.Height;
                    srcW = (int)(bitmap.Height * dstRatio);
                    srcX = (bitmap.Width - srcW) / 2;
                    srcY = 0;
                }
                else
                {
                    srcW = bitmap.Width;
                    srcH = (int)(bitmap.Width / dstRatio);
                    srcX = 0;
                    srcY = (bitmap.Height - srcH) / 2;
                }

                srcRect = new SKRect(srcX, srcY, srcX + srcW, srcY + srcH);
                dstRect = new SKRect(0, 0, targetWidth, targetHeight);
            }
            else
            {
                srcRect = new SKRect(0, 0, bitmap.Width, bitmap.Height);
                dstRect = new SKRect(0, 0, targetWidth, targetHeight);
            }

            bool isUpscaling = targetWidth > bitmap.Width || targetHeight > bitmap.Height;
            using var paint = new SKPaint {
                FilterQuality = isUpscaling ? SKFilterQuality.None : SKFilterQuality.High
            };
            canvas.DrawBitmap(bitmap, srcRect, dstRect, paint);

            bitmap.Dispose();
            bitmap = resized;
        }

        if (bitmap.ColorType != SKColorType.Rgba8888)
        {
            var converted = bitmap.Copy(SKColorType.Rgba8888);
            bitmap.Dispose();
            bitmap = converted;
        }

        return bitmap;
    }
}

public static class ImageSaver
{
    static (int x, int y)[] BuildSectionMap(
        int sectionNumber, int partialImageSize,
        int imageWidth, int imageHeight)
    {
        int blockWidth = Math.Min(imageWidth, 128);
        int blockHeight = Math.Min(imageHeight, 64);
        int subBlocksPerRow = blockWidth / 16;

        var map = new (int x, int y)[partialImageSize];

        for (int i = 0; i < partialImageSize; i++)
        {
            int block = i / 32;
            int blockHoriz = 16 * (block % subBlocksPerRow);
            int blockRow = block / subBlocksPerRow;
            int blockVert = (2 * blockRow) - (blockRow % 2);
            int subBlock = i % 4;
            int blockOffset = (block / 16) % 2;
            int subBlockInd = (4 * (blockOffset + subBlock) + (i / 4)) % 8;

            int x = blockHoriz + 8 * (subBlock / 2) + subBlockInd
                    + blockWidth * (sectionNumber % (imageWidth / blockWidth));
            int y = blockVert + 2 * (subBlock % 2)
                              + blockHeight * (sectionNumber / (imageWidth / blockWidth));

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
        byte tg = (byte)(targetArgb >> 8);
        byte tb = (byte)targetArgb;

        int best = 0;
        long bestDist = long.MaxValue;

        for (int i = 0; i < palette.Length; i++)
        {
            long dr = tr - (byte)(palette[i] >> 16);
            long dg = tg - (byte)(palette[i] >> 8);
            long db = tb - (byte)palette[i];
            long dist = dr * dr + dg * dg + db * db;

            if (dist == 0)
            {
                best = i;
                break;
            }
            if (dist < bestDist)
            {
                bestDist = dist;
                best = i;
            }
        }

        cache[targetArgb] = best;
        return best;
    }


    public static byte[] SaveImageToBytes(byte[] originalData, ImageMrgFile imageFile, SKBitmap? bitmap = null, string? pngPath = null)
    {
        return imageFile switch {
            ImageMrgFile.PicMini => SavePicMiniToBytes(originalData, bitmap, pngPath),
            _ => SaveStandardImageToBytes(originalData, bitmap, pngPath)
        };
    }

    static byte[] SaveStandardImageToBytes(byte[] originalData, SKBitmap? bitmap, string? pngPath)
    {
        const int paddingSize = 0x60;

        int totalImageSize = BitConverter.ToInt32(originalData, 0x8);
        int imageWidth = BitConverter.ToUInt16(originalData, 0x54);
        int imageHeight = BitConverter.ToUInt16(originalData, 0x56);
        int numberOfSections = BitConverter.ToUInt16(originalData, 0x58);
        if (numberOfSections > 100) numberOfSections = 100;

        var (paletteOffset, paletteSize) = FindPaletteStartAndSize(originalData, totalImageSize, paddingSize);
        if (paletteOffset == -1) throw new InvalidDataException("Could not locate palette in source data.");
        if (paletteSize != 0x200 && paletteSize != 0x400) throw new InvalidDataException($"Unsupported palette size 0x{paletteSize:X}.");

        int startOfImage = paletteOffset + paletteSize + paddingSize;
        int partialImageSize = GetPartialImageSize(originalData, totalImageSize, startOfImage, paddingSize);
        uint[] palette = ReadPalette(originalData, paletteOffset, paletteSize);

        SKBitmap src = LoadSourceBitmap(bitmap, pngPath, imageWidth, imageHeight);
        bool disposeSrc = !ReferenceEquals(src, bitmap);

        var pixels = ReadPixels(src, imageWidth, imageHeight);
        if (disposeSrc) src.Dispose();

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
                //Not sure if nearest colour is still needed?
                uint argb = pixels[px + py * imageWidth];
                int nearestIdx = FindNearestPaletteIndex(argb, palette, colourCache);
               
                output[sectionOffset + i] = (byte)InverseConvertPlace(nearestIdx, palette.Length);
            }
        }

        return output;
    }

    static byte[] SavePicMiniToBytes(byte[] originalData, SKBitmap? bitmap, string? pngPath)
    {
        const int paletteOffset = 0x120;
        const int paletteSize = 0x200;
        const int startOfImage = 0x380;
        const int imageWidth = 40;
        const int imageHeight = 32;

        uint[] palette = ReadPalette(originalData, paletteOffset, paletteSize);
        SKBitmap src = LoadSourceBitmap(bitmap, pngPath, imageWidth, imageHeight);
        bool disposeSrc = !ReferenceEquals(src, bitmap);

        var pixels = ReadPixels(src, imageWidth, imageHeight);
        if (disposeSrc) src.Dispose();

        var colourCache = new Dictionary<uint, int>();
        byte[] output = (byte[])originalData.Clone();

        int pixelCount = imageWidth * imageHeight;
        for (int i = 0; i < pixelCount; i++)
        {
            int nearestIdx = FindNearestPaletteIndex(pixels[i], palette, colourCache);
            output[startOfImage + i] = (byte)InverseConvertPlace(nearestIdx, palette.Length);
        }

        return output;
    }

    static SKBitmap LoadSourceBitmap(SKBitmap? bitmap, string? pngPath, int expectedWidth, int expectedHeight)
    {
        SKBitmap src;
        if (bitmap != null)
        {
            src = bitmap.ColorType != SKColorType.Bgra8888 ? bitmap.Copy(SKColorType.Bgra8888) : bitmap;
        }
        else if (pngPath != null)
        {
            src = SKBitmap.Decode(pngPath) ?? throw new FileNotFoundException($"Could not load PNG: {pngPath}");
        }
        else
        {
            throw new ArgumentException("Either png or bitmap must be provided.");
        }


        if (src.Width != expectedWidth || src.Height != expectedHeight)
        {
            throw new InvalidDataException($"Image size {src.Width}x{src.Height} doesn't match target {expectedWidth}x{expectedHeight}.");
        }
        
        return src;
    }

    static uint[] ReadPixels(SKBitmap src, int width, int height)
    {
        var pixels = new uint[width * height];
        unsafe
        {
            uint* p = (uint*)src.GetPixels().ToPointer();
            for (int i = 0; i < pixels.Length; i++)
                pixels[i] = p[i];
        }
        return pixels;
    }


    static (int paletteOffset, int paletteSize) FindPaletteStartAndSize(byte[] data, int totalImageSize, int paddingSize)
    {
        var span = data.AsSpan(0, totalImageSize);
        var pattern = ImageCreator.endOfSectionSig.AsSpan();
        int? paletteOffset = null;
        int pos = 0;

        while (pos < span.Length)
        {
            int idx = span[pos..].IndexOf(pattern);
            if (idx == -1) break;
            idx += pos;

            if (paletteOffset == null)
                paletteOffset = idx + ImageCreator.endOfSectionSig.Length;
            else
            {
                int paletteEnd = idx + ImageCreator.endOfSectionSig.Length;
                return (paletteOffset.Value, paletteEnd - paletteOffset.Value - paddingSize);
            }
            pos = idx + ImageCreator.endOfSectionSig.Length;
        }
        return (-1, -1);
    }

    static int GetPartialImageSize(byte[] data, int totalImageSize, int startOfImage, int paddingSize)
    {
        var span = data.AsSpan(startOfImage, totalImageSize - startOfImage);
        var pattern = ImageCreator.endOfSectionSig.AsSpan();
        int pos = 0;

        while (pos < span.Length)
        {
            int idx = span[pos..].IndexOf(pattern);
            if (idx == -1) break;
            idx += pos;

            int address = startOfImage + idx;
            if (address % 16 == 6)
                return address + ImageCreator.endOfSectionSig.Length - startOfImage - paddingSize;

            pos = idx + ImageCreator.endOfSectionSig.Length;
        }
        return totalImageSize - startOfImage;
    }

    static uint[] ReadPalette(byte[] data, int paletteOffset, int paletteSize)
    {
        if (paletteSize == 0x400)
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
        else // 0x200, maybe do a switch later
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


    static int InverseConvertPlace(int targetIndex, int paletteSize)
    {
        for (int raw = 0; raw < paletteSize; raw++)
        {
            int placeType = (raw % 32) / 8;
            int adjusted = raw;
            if (placeType == 1) adjusted += 8;
            else if (placeType == 2) adjusted -= 8;
            if (adjusted % paletteSize == targetIndex)
                return raw;
        }
        return targetIndex;
    }


    public static byte[] SavePaletteToBytes(byte[] originalData)
    {
        const int paddingSize = 0x60;

        int totalImageSize = BitConverter.ToInt32(originalData, 0x8);
        var (paletteOffset, paletteSize) = FindPaletteStartAndSize(originalData, totalImageSize, paddingSize);
        if (paletteOffset == -1) throw new InvalidDataException("Could not locate palette.");
        if (paletteSize != 0x200 && paletteSize != 0x400) throw new InvalidDataException($"Unsupported palette size 0x{paletteSize:X}.");

        byte[] output = (byte[])originalData.Clone();
        var palette = GameImageManager.CurrentTexture.Palette;

        if (paletteSize == 0x400)
        {
            for (int i = 0; i < palette.Length; i++)
            {
                byte r = (byte)(palette[i] >> 16);
                byte g = (byte)(palette[i] >> 8);
                byte b = (byte)(palette[i]);
                int offset = paletteOffset + i * 4;
                output[offset + 0] = r;
                output[offset + 1] = g;
                output[offset + 2] = b;
                output[offset + 3] = 0xFF;
            }
        }
        else // 0x200 Todo switch later
        {
            for (int i = 0; i < palette.Length; i++)
            {
                byte r = (byte)(palette[i] >> 16);
                byte g = (byte)(palette[i] >> 8);
                byte b = (byte)(palette[i]);

                byte r5 = (byte)((r * 31 + 127) / 255);
                byte g5 = (byte)((g * 31 + 127) / 255);
                byte b5 = (byte)((b * 31 + 127) / 255);

                ushort rgb555 = (ushort)(r5 | (g5 << 5) | (b5 << 10));
                int offset = paletteOffset + i * 2;
                output[offset + 0] = (byte)(rgb555 & 0xFF);
                output[offset + 1] = (byte)(rgb555 >> 8);
            }
        }

        return output;
    }
}