using System.Drawing;
using System.Numerics;
using ImGuiNET;
using NativeFileDialogSharp;
namespace DotrModdingTool2IMGUI;

class FusionEditorWindow : IImGuiWindow
{
    ImFontPtr font = Fonts.MonoSpace;
    List<KeyValuePair<int, FusionData>> sortedData;
    string filter1Text = "";
    string filter2Text = "";
    string filter3Text = "";
    string searchText = "";
    bool lowerFocusInput = true;
    bool higherFocusInput = true;
    bool resultFocusInput = true;

    Vector4 tableBgColour = UserSettings.FusionTableBgColour;
    Vector4 searchColour = UserSettings.FusionDropdownColour;

    public FusionEditorWindow()
    {
        EditorWindow.OnIsoLoaded += OnIsoLoaded;
    }

    public void OnIsoLoaded()
    {
        sortedData = FusionData.FusionTableData.ToList();
    }

  
    public void Render()
    {
        DrawFusionTable();
    }

    public void SaveFusionChanges()
    {
        FusionData.FusionTableData = sortedData.ToDictionary();
        DataAccess.Instance.SaveFusionData(FusionData.Bytes);
    }

    unsafe void DrawFusionTable()
    {
        ImGui.PushFont(font);
        if (!DataAccess.Instance.IsIsoLoaded)
        {
            ImGui.Text("Please load ISO file");
            ImGui.PopFont();
            return;
        }


        ImGuiListClipperPtr clipper = new ImGuiListClipperPtr(ImGuiNative.ImGuiListClipper_ImGuiListClipper());
        float columnWidth = ImGui.CalcTextSize("Winged Dragon, Guardian of the Fortress #1").X + 100;
        if (ImGui.ColorEdit4("Table Background", ref tableBgColour, ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.NoInputs))
        {
            UserSettings.FusionTableBgColour = tableBgColour;
        }
        ImGui.SameLine();
        if (ImGui.ColorEdit4("Search Dropdown", ref searchColour, ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.NoInputs))
        {
            UserSettings.FusionDropdownColour = searchColour;
        }
        ImGui.Text("Search Bar");
        ImGui.InputText("##SearchBar", ref searchText, 32);
        ImGui.SameLine();
        if (ImGui.Button("Import from CSV"))
        {
            DialogResult result = Dialog.FileOpen("csv");
            if (result.IsOk)
            {
                string isoPath = result.Path;
                if (result.Path.EndsWith(".csv"))
                {
                    FusionData.ImportFromCSV(isoPath);
                    sortedData = FusionData.FusionTableData.ToList();
                }
                else
                {
                    Console.WriteLine("Should show pop up error");
                }
            }
        }
        ImGui.SameLine();
        if (ImGui.Button("Export to CSV"))
        {
            DialogResult result = Dialog.FileSave("csv");
            if (result.IsOk)
            {
                string isoPath = result.Path + ".csv";
                FusionData.ExportToCSV(isoPath);
            }
        }
        ImGui.PushStyleColor(ImGuiCol.TableRowBg, tableBgColour);
        ImGui.PushStyleColor(ImGuiCol.TableRowBgAlt, tableBgColour);
        ImGui.PushStyleColor(ImGuiCol.FrameBg, tableBgColour);
        ImGui.PushStyleColor(ImGuiCol.Button, tableBgColour);
        ImGui.PushStyleColor(ImGuiCol.PopupBg, searchColour);

        if (ImGui.BeginTable("##FusionTable", 4,
                ImGuiTableFlags.Borders | ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.NoHostExtendX | ImGuiTableFlags.RowBg |
                ImGuiTableFlags.Sortable))
        {
            ImGuiTableColumnFlags columnFlags = ImGuiTableColumnFlags.None;
            ImGui.TableSetupColumn("Fusion Id", columnFlags);
            ImGui.TableSetupColumn("Card 1", columnFlags, columnWidth);
            ImGui.TableSetupColumn("Card 2", columnFlags, columnWidth);
            ImGui.TableSetupColumn("Fusion Result", columnFlags, columnWidth);
            ImGui.TableHeadersRow();

            ImGuiTableSortSpecsPtr sortSpecifications = ImGui.TableGetSortSpecs();

            if (sortSpecifications.SpecsDirty)
            {
                bool ascending = sortSpecifications.Specs.SortDirection == ImGuiSortDirection.Ascending;
                sortedData.Sort((pairA, pairB) =>
                {
                    int keyA = pairA.Key;
                    int keyB = pairB.Key;
                    var a = pairA.Value;
                    var b = pairB.Value;

                    switch (sortSpecifications.Specs.ColumnIndex)
                    {
                        case 0: return ascending ? keyA.CompareTo(keyB) : keyB.CompareTo(keyA);
                        case 1:
                            return ascending
                                ? string.CompareOrdinal(a.lowerCardName, b.lowerCardName)
                                : string.CompareOrdinal(b.lowerCardName, a.lowerCardName);
                        case 2:
                            return ascending
                                ? string.CompareOrdinal(a.higherCardName, b.higherCardName)
                                : string.CompareOrdinal(b.higherCardName, a.higherCardName);
                        case 3:
                            return ascending
                                ? string.CompareOrdinal(a.cardResultName, b.cardResultName)
                                : string.CompareOrdinal(b.cardResultName, a.cardResultName);
                        default: return 0;
                    }
                });
                sortSpecifications.SpecsDirty = false;
            }


            List<KeyValuePair<int, FusionData>> filteredData = sortedData.Where(entry =>
            {
                var fusion = entry.Value;
                bool matchesLower = fusion.lowerCardName.Contains(searchText, StringComparison.OrdinalIgnoreCase);
                bool matchesHigher = fusion.higherCardName.Contains(searchText, StringComparison.OrdinalIgnoreCase);
                bool matchesResult = fusion.cardResultName.Contains(searchText, StringComparison.OrdinalIgnoreCase);

                return matchesLower || matchesHigher || matchesResult;
            }).ToList();

            clipper.Begin(filteredData.Count);
            while (clipper.Step())
            {
                for (int i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
                {
                    var entry = filteredData[i];
                    int id = entry.Key;
                    var fusion = entry.Value;
                    fusion.UpdateFusion();

                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);
                    ImGui.Text(id.ToString());



                    ImGui.TableSetColumnIndex(1);
                    ImGui.SetNextItemWidth(columnWidth);
                    int selected1 = fusion.lowerCardId;
                    if (ImGui.BeginCombo($"##lower_{id}", Card.cardNameList[selected1]))
                    {
                        if (lowerFocusInput)
                        {
                            ImGui.SetKeyboardFocusHere();
                            lowerFocusInput = false;
                        }
                        ImGui.PushStyleColor(ImGuiCol.FrameBg, searchColour);
                        ImGui.InputText($"##searchInputLower_{id}", ref filter1Text, 64);
                        ImGui.PopStyleColor();

                        List<string> filteredList = Card.cardNameList
                            .Where(cardName => cardName.Contains(filter1Text, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        bool anyVisible = false;
                        foreach (var cardName in filteredList)
                        {
                            int index = Array.IndexOf(Card.cardNameList, cardName);
                            bool isSelected = selected1 == index;
                            if (ImGui.Selectable(cardName, isSelected))
                            {
                                selected1 = index;
                                fusion.lowerCardId = (ushort)selected1;
                                filter1Text = "";
                                fusion.UpdateFusion();
                            }
                            if (ImGui.IsItemVisible())
                            {
                                anyVisible = true;
                            }
                            if (ImGui.IsItemHovered())
                            {
                                ImGui.BeginTooltip();
                                ImGui.Text("Card Preview");
                                ImGui.Image(GlobalImages.Instance.Cards[cardName], new Vector2(128, 128));
                                ImGui.EndTooltip();
                            }
                        }
                        if (!anyVisible)
                        {
                            filter1Text = "";
                            lowerFocusInput = true;

                        }
                        ImGui.EndCombo();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        GlobalImgui.RenderTooltipCardImage(fusion.lowerCardName);
                    }


                    ImGui.TableSetColumnIndex(2);
                    ImGui.SetNextItemWidth(columnWidth);
                    int selected2 = fusion.higherCardId;
                    if (ImGui.BeginCombo($"##higher{id}", Card.cardNameList[selected2]))
                    {
                        if (higherFocusInput)
                        {
                            ImGui.SetKeyboardFocusHere();
                            higherFocusInput = false;
                        }
                        ImGui.InputText($"##searchInputHigher_{id}", ref filter2Text, 64);
                        List<string> filteredList = Card.cardNameList
                            .Where(cardName => cardName.Contains(filter2Text, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        bool anyVisible = false;
                        foreach (var cardName in filteredList)
                        {
                            int index = Array.IndexOf(Card.cardNameList, cardName);
                            bool isSelected = selected2 == index;
                            if (ImGui.Selectable(cardName, isSelected))
                            {
                                selected2 = index;
                                fusion.higherCardId = (ushort)selected2;
                                fusion.UpdateFusion();
                            }
                            if (ImGui.IsItemVisible())
                            {
                                anyVisible = true;
                            }
                            if (ImGui.IsItemHovered())
                            {
                                GlobalImgui.RenderTooltipCardImage(cardName);
                            }
                        }
                        if (!anyVisible)
                        {
                            filter2Text = "";
                            higherFocusInput = true;
                        }
                        ImGui.EndCombo();

                    }
                    if (ImGui.IsItemHovered())
                    {
                        GlobalImgui.RenderTooltipCardImage(fusion.higherCardName);
                    }

                    ImGui.TableSetColumnIndex(3);
                    ImGui.SetNextItemWidth(columnWidth);
                    int selectedResult = fusion.resultId;
                    if (ImGui.BeginCombo($"##result{id}", Card.cardNameList[selectedResult]))
                    {
                        if (resultFocusInput)
                        {
                            ImGui.SetKeyboardFocusHere();
                            resultFocusInput = false;
                        }
                        ImGui.InputText($"##searchInputResult_{id}", ref filter3Text, 64);
                        List<string> filteredList = Card.cardNameList
                            .Where(cardName => cardName.Contains(filter3Text, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        bool anyVisible = false;
                        foreach (var cardName in filteredList)
                        {
                            int index = Array.IndexOf(Card.cardNameList, cardName);
                            bool isSelected = selectedResult == index;
                            if (ImGui.Selectable(cardName, isSelected))
                            {
                                selectedResult = index;
                                fusion.resultId = (ushort)selectedResult;
                                fusion.UpdateFusion();

                            }
                            if (ImGui.IsItemVisible())
                            {
                                anyVisible = true;
                            }
                            if (ImGui.IsItemHovered())
                            {
                                GlobalImgui.RenderTooltipCardImage(cardName);
                            }
                        }
                        if (!anyVisible)
                        {
                            filter3Text = "";
                            resultFocusInput = true;

                        }
                        ImGui.EndCombo();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        GlobalImgui.RenderTooltipCardImage(fusion.cardResultName);
                    }
                }

            }

            ImGui.EndTable();

        }
        ImGui.PopStyleColor(5);
        ImGui.PopFont();
    }

    public void Free()
    {

    }
}