using System.Numerics;
using ImGuiNET;
namespace DotrModdingTool2IMGUI;

public class ImGuiErrorPopup
{
    bool showErrorPopup;
    string errorMessage;


    public ImGuiErrorPopup()
    {
        showErrorPopup = false;
        errorMessage = string.Empty;
    }

    public void Show(string message,string messageTitle = "Error")
    {
        errorMessage = message;
        showErrorPopup = true;
        

    }


    public void Draw(ImFontPtr imFontPtr)
    {
        if (showErrorPopup)
        {
            ImGui.OpenPopup("Error");
            Vector2 center = ImGui.GetMainViewport().GetCenter();
            ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            if (ImGui.BeginPopupModal("Error", ref showErrorPopup, ImGuiWindowFlags.Modal | ImGuiWindowFlags.AlwaysAutoResize))
            {
                if (!imFontPtr.IsLoaded())
                {
                    Draw();
                    return;
                }
                ImGui.PushFont(imFontPtr);
                ImGui.Text(errorMessage);
                ImGui.Separator();
                if (ImGui.Button("OK"))
                {
                    showErrorPopup = false; // Close the popup
                    ImGui.CloseCurrentPopup();
                }
                ImGui.PopFont();
                ImGui.EndPopup();
            }
        }
    }

    public void Draw()
    {
        if (showErrorPopup)
        {
            ImGui.OpenPopup("Error");
            Vector2 center = ImGui.GetMainViewport().GetCenter();
            ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            if (ImGui.BeginPopupModal("Error", ref showErrorPopup, ImGuiWindowFlags.Modal | ImGuiWindowFlags.AlwaysAutoResize))
            {
                ImGui.SetWindowFontScale(1f);
                ImGui.Text(errorMessage);
                ImGui.Separator();
                if (ImGui.Button("OK"))
                {
                    ImGui.SetWindowFontScale(1f);
                    showErrorPopup = false;
                    ImGui.CloseCurrentPopup();
                }
                ImGui.EndPopup();
            }
        }
    }
}