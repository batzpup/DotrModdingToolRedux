using System.Reflection;
using ImGuiNET;
using rlImGui_cs;
namespace DotrModdingTool2IMGUI;

public static class Fonts
{
    public static ImFontPtr MonoSpace = LoadCustomFont();
    public static ImFontPtr JapaneseFont = LoadCustomFont("NotoSansJP-Regular.ttf", 32, ImGui.GetIO().Fonts.GetGlyphRangesJapanese());

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
                unsafe
                {
                    fixed (byte* p = fontData)
                    {
                        IntPtr ptr = (IntPtr)p;
                        if (glyphRanges.HasValue)
                            font = ImGui.GetIO().Fonts.AddFontFromMemoryTTF(ptr, bytesRead, pixelSize, null, glyphRanges.Value);
                        else
                            font = ImGui.GetIO().Fonts.AddFontFromMemoryTTF(ptr, bytesRead, pixelSize);
                    }
                }
            }
            else
            {
                font = ImGui.GetIO().Fonts.AddFontDefault();
                font.ConfigData.SizePixels = pixelSize;
            }
        }
        ImGui.GetIO().Fonts.Build();
        rlImGui.ReloadFonts();
        return font;
    }
}