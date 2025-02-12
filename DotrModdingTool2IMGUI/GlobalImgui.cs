using System.Numerics;
using ImGuiNET;
namespace DotrModdingTool2IMGUI;

public class GlobalImgui
{
    public static bool ShowImageHighlight = true;
    public static Vector4 defaultColor = new GuiColour(20, 20, 20, 240).value;

    public static void RenderTooltipCardImage(string cardName)
    {
        if (!ShowImageHighlight)
            return;
        ImGui.PushStyleColor(ImGuiCol.PopupBg, defaultColor);
        ImGui.BeginTooltip();
        ImGui.Text("Card Preview");
        float tooltipWidth = ImGui.GetWindowSize().X;

        ImGui.SetCursorPosX((tooltipWidth - 128) * 0.5f);
        ImGui.Image(GlobalImages.Instance.Cards[cardName], new Vector2(128, 128));
        ImGui.EndTooltip();
        ImGui.PopStyleColor();

    }

    public static void RenderTooltipOpponentImage(EEnemyImages enemyImage)
    {
        if (!ShowImageHighlight)
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
        if (!ShowImageHighlight)
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
}