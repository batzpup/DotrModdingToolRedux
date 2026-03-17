using System.Numerics;
using ImGuiNET;
namespace DotrModdingTool2IMGUI;

public class EnemyEditorWindow : IImGuiWindow
{
    public MapEditorWindow MapEditorWindow;
    public DeckEditorWindow DeckEditorWindow;
    public NameCalculatorWindow NameCalculatorWindow;
    DataAccess _dataAccess;


    int currentDeckListIndex;
    Deck currentDeck;
    public EnemyEditorWindow(ImFontPtr mapEditorFont)
    {
        _dataAccess = DataAccess.Instance;
        MapEditorWindow = new MapEditorWindow(mapEditorFont);
        DeckEditorWindow = new DeckEditorWindow();
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
                ImGui.PushFont(FontManager.GetFont(FontManager.FontFamily.NotoSansJP, 32));
                MapEditorWindow.Render();
                ImGui.EndTabItem();
                ImGui.PopFont();
            }
            if (ImGui.BeginTabItem("Name Calculator"))
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