using System.Drawing;
using System.Numerics;
using ImGuiNET;
using NativeFileDialogSharp;
namespace DotrModdingTool2IMGUI;

class FusionEditorWindow : IImGuiWindow
{
    ImFontPtr font = FontManager.GetFont(FontManager.FontFamily.NotoSansJP, 28);
    public List<KeyValuePair<int, FusionData>> AllFusionData;
    public List<FusionData> DeckFusionData;
    string filter1Text = "";
    string filter2Text = "";
    string filter3Text = "";
    string searchText = "";
    bool lowerFocusInput = true;
    bool higherFocusInput = true;
    bool resultFocusInput = true;

    Vector4 tableBgColour = UserSettings.FusionTableBgColour;
    Vector4 searchColour = UserSettings.FusionDropdownColour;


    Deck currentDeck;
    int currentDeckListIndex = 0;


    public FusionEditorWindow()
    {
        EditorWindow.OnIsoLoaded += OnIsoLoaded;

    }

    public void OnIsoLoaded()
    {
        AllFusionData = FusionData.FusionTableData.ToList();
        currentDeck = Deck.DeckList[currentDeckListIndex];
        DeckFusionData = GetFusionsFromDeck();
    }

    List<FusionData> GetFusionsFromDeck()
    {
        List<FusionData> fusions = new List<FusionData>();
        List<FusionData> fusionTable = FusionData.FusionTableData.Values.ToList();

        for (int i = 0; i < currentDeck.CardList.Count; i++)
        {
            for (int j = i + 1; j < currentDeck.CardList.Count - 1; j++)
            {
                int lowCardId = currentDeck.CardList[i].CardConstant.Index;
                int highCardId = currentDeck.CardList[j].CardConstant.Index;
                if (lowCardId > highCardId)
                {
                    lowCardId ^= highCardId;
                    highCardId ^= lowCardId;
                    lowCardId ^= highCardId;
                }
                foreach (var fusionData in fusionTable)
                {
                    if (fusionData.lowerCardId != lowCardId)
                        continue;
                    if (fusionData.higherCardId != highCardId)
                        continue;
                    fusions.Add(fusionData);

                }
            }
        }
        return fusions;
    }


    public void Render()
    {
        ImGui.PushStyleColor(ImGuiCol.TabSelected, new GuiColour(0, 189, 0).value);
        ImGui.PushStyleColor(ImGuiCol.TabHovered, new GuiColour(128, 128, 0).value);
        if (ImGui.BeginTabBar("FusionsBar"))
        {
            if (ImGui.BeginTabItem("All Fusions"))
            {
                DrawAllFusionTable();
                ImGui.EndTabItem();
            }

            if (ImGui.BeginTabItem("Possible fusions from deck"))
            {
                DrawDeckFusionTable();
                ImGui.EndTabItem();
            }
            ImGui.EndTabBar();
        }
        ImGui.PopStyleColor(2);

    }

    unsafe void DrawDeckFusionTable()
    {
        ImGui.PushFont(font);
        ImGuiListClipperPtr clipper = new ImGuiListClipperPtr(ImGuiNative.ImGuiListClipper_ImGuiListClipper());
        if (ImGui.ColorEdit4("Table Background", ref tableBgColour, ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.NoInputs))
        {
            UserSettings.FusionTableBgColour = tableBgColour;
        }
        ImGui.SameLine();
        if (ImGui.ColorEdit4("Search Dropdown", ref searchColour, ImGuiColorEditFlags.AlphaBar | ImGuiColorEditFlags.NoInputs))
        {
            UserSettings.FusionDropdownColour = searchColour;
        }
        if (!DataAccess.Instance.IsIsoLoaded)
        {
            ImGui.Text("Please load ISO file");
            ImGui.PopFont();
            return;
        }


        ImGui.Text("Deck");
        ImGui.Image(GlobalImages.Instance.Cards[Deck.DeckList[currentDeckListIndex].DeckLeader.Name.Default], ImageHelper.DefaultImageSize);
        ImGui.SameLine();
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 2f);
        if (ImGui.BeginCombo("##Decks", $"{Deck.NamePrefix(currentDeckListIndex)} - {currentDeck.DeckLeader.Name.Current}", ImGuiComboFlags.HeightLarge))
        {
            for (var index = 0; index < Deck.DeckList.Count; index++)
            {
                bool isSelected = Deck.DeckList[index] == currentDeck;
                if (ImGui.Selectable($"{Deck.NamePrefix(index)} - {Deck.DeckList[index].DeckLeader.Name.Current}", isSelected))
                {
                    currentDeckListIndex = index;
                    currentDeck = Deck.DeckList[currentDeckListIndex];
                    DeckFusionData = GetFusionsFromDeck();
                }
                if (isSelected)
                {
                    ImGui.SetItemDefaultFocus();
                }
                if (ImGui.IsItemHovered())
                {
                    if (index >= 27 && index <= 47)
                    {
                        GlobalImgui.RenderTooltipOpponentImage((EEnemyImages)index - 26);
                    }
                    GlobalImgui.RenderTooltipCardImage(Deck.DeckList[index].DeckLeader.Name.Default);
                }
            }
            ImGui.EndCombo();
        }




        ImGui.Text("Search Bar");
        ImGui.InputText("##SearchBar", ref searchText, 32);
        //ImGui.SameLine();
        //if (ImGui.Button("Import from CSV"))
        //{
        //    DialogResult result = Dialog.FileOpen("csv");
        //    if (result.IsOk)
        //    {
        //        string isoPath = result.Path;
        //        if (result.Path.EndsWith(".csv"))
        //        {
        //            FusionData.ImportFromCSV(isoPath);
        //            AllFusionData = FusionData.FusionTableData.ToList();
        //        }
        //        else
        //        {
        //            Console.WriteLine("Should show pop up error");
        //        }
        //    }
        //}
        //ImGui.SameLine();
        //if (ImGui.Button("Export to CSV"))
        //{
        //    DialogResult result = Dialog.FileSave("csv");
        //    if (result.IsOk)
        //    {
        //        string isoPath = result.Path;
        //        FusionData.ExportToCSV(isoPath);
        //    }
        //}
        ImGui.PushStyleColor(ImGuiCol.TableRowBg, tableBgColour);
        ImGui.PushStyleColor(ImGuiCol.TableRowBgAlt, tableBgColour);
        ImGui.PushStyleColor(ImGuiCol.FrameBg, tableBgColour);
        ImGui.PushStyleColor(ImGuiCol.Button, tableBgColour);
        ImGui.PushStyleColor(ImGuiCol.PopupBg, searchColour);

        if (ImGui.BeginTable("##FusionTable", 4,
                ImGuiTableFlags.Reorderable | ImGuiTableFlags.Hideable |
                ImGuiTableFlags.Sortable |
                ImGuiTableFlags.SortMulti | ImGuiTableFlags.BordersV | ImGuiTableFlags.RowBg |
                ImGuiTableFlags.Borders |
                ImGuiTableFlags.ScrollX |
                ImGuiTableFlags.ScrollY |
                ImGuiTableFlags.BordersInnerH))
        {


            ImGui.TableSetupColumn("Id", ImGuiTableColumnFlags.WidthFixed, ImGui.CalcTextSize("12456").X);
            ImGui.TableSetupColumn("Card 1", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("Card 2", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("Fusion Result", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableHeadersRow();

            ImGuiTableSortSpecsPtr sortSpecifications = ImGui.TableGetSortSpecs();

            if (sortSpecifications.SpecsDirty)
            {
                bool ascending = sortSpecifications.Specs.SortDirection == ImGuiSortDirection.Ascending;
                DeckFusionData.Sort((pairA, pairB) =>
                {

                    var a = pairA;
                    var b = pairB;

                    switch (sortSpecifications.Specs.ColumnIndex)
                    {

                        case 1:
                            return ascending
                                ? string.CompareOrdinal(a.lowerCardName.Current, b.lowerCardName.Current)
                                : string.CompareOrdinal(b.lowerCardName.Current, a.lowerCardName.Current);
                        case 2:
                            return ascending
                                ? string.CompareOrdinal(a.higherCardName.Current, b.higherCardName.Current)
                                : string.CompareOrdinal(b.higherCardName.Current, a.higherCardName.Current);
                        case 3:
                            return ascending
                                ? string.CompareOrdinal(a.cardResultName.Current, b.cardResultName.Current)
                                : string.CompareOrdinal(b.cardResultName.Current, a.cardResultName.Current);
                        default: return 0;
                    }
                });
                sortSpecifications.SpecsDirty = false;
            }


            List<FusionData> filteredData = DeckFusionData.Where(entry =>
            {
                var fusion = entry;
                bool matchesLower = fusion.lowerCardName.Current.Contains(searchText, StringComparison.OrdinalIgnoreCase);
                bool matchesHigher = fusion.higherCardName.Current.Contains(searchText, StringComparison.OrdinalIgnoreCase);
                bool matchesResult = fusion.cardResultName.Current.Contains(searchText, StringComparison.OrdinalIgnoreCase);

                return matchesLower || matchesHigher || matchesResult;
            }).ToList();

            clipper.Begin(filteredData.Count);
            while (clipper.Step())
            {
                for (int i = clipper.DisplayStart; i < clipper.DisplayEnd; i++)
                {
                    var entry = filteredData[i];

                    var fusion = entry;


                    ImGui.TableNextRow();
                    ImGui.TableSetColumnIndex(0);

                    int id = i;
                    ImGui.Text(id.ToString());



                    ImGui.TableSetColumnIndex(1);
                    ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                    int selected1 = fusion.lowerCardId;
                    ImGui.Text(Card.cardNameList[selected1].Current);

                    if (ImGui.IsItemHovered())
                    {
                        GlobalImgui.RenderTooltipCardImage(fusion.lowerCardName.Default);
                    }


                    ImGui.TableSetColumnIndex(2);
                    ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                    int selected2 = fusion.higherCardId;
                    ImGui.Text(Card.cardNameList[selected2].Current);

                    if (ImGui.IsItemHovered())
                    {
                        GlobalImgui.RenderTooltipCardImage(fusion.higherCardName.Default);
                    }

                    ImGui.TableSetColumnIndex(3);
                    ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                    int selectedResult = fusion.resultId;
                    ImGui.Text(Card.cardNameList[selectedResult].Current);

                    if (ImGui.IsItemHovered())
                    {
                        GlobalImgui.RenderTooltipCardImage(fusion.cardResultName.Default);
                    }
                }

            }

            ImGui.EndTable();

        }
        ImGui.PopStyleColor(5);
        ImGui.PopFont();
    }

    public void SaveFusionChanges()
    {
        FusionData.FusionTableData = AllFusionData.ToDictionary();
        DataAccess.Instance.SaveFusionData(FusionData.TableBytes);
    }

    unsafe void DrawAllFusionTable()
    {
        ImGui.PushFont(font);
        if (!DataAccess.Instance.IsIsoLoaded)
        {
            ImGui.Text("Please load ISO file");
            ImGui.PopFont();
            return;
        }


        ImGuiListClipperPtr clipper = new ImGuiListClipperPtr(ImGuiNative.ImGuiListClipper_ImGuiListClipper());
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
                    AllFusionData = FusionData.FusionTableData.ToList();
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
                string isoPath = result.Path;
                FusionData.ExportToCSV(isoPath);
            }
        }
        ImGui.PushStyleColor(ImGuiCol.TableRowBg, tableBgColour);
        ImGui.PushStyleColor(ImGuiCol.TableRowBgAlt, tableBgColour);
        ImGui.PushStyleColor(ImGuiCol.FrameBg, tableBgColour);
        ImGui.PushStyleColor(ImGuiCol.Button, tableBgColour);
        ImGui.PushStyleColor(ImGuiCol.PopupBg, searchColour);

        if (ImGui.BeginTable("##FusionTable", 4,
                ImGuiTableFlags.Reorderable | ImGuiTableFlags.Hideable |
                ImGuiTableFlags.Sortable |
                ImGuiTableFlags.SortMulti | ImGuiTableFlags.BordersV | ImGuiTableFlags.RowBg |
                ImGuiTableFlags.Borders |
                ImGuiTableFlags.ScrollX |
                ImGuiTableFlags.ScrollY |
                ImGuiTableFlags.BordersInnerH))
        {


            ImGui.TableSetupColumn("Id", ImGuiTableColumnFlags.WidthFixed, ImGui.CalcTextSize("12456").X);
            ImGui.TableSetupColumn("Card 1", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("Card 2", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("Fusion Result", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableHeadersRow();

            ImGuiTableSortSpecsPtr sortSpecifications = ImGui.TableGetSortSpecs();

            if (sortSpecifications.SpecsDirty)
            {
                bool ascending = sortSpecifications.Specs.SortDirection == ImGuiSortDirection.Ascending;
                AllFusionData.Sort((pairA, pairB) =>
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
                                ? string.CompareOrdinal(a.lowerCardName.Current, b.lowerCardName.Current)
                                : string.CompareOrdinal(b.lowerCardName.Current, a.lowerCardName.Current);
                        case 2:
                            return ascending
                                ? string.CompareOrdinal(a.higherCardName.Current, b.higherCardName.Current)
                                : string.CompareOrdinal(b.higherCardName.Current, a.higherCardName.Current);
                        case 3:
                            return ascending
                                ? string.CompareOrdinal(a.cardResultName.Current, b.cardResultName.Current)
                                : string.CompareOrdinal(b.cardResultName.Current, a.cardResultName.Current);
                        default: return 0;
                    }
                });
                sortSpecifications.SpecsDirty = false;
            }


            List<KeyValuePair<int, FusionData>> filteredData = AllFusionData.Where(entry =>
            {
                var fusion = entry.Value;
                bool matchesLower = fusion.lowerCardName.Current.Contains(searchText, StringComparison.OrdinalIgnoreCase);
                bool matchesHigher = fusion.higherCardName.Current.Contains(searchText, StringComparison.OrdinalIgnoreCase);
                bool matchesResult = fusion.cardResultName.Current.Contains(searchText, StringComparison.OrdinalIgnoreCase);

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
                    ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                    int selected1 = fusion.lowerCardId;
                    if (ImGui.BeginCombo($"##lower_{id}", Card.cardNameList[selected1].Current))
                    {
                        if (lowerFocusInput)
                        {
                            ImGui.SetKeyboardFocusHere();
                            lowerFocusInput = false;
                        }
                        ImGui.PushStyleColor(ImGuiCol.FrameBg, searchColour);
                        ImGui.InputText($"##searchInputLower_{id}", ref filter1Text, 64);
                        ImGui.PopStyleColor();

                        List<ModdedStringName> filteredList = Card.cardNameList
                            .Where(cardName => cardName.Current.Contains(filter1Text, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        bool anyVisible = false;
                        foreach (var cardName in filteredList)
                        {
                            int index = Array.IndexOf(Card.cardNameList, cardName);
                            bool isSelected = selected1 == index;
                            if (ImGui.Selectable(cardName.Current, isSelected))
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
                                if (UserSettings.ToggleImageTooltips)
                                {
                                    ImGui.BeginTooltip();
                                    ImGui.Text("Card Preview");
                                    ImGui.Image(GlobalImages.Instance.Cards[cardName.Default], new Vector2(128, 128));
                                    ImGui.EndTooltip();
                                }

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
                        GlobalImgui.RenderTooltipCardImage(fusion.lowerCardName.Default);
                    }


                    ImGui.TableSetColumnIndex(2);
                    ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                    int selected2 = fusion.higherCardId;
                    if (ImGui.BeginCombo($"##higher{id}", Card.cardNameList[selected2].Current))
                    {
                        if (higherFocusInput)
                        {
                            ImGui.SetKeyboardFocusHere();
                            higherFocusInput = false;
                        }
                        ImGui.InputText($"##searchInputHigher_{id}", ref filter2Text, 64);
                        List<ModdedStringName> filteredList = Card.cardNameList
                            .Where(cardName => cardName.Current.Contains(filter2Text, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        bool anyVisible = false;
                        foreach (var cardName in filteredList)
                        {
                            int index = Array.IndexOf(Card.cardNameList, cardName);
                            bool isSelected = selected2 == index;
                            if (ImGui.Selectable(cardName.Current, isSelected))
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
                                GlobalImgui.RenderTooltipCardImage(cardName.Default);
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
                        GlobalImgui.RenderTooltipCardImage(fusion.higherCardName.Default);
                    }

                    ImGui.TableSetColumnIndex(3);
                    ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
                    int selectedResult = fusion.resultId;
                    if (ImGui.BeginCombo($"##result{id}", Card.cardNameList[selectedResult].Current))
                    {
                        if (resultFocusInput)
                        {
                            ImGui.SetKeyboardFocusHere();
                            resultFocusInput = false;
                        }
                        ImGui.InputText($"##searchInputResult_{id}", ref filter3Text, 64);
                        List<ModdedStringName> filteredList = Card.cardNameList
                            .Where(cardName => cardName.Current.Contains(filter3Text, StringComparison.OrdinalIgnoreCase))
                            .ToList();
                        bool anyVisible = false;
                        foreach (var cardName in filteredList)
                        {
                            int index = Array.IndexOf(Card.cardNameList, cardName);
                            bool isSelected = selectedResult == index;
                            if (ImGui.Selectable(cardName.Current, isSelected))
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
                                GlobalImgui.RenderTooltipCardImage(cardName.Default);
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
                        GlobalImgui.RenderTooltipCardImage(fusion.cardResultName.Default);
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