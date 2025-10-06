using System.Numerics;
using ImGuiNET;
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

        ImGui.SetCursorPosX((tooltipWidth - 128) * 0.5f);
        ImGui.Image(texture, new Vector2(128, 128));
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
}