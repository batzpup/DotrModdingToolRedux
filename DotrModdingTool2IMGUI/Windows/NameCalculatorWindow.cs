using System.Numerics;
using ImGuiNET;
namespace DotrModdingTool2IMGUI;

public class NameCalculatorWindow : IImGuiWindow
{
    public NameCalculatorWindow()
    {
        EditorWindow.OnIsoLoaded += onIsoLoaded;
    }

    static Dictionary<char, int> characterMap = new Dictionary<char, int> {
        { ',', 0x002 }, { 'A', 0x056 }, { 'B', 0x057 }, { 'C', 0x058 }, { 'D', 0x059 },
        { 'E', 0x05A }, { 'F', 0x05B }, { 'G', 0x05C }, { 'H', 0x05D }, { 'I', 0x05E },
        { 'J', 0x05F }, { 'K', 0x060 }, { 'L', 0x061 }, { 'M', 0x062 }, { 'N', 0x063 },
        { 'O', 0x064 }, { 'P', 0x065 }, { 'Q', 0x066 }, { 'R', 0x067 }, { 'S', 0x068 },
        { 'T', 0x069 }, { 'U', 0x06A }, { 'V', 0x06B }, { 'W', 0x06C }, { 'X', 0x06D },
        { 'Y', 0x06E }, { 'Z', 0x06F }, { ' ', 0x070 }, { '[', 0x071 }, { ']', 0x072 },
        { 'a', 0x073 }, { 'b', 0x074 }, { 'c', 0x075 }, { 'd', 0x076 }, { 'e', 0x077 },
        { 'f', 0x078 }, { 'g', 0x079 }, { 'h', 0x07A }, { 'i', 0x07B }, { 'j', 0x07C },
        { 'k', 0x07D }, { 'l', 0x07E }, { 'm', 0x07F }, { 'n', 0x080 }, { 'o', 0x081 },
        { 'p', 0x082 }, { 'q', 0x083 }, { 'r', 0x084 }, { 's', 0x085 }, { 't', 0x086 },
        { 'u', 0x087 }, { 'v', 0x088 }, { 'w', 0x089 }, { 'x', 0x08A }, { 'y', 0x08B },
        { 'z', 0x08C }, { '(', 0x08D }, { ')', 0x08E }, { '1', 0x08F }, { '2', 0x090 },
        { '3', 0x091 }, { '4', 0x092 }, { '5', 0x093 }, { '6', 0x094 }, { '7', 0x095 },
        { '8', 0x096 }, { '9', 0x097 }, { '0', 0x098 }, { '!', 0x099 }, { '"', 0x09A },
        { '#', 0x09B }, { '$', 0x09C }, { '%', 0x09D }, { '&', 0x09E }, { '\'', 0x09F },
        { '=', 0x0A0 }, { '^', 0x0A1 }, { '-', 0x0A2 }, { '¥', 0x0A3 }, { '.', 0x0A4 },
        { '/', 0x0A5 }, { '_', 0x0A6 }, { 'α', 0x169 }, { '<', 0x16A }, { '>', 0x16B },
        { '△', 0x16D }, { '□', 0x16E }, { '×', 0x16F }, { ';', 0x170 },
    };

    static readonly ushort[] DeckTab = new ushort[] {
        0x00AA, // index 0
        0x04CB, // index 1
        0x08EC, // index 2
        0x0D2D, // index 3
        0x114E, // index 4
        0x052B, // index 5
        0x0CCD, // index 6
        0x08AC, // index 7
        0x00EA, // index 8
        0x04AB, // index 9
        0x08CC, // index 10
        0x0C8D, // index 11
        0x41A9, // index 12
        0x316F, // index 13
        0x09EA, // index 14
        0x0107, // index 15
    };

    string nameInput = String.Empty;
    int paddingValue = 14;
    int currentValue = 0;
    int[] resultDeckIndices = null;
    List<DeckCard>[] sortedDeckList = new List<DeckCard>[3];

    void onIsoLoaded()
    {

    }

    public void UpdateDeckData()
    {
        for (int i = 0; i < 3; i++)
        {
            sortedDeckList[i] = new List<DeckCard>(Deck.DeckList[resultDeckIndices[i]].CardList);
        }

    }

    public unsafe void Render()
    {
        if (!DataAccess.Instance.IsIsoLoaded)
        {
            ImGui.Text("Please load ISO file");
            return;
        }
        ImGui.Text("Name");
        ImGui.SetNextItemWidth(ImGui.CalcTextSize("AAAAAAAAAAAAA").X);
        if (ImGui.InputText("##Name", ref nameInput, 12, ImGuiInputTextFlags.CallbackCharFilter, FilterNameInput))
        {
            currentValue = 0;


            foreach (var character in nameInput)
            {
                currentValue += characterMap[character] % 16;
            }
            currentValue += (12 - nameInput.Length) * 14;
            currentValue = currentValue % 16;
            resultDeckIndices = GetStarterDeckPool(currentValue);
            UpdateDeckData();
        }
        if (resultDeckIndices != null)
        {
            float availableHeight = ImGui.GetContentRegionAvail().Y;
            ImGui.BeginChild("Deck1", new Vector2(ImGui.GetContentRegionAvail().X / 3f, availableHeight), ImGuiChildFlags.Border | ImGuiChildFlags.AlwaysAutoResize);

            ImGui.SetCursorPosX((ImGui.GetContentRegionAvail().X - ImGui.CalcTextSize(Deck.DeckList[resultDeckIndices[2]].ToString()).X) / 2f);
            ImGui.Text(Deck.DeckList[resultDeckIndices[0]].ToString());
           
            ImGui.SetCursorPosX((ImGui.GetContentRegionAvail().X - ImageHelper.DefaultImageSize.X) / 2f);
            ImGui.Image(GlobalImages.Instance.Cards[Deck.DeckList[resultDeckIndices[0]].DeckLeader.Name.Default], ImageHelper.DefaultImageSize);
           
            RenderDeckTable(0);
            ImGui.EndChild();

            ImGui.SameLine();
            ImGui.BeginChild("Deck2", new Vector2(ImGui.GetContentRegionAvail().X / 2, availableHeight), ImGuiChildFlags.Border | ImGuiChildFlags.AlwaysAutoResize);


            ImGui.SetCursorPosX((ImGui.GetContentRegionAvail().X - ImGui.CalcTextSize(Deck.DeckList[resultDeckIndices[2]].ToString()).X) / 2f);
            ImGui.Text(Deck.DeckList[resultDeckIndices[1]].ToString());
          
            ImGui.SetCursorPosX((ImGui.GetContentRegionAvail().X - ImageHelper.DefaultImageSize.X) / 2f);
            ImGui.Image(GlobalImages.Instance.Cards[Deck.DeckList[resultDeckIndices[1]].DeckLeader.Name.Default], ImageHelper.DefaultImageSize);


            RenderDeckTable(1);
            ImGui.EndChild();

            ImGui.SameLine();
            ImGui.BeginChild("Deck3", new Vector2(ImGui.GetContentRegionAvail().X, availableHeight), ImGuiChildFlags.Border | ImGuiChildFlags.AlwaysAutoResize);

            ImGui.SetCursorPosX((ImGui.GetContentRegionAvail().X - ImGui.CalcTextSize(Deck.DeckList[resultDeckIndices[2]].ToString()).X) / 2f);
            ImGui.Text(Deck.DeckList[resultDeckIndices[2]].ToString());
            
            ImGui.SetCursorPosX((ImGui.GetContentRegionAvail().X - ImageHelper.DefaultImageSize.X) / 2f);
            ImGui.Image(GlobalImages.Instance.Cards[Deck.DeckList[resultDeckIndices[2]].DeckLeader.Name.Default], ImageHelper.DefaultImageSize);
            
            RenderDeckTable(2);
            ImGui.EndChild();







        }

    }


    ImGuiTableFlags tableFlags = ImGuiTableFlags.Resizable | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Hideable | ImGuiTableFlags.Sortable |
                                 ImGuiTableFlags.SortMulti | ImGuiTableFlags.BordersV | ImGuiTableFlags.RowBg | ImGuiTableFlags.Borders |
                                 ImGuiTableFlags.ScrollX |
                                 ImGuiTableFlags.ScrollY | ImGuiTableFlags.SizingStretchSame | ImGuiTableFlags.BordersInnerH;

    void RenderDeckTable(int deckIndex)
    {
        ImGui.PushFont(FontManager.GetFont(FontManager.FontFamily.NotoSansJP, 20));
        if (ImGui.BeginTable("CurrentDeck", 8, tableFlags))
        {
            unsafe
            {
                float idWidth = ImGui.CalcTextSize("999").X;
                float nameWidth = ImGui.CalcTextSize("Winged Dragon, Guardian of the Fortress #1").X;
                float attackWidth = ImGui.CalcTextSize("9999").X;
                float levelWidth = ImGui.CalcTextSize("LVL").X;
                float attributeWidth = ImGui.CalcTextSize("Attribute").X;
                float typeWidth = ImGui.CalcTextSize("Trap (Full Ranged)").X;
                float dcWidth = ImGui.CalcTextSize("99").X;

                ImGuiTableColumnFlags columnFlags = ImGuiTableColumnFlags.None;
                ImGui.TableSetupColumn("ID", columnFlags | ImGuiTableColumnFlags.WidthFixed, idWidth + 10);
                ImGui.TableSetupColumn("Name", columnFlags | ImGuiTableColumnFlags.WidthStretch, nameWidth);
                ImGui.TableSetupColumn("ATK", columnFlags | ImGuiTableColumnFlags.WidthFixed, attackWidth + 20);
                ImGui.TableSetupColumn("DEF", columnFlags | ImGuiTableColumnFlags.WidthFixed, attackWidth + 20);
                ImGui.TableSetupColumn("LVL", columnFlags | ImGuiTableColumnFlags.WidthFixed, levelWidth + 20);
                ImGui.TableSetupColumn("Attribute", columnFlags | ImGuiTableColumnFlags.WidthFixed, attributeWidth + 10);
                ImGui.TableSetupColumn("Type", columnFlags | ImGuiTableColumnFlags.WidthFixed, typeWidth + 20);
                ImGui.TableSetupColumn("DC", columnFlags | ImGuiTableColumnFlags.WidthFixed, dcWidth);
                ImGui.TableHeadersRow();


                ImGuiTableSortSpecsPtr sortSpecifications = ImGui.TableGetSortSpecs();
                bool ascending = sortSpecifications.Specs.SortDirection == ImGuiSortDirection.Ascending;
                if (sortSpecifications.SpecsDirty)
                {
                    sortedDeckList[deckIndex].Sort((a, b) =>
                    {
                        CardConstant cardA = a.CardConstant;
                        CardConstant cardB = b.CardConstant;

                        switch (sortSpecifications.Specs.ColumnIndex)
                        {
                            case 0: return DeckEditorWindow.CompareWithFallback(cardA.Index, cardB.Index, cardA.Name.Current, cardB.Name.Current, ascending);
                            case 1: return DeckEditorWindow.CompareWithFallback(cardA.Name.Current, cardB.Name.Current, cardA.Index, cardB.Index, ascending);
                            case 2: return DeckEditorWindow.CompareWithFallback(cardA.Attack, cardB.Attack, cardA.Index, cardB.Index, ascending);
                            case 3: return DeckEditorWindow.CompareWithFallback(cardA.Defense, cardB.Defense, cardA.Index, cardB.Index, ascending);
                            case 4: return DeckEditorWindow.CompareWithFallback(cardA.Level, cardB.Level, cardA.Index, cardB.Index, ascending);
                            case 5:
                                return DeckEditorWindow.CompareWithFallback(CardAttribute.GetAttributeVisual(cardA), CardAttribute.GetAttributeVisual(cardB),
                                    cardA.Index, cardB.Index, ascending);
                            case 6: return DeckEditorWindow.CompareWithFallback(cardA.Type, cardB.Type, cardA.Index, cardB.Index, ascending);
                            case 7: return DeckEditorWindow.CompareWithFallback(cardA.DeckCost, cardB.DeckCost, cardA.Index, cardB.Index, ascending);
                            default: return 0;
                        }
                    });
                    sortSpecifications.SpecsDirty = false;
                }

                for (var index = 0; index < sortedDeckList[deckIndex].Count; index++)
                {
                    CardConstant cardConstant = sortedDeckList[deckIndex][index].CardConstant;
                    var colour = DeckEditorWindow.CardConstantRowColor(cardConstant).value;
                    ImGui.TableNextRow();

                    uint rowColor = (uint)((int)(colour.W * 255) << 24 | (int)(colour.Z * 255) << 16 | (int)(colour.Y * 255) << 8 |
                                           (int)(colour.X * 255));

                    ImGui.TableSetBgColor(ImGuiTableBgTarget.RowBg0, rowColor);


                    ImGui.PushID(index);



                    ImGui.TableSetColumnIndex(0);
                    ImGui.TextUnformatted(cardConstant.Index.ToString());

                    ImGui.TableSetColumnIndex(1);
                    ImGui.Text(cardConstant.Name.Current);

                    ImGui.TableSetColumnIndex(2);
                    ImGui.Text(cardConstant.Attack.ToString());

                    ImGui.TableSetColumnIndex(3);
                    ImGui.Text(cardConstant.Defense.ToString());

                    ImGui.TableSetColumnIndex(4);
                    ImGui.Text(cardConstant.Level.ToString());

                    ImGui.TableSetColumnIndex(5);
                    ImGui.Text(cardConstant.AttributeName);

                    ImGui.TableSetColumnIndex(6);
                    ImGui.Text(cardConstant.Type);

                    ImGui.TableSetColumnIndex(7);
                    ImGui.Text(cardConstant.DeckCost.ToString());


                    ImGui.TableSetColumnIndex(0);
                    ImGui.PopID();
                    if (ImGui.Selectable($"##index", false, ImGuiSelectableFlags.SpanAllColumns))
                    {
                    }
                    if (ImGui.IsItemHovered())
                    {
                        GlobalImgui.RenderTooltipCardImage(cardConstant.Name.Default);
                    }
                }
                ImGui.EndTable();
            }
        }
        ImGui.PopFont();
    }

    int[] GetStarterDeckPool(int nameResultIndex)
    {
        ushort packed = DeckTab[nameResultIndex & 0xF];

        return new int[] {
            (packed >> 0) & 0x1F,
            (packed >> 5) & 0x1F,
            (packed >> 10) & 0x1F,
        };
    }

    unsafe int FilterNameInput(ImGuiInputTextCallbackData* data)
    {
        char typedChar = (char)data->EventChar;
        if (characterMap.ContainsKey(typedChar))
        {
            return 0;
        }

        return 1;
    }


    public void Free()
    {

    }
}