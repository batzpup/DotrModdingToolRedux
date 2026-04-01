using System.Numerics;
using ImGuiNET;
namespace DotrModdingTool2IMGUI;

class PreloadImageEditorWindow : IImGuiWindow
{
    List<(int preloadIndex, ModdedStringName name)> filteredPreloadImages = new List<(int, ModdedStringName)>();
    List<ModdedStringName> filteredCardImageNames = new();
    int currentPreloadedImageIndex = 0;
    int currentImageAssignedIndex = 0;

    string cardSearchLeft = "";
    string cardSearchRight = "";
    bool scrollToSelected;
    bool useDefaultNames;

    public PreloadImageEditorWindow()
    {
        EditorWindow.OnIsoLoaded += OnIsoLoaded;
    }

    void OnIsoLoaded()
    {
        currentImageAssignedIndex = GameImageManager.PicPackImages[currentPreloadedImageIndex];
    }

    public void Render()
    {
        Vector4 listBoxBg = ImGui.GetStyle().Colors[(int)ImGuiCol.FrameBg];

        ImGui.PushFont(FontManager.GetFont(FontManager.FontFamily.SpaceMono, 32));
        ImGui.BeginChild("leftHandMisc", new Vector2(ImGui.GetContentRegionAvail().X / 2f, ImGui.GetContentRegionAvail().Y));
        ImGui.Text("Preloaded Images");
        ImGui.Separator();
        ImGui.Checkbox("Use Default names", ref useDefaultNames);
        ImGui.Text("Card Search");
        ImGui.SetNextItemWidth(-1);
        ImGui.InputText("##CardSearchLeft", ref cardSearchLeft, 32);

        Vector2 availArea = ImGui.GetContentRegionAvail();

        if (useDefaultNames)
        {
            filteredPreloadImages = GameImageManager.PicPackImages
                .Where(kvp => GameImageManager.PreloadDefaultImageNameList[kvp.Key].Current.Contains(cardSearchLeft, StringComparison.OrdinalIgnoreCase)
                              || kvp.Key.ToString().Contains(cardSearchLeft))
                .Select(kvp => (preloadIndex: kvp.Key, name: GameImageManager.PreloadDefaultImageNameList[kvp.Key]))
                .ToList();
        }
        else
        {
            filteredPreloadImages = GameImageManager.PicPackImages
                .Where(kvp => Card.cardNameList[kvp.Value].Current.Contains(cardSearchLeft, StringComparison.OrdinalIgnoreCase)
                              || kvp.Key.ToString().Contains(cardSearchLeft))
                .Select(kvp => (preloadIndex: kvp.Key, name: Card.cardNameList[kvp.Value]))
                .ToList();
        }


        ImGui.PushStyleColor(ImGuiCol.TableRowBg, listBoxBg);
        ImGui.PushStyleColor(ImGuiCol.TableRowBgAlt, listBoxBg);
        ImGui.SetNextItemWidth(availArea.X);
        if (ImGui.BeginTable("##PreloadImageTable", 2, ImGuiTableFlags.ScrollY | ImGuiTableFlags.RowBg))
        {
            ImGui.TableSetupColumn("##ID", ImGuiTableColumnFlags.WidthFixed, 45);
            ImGui.TableSetupColumn("##Name", ImGuiTableColumnFlags.WidthStretch);

            for (var i = 0; i < filteredPreloadImages.Count; i++)
            {
                var (preloadIndex, filteredName) = filteredPreloadImages[i];
                bool isSelected = preloadIndex == currentPreloadedImageIndex;

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);

                ImGui.PushFont(FontManager.GetBestFitFont(filteredName.Current, availArea.X, availArea.Y, FontManager.FontFamily.NotoSansJP));
                if (ImGui.Selectable($"##{filteredName.Current}", isSelected, ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowDoubleClick))
                {
                    currentPreloadedImageIndex = preloadIndex;
                    currentImageAssignedIndex = GameImageManager.PicPackImages[preloadIndex];
                    scrollToSelected = true;
                }
                bool hovered = ImGui.IsItemHovered();
                ImGui.SameLine();
                ImGui.Text($"{preloadIndex}");

                ImGui.TableSetColumnIndex(1);
                ImGui.Text($"{filteredName.Current}");
                ImGui.PopFont();

                if (hovered)
                {
                    GlobalImgui.RenderTooltipCardImage(filteredName.Default);
                }
            }
            ImGui.EndTable();
        }
        ImGui.PopStyleColor(2);

        ImGui.EndChild();

        ImGui.SameLine();

        ImGui.BeginChild("rightHandMisc", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y));
        ImGui.Text("Card Image to load");
        ImGui.Separator();
        ImGui.Text("Card Search");
        ImGui.InputText("##CardSearchRight", ref cardSearchRight, 32);

        availArea = ImGui.GetContentRegionAvail();

        filteredCardImageNames = Card.cardNameList
            .Select((cardName, index) => (cardName, index))
            .Where(x => x.cardName.Current.Contains(cardSearchRight, StringComparison.OrdinalIgnoreCase) 
                        || x.index.ToString().Contains(cardSearchRight))
            .Select(x => x.cardName)
            .ToList();

        ImGui.PushStyleColor(ImGuiCol.TableRowBg, listBoxBg);
        ImGui.PushStyleColor(ImGuiCol.TableRowBgAlt, listBoxBg);
        ImGui.SetNextItemWidth(availArea.X);
        if (ImGui.BeginTable("##CardImagesTable", 2, ImGuiTableFlags.ScrollY | ImGuiTableFlags.RowBg))
        {
            ImGui.TableSetupColumn("##ID", ImGuiTableColumnFlags.WidthFixed, 45);
            ImGui.TableSetupColumn("##Name", ImGuiTableColumnFlags.WidthStretch);

            for (var i = 0; i < filteredCardImageNames.Count; i++)
            {
                ModdedStringName currentCard = filteredCardImageNames[i];
                int cardIndex = Array.IndexOf(Card.cardNameList, currentCard);
                bool isSelected = cardIndex == currentImageAssignedIndex;

                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);

                ImGui.PushFont(FontManager.GetBestFitFont(currentCard.Current, availArea.X, availArea.Y, FontManager.FontFamily.NotoSansJP));
                if (ImGui.Selectable($"##{currentCard.Current}", isSelected, ImGuiSelectableFlags.SpanAllColumns | ImGuiSelectableFlags.AllowDoubleClick))
                {
                    currentImageAssignedIndex = cardIndex;
                    GameImageManager.PicPackImages[currentPreloadedImageIndex] = cardIndex;
                }
                if (isSelected && scrollToSelected)
                {
                    ImGui.SetScrollHereY(0.5f);
                    scrollToSelected = false;
                }
                bool hovered = ImGui.IsItemHovered();
                ImGui.SameLine();
                ImGui.Text($"{cardIndex}");

                ImGui.TableSetColumnIndex(1);
                ImGui.Text($"{currentCard.Current}");
                ImGui.PopFont();

                if (hovered)
                {
                    GlobalImgui.RenderTooltipCardImage(currentCard.Default);
                }
            }
            ImGui.EndTable();
        }
        ImGui.PopStyleColor(2);

        ImGui.EndChild();
        ImGui.PopFont();
    }

    public void Free()
    {
    }
}