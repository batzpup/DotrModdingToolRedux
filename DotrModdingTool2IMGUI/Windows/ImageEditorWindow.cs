using ImGuiNET;
using SkiaSharp;
namespace DotrModdingTool2IMGUI;

class ImageEditorWindow : IImGuiWindow
{
    TextureEditorWindow textureEditorWindow = new TextureEditorWindow();
    PreloadImageEditorWindow preloadImageEditorWindow = new();


    public void Render()
    {
        
        ImGui.PushStyleColor(ImGuiCol.TabSelected, new GuiColour(0, 189, 0).value);
        ImGui.PushStyleColor(ImGuiCol.TabHovered, new GuiColour(128, 128, 0).value);
        
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
        
        ImGui.PopStyleColor(2);

    }

    public void Free()
    {

    }
}