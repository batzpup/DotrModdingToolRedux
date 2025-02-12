using System.Numerics;
using ImGuiNET;
namespace DotrModdingTool2IMGUI;

public class ImGuiModalPopup
{
    public Action? callback;
    public bool showErrorPopup;
    bool yesNo = false;
    string errorMessage;
    string messageTitle;


    public ImGuiModalPopup()
    {
        showErrorPopup = false;
        errorMessage = string.Empty;
    }

    public void Show(string message, string MessageTitle = "Error", Action? callback = null, bool YesNo = false)
    {
        yesNo = YesNo;
        this.callback = callback;
        messageTitle = MessageTitle;
        errorMessage = message;
        showErrorPopup = true;
    }


    public void Draw(ImFontPtr imFontPtr)
    {
        ImGui.PushFont(imFontPtr);
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

                ImGui.Text(errorMessage);
                ImGui.Separator();
                if (yesNo)
                {
                    Vector2 buttonSize = new Vector2(ImGui.CalcTextSize("YESNO").X, ImGui.CalcTextSize("YESNO").X * 0.75f);
                    if (ImGui.Button("Yes", buttonSize))
                    {
                        showErrorPopup = false;
                        callback?.Invoke();
                        ImGui.CloseCurrentPopup();
                    }
                    ImGui.SameLine();
                    if (ImGui.Button("No", buttonSize))
                    {
                        showErrorPopup = false;
                        ImGui.CloseCurrentPopup();
                    }
                }
                else
                {
                    if (ImGui.Button("OK"))
                    {
                        showErrorPopup = false;
                        callback?.Invoke();
                        ImGui.CloseCurrentPopup();
                    }
                }


                ImGui.EndPopup();
                ImGui.PopFont();
            }
        }
    }

    public void Draw()
    {
        if (showErrorPopup)
        {
            ImGui.PushFont(Fonts.MonoSpace);
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
            ImGui.PopFont();
        }
    }
}