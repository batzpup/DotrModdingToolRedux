using System.Numerics;
using ImGuiNET;
namespace DotrModdingTool2IMGUI;

public class GeneralEditorWindow : IImGuiWindow
{
    public MapEditorWindow MapEditorWindow;
    public DeckEditorWindow DeckEditorWindow;
    public DestinyDrawEditorWindow DestinyDrawEditorWindow;
    public NameCalculatorWindow NameCalculatorWindow;

    int currentDeckListIndex;
    Deck currentDeck;
    
    public GeneralEditorWindow(ImFontPtr mapEditorFont)
    {
        MapEditorWindow = new MapEditorWindow(mapEditorFont);
        DeckEditorWindow = new DeckEditorWindow();
        DestinyDrawEditorWindow = new DestinyDrawEditorWindow();
        NameCalculatorWindow = new NameCalculatorWindow();
    }

    public void Render()
    {
        ImGui.PushFont(FontManager.GetFont(FontManager.FontFamily.NotoSansJP, 32));
        ImGui.PushStyleColor(ImGuiCol.TabSelected, new GuiColour(0, 189, 0).value);
        ImGui.PushStyleColor(ImGuiCol.TabHovered, new GuiColour(128, 128, 0).value);
        
        if (ImGui.BeginTabBar("CardEditorMode"))
        {
            if (ImGui.BeginTabItem("Deck Editor"))
            {
                DeckEditorWindow.Render();
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Map Editor"))
            {
                MapEditorWindow.Render();
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Destiny Draw Editor"))
            {
                DestinyDrawEditorWindow.Render();
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Starter Deck Calculator"))
            {
                NameCalculatorWindow.Render();
                ImGui.EndTabItem();
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