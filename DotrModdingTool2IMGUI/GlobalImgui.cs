using System.Numerics;
using ImGuiNET;
using Raylib_cs;
namespace DotrModdingTool2IMGUI;

public class GlobalImgui
{
    public static Vector4 defaultColor = new GuiColour(20, 20, 20, 240).value;

    public static void RenderTooltipCardImage(string cardName)
    {
        if (!UserSettings.ToggleImageTooltips)
            return;
        if (!GlobalImages.Instance.Cards.TryGetValue(cardName, out var texture))
            return;
        ImGui.PushStyleColor(ImGuiCol.PopupBg, defaultColor);

        ImGui.BeginTooltip();
        ImGui.Text("Card Preview");
        float tooltipWidth = ImGui.GetWindowSize().X;

        ImGui.SetCursorPosX((tooltipWidth - ImageHelper.DefaultImageSize.X) * 0.5f);
        ImGui.Image(texture, ImageHelper.DefaultImageSize);
        ImGui.EndTooltip();
        ImGui.PopStyleColor();

    }

    public static void RenderTooltipOpponentImage(EEnemyImages enemyImage)
    {
        if (!UserSettings.ToggleImageTooltips)
            return;
        ImGui.PushStyleColor(ImGuiCol.PopupBg, defaultColor);
        ImGui.BeginTooltip();
        ImGui.Text("Enemy Preview");
        float tooltipWidth = ImGui.GetWindowSize().X;
        ImGui.SetCursorPosX((tooltipWidth - 128) * 0.5f);
        ImGui.Image(GlobalImages.Instance.Enemies[enemyImage], new Vector2(128, 128));
        ImGui.EndTooltip();
        ImGui.PopStyleColor();
    }

    public static void RenderTooltipRankImage(DeckLeaderRank leaderRank, int xSize = 64, int ySize = 64)
    {
        if (!UserSettings.ToggleImageTooltips)
            return;
        ImGui.PushStyleColor(ImGuiCol.PopupBg, defaultColor);
        ImGui.BeginTooltip();
        ImGui.Text("Rank Preview");
        float tooltipWidth = ImGui.GetWindowSize().X;
        ImGui.SetCursorPosX((tooltipWidth - xSize) * 0.5f);
        ImGui.Image(GlobalImages.Instance.LeaderRanks[leaderRank], new Vector2(xSize, ySize));
        ImGui.EndTooltip();
        ImGui.PopStyleColor();
    }

    public static void CardEditorCombo<T>(string label, ref int currentIndex, T[] values, Action<int> applyChange)
    {
        string previewText = (currentIndex >= 0 && currentIndex < values.Length)
            ? values[currentIndex]?.ToString()
            : values[values.Length - 1].ToString();
        if (ImGui.BeginCombo(label, previewText))
        {
            for (int i = 0; i < values.Length; i++)
            {
                bool isSelected = (currentIndex == i);
                if (ImGui.Selectable(values[i]?.ToString(), isSelected))
                {
                    currentIndex = i;
                    applyChange(i);
                }

                if (isSelected)
                {
                    ImGui.SetItemDefaultFocus();
                }
            }
            ImGui.EndCombo();
        }
    }

    public static void AutoWrappingButton(string label, Action onClick, ref float lineWidth, float availableSpace, bool disabled = false)
    {
        ImGui.PushFont(FontManager.GetBestFitFont("Make Current Map Default", false, FontManager.FontFamily.NotoSansJP));
        float padding = ImGui.GetStyle().FramePadding.X * 2;
        float spaceToUse = ImGui.CalcTextSize(label).X + padding + ImGui.GetStyle().ItemSpacing.X;

        if (lineWidth + spaceToUse > availableSpace && lineWidth > 0)
        {
            lineWidth = 0;
        }
        else if (lineWidth > 0)
        {
            ImGui.SameLine();
        }

        if (disabled)
        {
            ImGui.BeginDisabled();
        }

        if (ImGui.Button(label))
        {
            onClick?.Invoke();
        }

        if (disabled)
        {
            ImGui.EndDisabled();
        }

        lineWidth += spaceToUse;
        ImGui.PopFont();
    }
}