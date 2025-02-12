using System.Numerics;
using ImGuiNET;
namespace DotrModdingTool2IMGUI;

public class ImGuiModalPopup
{
    public bool showErrorPopup;
    string errorMessage;
    string messageTitle;


    public ImGuiModalPopup()
    {
        showErrorPopup = false;
        errorMessage = string.Empty;
    }

    public void Show(string message, string MessageTitle = "Error")
    {
        messageTitle = MessageTitle;
        errorMessage = message;
        showErrorPopup = true;


    }


    public void Draw(ImFontPtr imFontPtr)
    {
        if (showErrorPopup)
        {
            ImGui.OpenPopup(messageTitle);
            Vector2 center = ImGui.GetMainViewport().GetCenter();
            ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            if (ImGui.BeginPopupModal(messageTitle, ref showErrorPopup, ImGuiWindowFlags.Modal | ImGuiWindowFlags.AlwaysAutoResize))
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
            ImGui.OpenPopup(messageTitle);
            Vector2 center = ImGui.GetMainViewport().GetCenter();
            ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            if (ImGui.BeginPopupModal(messageTitle, ref showErrorPopup, ImGuiWindowFlags.Modal | ImGuiWindowFlags.AlwaysAutoResize))
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