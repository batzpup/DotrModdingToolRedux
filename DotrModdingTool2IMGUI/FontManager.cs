using System.Collections;
using System.Numerics;
using System.Reflection;
using System.Runtime.InteropServices;
using ImGuiNET;
using rlImGui_cs;
namespace DotrModdingTool2IMGUI;

public static class FontManager
{
    public static Dictionary<FontFamily, Dictionary<int, ImFontPtr>> Fonts = new();

    public enum FontFamily
    {
        NotoSansJP,
        SpaceMono
    }

    public static Dictionary<FontFamily, string> FontNames = new() {
        { FontFamily.NotoSansJP, "NotoSansJP-Regular.ttf" },
        { FontFamily.SpaceMono, "SpaceMonoRegular-JRrmm.ttf" }
    };

    public static void LoadFonts()
    {
        int[] sizes = { 10, 12, 14, 16, 18, 20, 22, 24, 26, 28, 30, 32 };
        foreach (var kvp in FontNames)
        {

            var sizeDict = new Dictionary<int, ImFontPtr>();
            foreach (int size in sizes)
            {
                ImFontPtr font = LoadCustomFont(kvp.Value, size);
                sizeDict[size] = font;
            }
            Fonts[kvp.Key] = sizeDict;
        }
        ImGui.GetIO().Fonts.Build();
        rlImGui.ReloadFonts();
    }

    public static ImFontPtr GetFont(FontFamily family, int requestedSize)
    {
        if (!Fonts.TryGetValue(family, out var dict))
            return IntPtr.Zero;

        int closest = dict.Keys.OrderBy(s => Math.Abs(s - requestedSize)).First();
        return dict[closest];
    }

    public static ImFontPtr GetBestFitFont(string text, bool useWindowSize = false, FontFamily fontName = FontFamily.NotoSansJP)
    {
        unsafe
        {
            Vector2 size = useWindowSize ? ImGui.GetWindowSize() : ImGui.GetContentRegionAvail();
            if (!Fonts.TryGetValue(fontName, out var fontSizes) || fontSizes.Count == 0)
                throw new Exception($"Font '{fontName}' not loaded!");

            ImFontPtr bestFont = default;
            int bestSize = 0;

            size -= ImGui.GetStyle().FramePadding * 2;


            ImFontPtr originalFont = ImGui.GetFont();

            foreach (var kvp in fontSizes.OrderBy(x => x.Key))
            {
                ImFontPtr font = kvp.Value;

                ImGui.PushFont(font);
                Vector2 textSize = ImGui.CalcTextSize(text);
                ImGui.PopFont();

                if (textSize.X <= size.X && textSize.Y <= size.Y)
                {
                    bestFont = font;
                    bestSize = kvp.Key;
                }
                else if (kvp.Key > bestSize)
                {
                    break;
                }
            }

            // Restore the original font
            ImGui.PushFont(originalFont);
            ImGui.PopFont();

            if (bestFont.NativePtr == (void*)IntPtr.Zero)
            {
                bestFont = fontSizes.OrderBy(x => x.Key).First().Value;
                Console.WriteLine($"No font fits! Using smallest font (size {fontSizes.Keys.Min()})");
            }

            return bestFont;
        }
    }

    public static ImFontPtr GetBestFitFont(string text, float maxWidth, float maxHeight, FontFamily fontName = FontFamily.SpaceMono)
    {
        unsafe
        {
            if (!Fonts.TryGetValue(fontName, out var fontSizes) || fontSizes.Count == 0)
                throw new Exception($"Font '{fontName}' not loaded!");

            ImFontPtr bestFont = default;

            foreach (var kvp in fontSizes)
            {
                ImFontPtr font = kvp.Value;

                ImGui.PushFont(font);
                Vector2 textSize = ImGui.CalcTextSize(text);
                ImGui.PopFont();

                if (textSize.X <= maxWidth && textSize.Y <= maxHeight)
                    bestFont = font;
                else
                    break;
            }


            if (bestFont.NativePtr == (void*)IntPtr.Zero)
                bestFont = fontSizes.Values.First();

            return bestFont;
        }
    }


    public static ImFontPtr LoadCustomFont(string fontPath = "SpaceMonoRegular-JRrmm.ttf", int pixelSize = 32, IntPtr? glyphRanges = null)
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string resourceName = $"{assembly.GetName().Name}.Fonts.{fontPath}";
        //Console.WriteLine($"Loading resource: {resourceName}");
        ImFontPtr font;
        using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
        {

            if (stream is null)
            {
                Console.Error.WriteLine($"No resource exists with the name {resourceName}");
                font = ImGui.GetIO().Fonts.AddFontDefault();
                font.ConfigData.SizePixels = pixelSize;
                return font;
            }
            byte[] fontData = new byte[stream.Length];
            int bytesRead = stream.Read(fontData);
            if (bytesRead == stream.Length)
            {
                // Allocate unmanaged memory that persists for the application lifetime
                IntPtr unmanagedPtr = Marshal.AllocHGlobal(bytesRead);
                Marshal.Copy(fontData, 0, unmanagedPtr, bytesRead);

                if (glyphRanges.HasValue)
                    font = ImGui.GetIO().Fonts.AddFontFromMemoryTTF(unmanagedPtr, bytesRead, pixelSize, null, glyphRanges.Value);
                else
                    font = ImGui.GetIO().Fonts.AddFontFromMemoryTTF(unmanagedPtr, bytesRead, pixelSize);
            }
            else
            {
                font = ImGui.GetIO().Fonts.AddFontDefault();
                font.ConfigData.SizePixels = pixelSize;
            }
        }
        return font;
    }
}