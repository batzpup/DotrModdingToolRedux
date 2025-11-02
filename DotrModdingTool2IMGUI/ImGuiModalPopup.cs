using System.Numerics;
using ImGuiNET;
namespace DotrModdingTool2IMGUI;

public class ImGuiModalPopup
{
    public Action? callback;
    public bool showErrorPopup;
    ShowType showType;
    string errorMessage;
    string messageTitle;


    public ImGuiModalPopup()
    {
        showErrorPopup = false;
        errorMessage = string.Empty;
    }

    public enum ShowType
    {
        OneButton,
        YesNo,
        NoButton
    }

    public void Show(string message, string MessageTitle = "Error", Action? callback = null, ShowType type = ShowType.OneButton)
    {
        showType = type;
        this.callback = callback;
        messageTitle = MessageTitle;
        errorMessage = message;
        showErrorPopup = true;
    }

    public void Hide()
    {
        showErrorPopup = false;
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
                switch (showType)
                {
                    case ShowType.OneButton:
                        if (ImGui.Button("OK"))
                        {
                            showErrorPopup = false;
                            callback?.Invoke();
                            ImGui.CloseCurrentPopup();
                        }
                        break;
                    case ShowType.YesNo:
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
                        break;
                    case ShowType.NoButton:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }


                ImGui.EndPopup();
                ImGui.PopFont();
            }
        }
    }

    public void Draw()
    {
        Vector2 size = ImGui.GetWindowSize();
        ImGui.PushFont(FontManager.GetBestFitFont(errorMessage,size.X,size.Y));
        if (showErrorPopup)
        {
            ImGui.OpenPopup(messageTitle);
            Vector2 center = ImGui.GetMainViewport().GetCenter();
            ImGui.SetNextWindowPos(center, ImGuiCond.Appearing, new Vector2(0.5f, 0.5f));
            if (ImGui.BeginPopupModal(messageTitle, ref showErrorPopup, ImGuiWindowFlags.Modal | ImGuiWindowFlags.AlwaysAutoResize))
            {
                ImGui.Text(errorMessage);
                ImGui.Separator();
                switch (showType)
                {
                    case ShowType.OneButton:
                        if (ImGui.Button("OK"))
                        {
                            showErrorPopup = false;
                            callback?.Invoke();
                            ImGui.CloseCurrentPopup();
                        }
                        break;
                    case ShowType.YesNo:
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
                        break;
                    case ShowType.NoButton:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                ImGui.EndPopup();
            }
        }
        ImGui.PopFont();
    }
}