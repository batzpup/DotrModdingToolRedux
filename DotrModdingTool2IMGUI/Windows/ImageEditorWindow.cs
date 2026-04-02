using ImGuiNET;
using SkiaSharp;
namespace DotrModdingTool2IMGUI;

class ImageEditorWindow : IImGuiWindow
{
    TextureEditorWindow textureEditorWindow = new TextureEditorWindow();
    PreloadImageEditorWindow preloadImageEditorWindow = new();


    public void Render()
    {
        if (!DataAccess.Instance.IsIsoLoaded)
        {
            return;
        }
        if (ImGui.BeginTabBar("ImageEditorTabBar"))
        {
            if (ImGui.BeginTabItem("Image Texture Editor"))
            {
                textureEditorWindow.Render();
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Preloaded Image Editor"))
            {
                preloadImageEditorWindow.Render();
                ImGui.EndTabItem();
            }


            ImGui.EndTabBar();
        }



    }

    public void Free()
    {

    }
}