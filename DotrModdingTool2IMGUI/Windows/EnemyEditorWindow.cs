using System.Numerics;
using ImGuiNET;
namespace DotrModdingTool2IMGUI;

public class EnemyEditorWindow : IImGuiWindow
{
    public MapEditorWindow MapEditorWindow;
    public DeckEditorWindow DeckEditorWindow;
    DataAccess _dataAccess;
    ImFontPtr DeckEditorFont;


    public EnemyEditorWindow(ImFontPtr mapEditorFont, ImFontPtr deckEditorFont)
    {
        _dataAccess = DataAccess.Instance;
        this.DeckEditorFont = deckEditorFont;
        MapEditorWindow = new MapEditorWindow(mapEditorFont);
        DeckEditorWindow = new DeckEditorWindow(deckEditorFont);
    }

    public void Render()
    {
        ImGui.PushFont(Fonts.MonoSpace);
        ImGui.PushStyleColor(ImGuiCol.TabSelected, new GuiColour(0, 189, 0).value);
        ImGui.PushStyleColor(ImGuiCol.TabHovered, new GuiColour(128, 128, 0).value);
        if (ImGui.BeginTabBar("CardEditorMode"))
        {
            if (ImGui.BeginTabItem("Deck Editor"))
            {
                ImGui.PushFont(DeckEditorFont);
                DeckEditorWindow.Render();
                ImGui.EndTabItem();
                ImGui.PopFont();
            }
            if (ImGui.BeginTabItem("Map Editor"))
            {
                ImGui.PushFont(Fonts.MonoSpace);
                MapEditorWindow.Render();
                ImGui.EndTabItem();
                ImGui.PopFont();
            }
            ImGui.EndTabBar();
        }
        ImGui.PopFont();
        ImGui.PopStyleColor(2);

    }

    public void Free()
    {

    }
}