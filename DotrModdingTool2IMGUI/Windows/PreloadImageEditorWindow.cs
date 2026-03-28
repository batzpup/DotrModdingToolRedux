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
       

        ImGui.PushFont(FontManager.GetFont(FontManager.FontFamily.SpaceMono, 32));
        ImGui.BeginChild("leftHandMisc", new Vector2(ImGui.GetContentRegionAvail().X / 2f, ImGui.GetContentRegionAvail().Y));
        ImGui.Text("Preloaded Images");
        ImGui.Separator();
        ImGui.Checkbox("Use Default names", ref useDefaultNames);
        ImGui.Text("Card Search");
        ImGui.InputText("##CardSearchLeft", ref cardSearchLeft, 32);
        ImGui.SetNextItemWidth(-1);
        
        if (ImGui.BeginListBox("##preloadedCards", ImGui.GetContentRegionAvail()))
        {
            Vector2 availArea = ImGui.GetContentRegionAvail();
            if (useDefaultNames)
            {
                filteredPreloadImages = GameImageManager.PicPackImages
                    .Where(kvp => GameImageManager.PreloadDefaultImageNameList[kvp.Key].Current
                        .Contains(cardSearchLeft, StringComparison.OrdinalIgnoreCase))
                    .Select(kvp => (preloadIndex: kvp.Key, name: GameImageManager.PreloadDefaultImageNameList[kvp.Key])) // <-- was Card.cardNameList[kvp.Value]
                    .ToList();
            }
            else
            {
                filteredPreloadImages = GameImageManager.PicPackImages
                    .Where(kvp => Card.cardNameList[kvp.Value].Current
                        .Contains(cardSearchLeft, StringComparison.OrdinalIgnoreCase))
                    .Select(kvp => (preloadIndex: kvp.Key, name: Card.cardNameList[kvp.Value]))
                    .ToList();
            }
            for (var i = 0; i < filteredPreloadImages.Count; i++)
            {
                var (preloadIndex, filteredName) = filteredPreloadImages[i];
                bool isSelected = preloadIndex == currentPreloadedImageIndex;

                ImGui.PushFont(FontManager.GetBestFitFont(filteredName.Current, availArea.X, availArea.Y, FontManager.FontFamily.NotoSansJP));
                if (ImGui.Selectable($"{filteredName.Current}", isSelected, ImGuiSelectableFlags.AllowDoubleClick))
                {
                    currentPreloadedImageIndex = preloadIndex;
                    currentImageAssignedIndex = GameImageManager.PicPackImages[preloadIndex];
                    scrollToSelected = true;
                }
                ImGui.PopFont();

                if (ImGui.IsItemHovered())
                {
                    GlobalImgui.RenderTooltipCardImage(filteredName.Default);
                }
            }
            ImGui.EndListBox();
        }
        ImGui.EndChild();

        ImGui.SameLine();

        ImGui.BeginChild("rightHandMisc", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y));
        ImGui.Text("Card Image to load");
        ImGui.Separator();
        ImGui.Text("Card Search");
        ImGui.InputText("##CardSearchRight", ref cardSearchRight, 32);
        ImGui.SetNextItemWidth(-1);
        if (ImGui.BeginListBox("##CardsImages", ImGui.GetContentRegionAvail()))
        {

            Vector2 availArea = ImGui.GetContentRegionAvail();

            filteredCardImageNames = Card.cardNameList
                .Where(cardName => cardName.Current.Contains(cardSearchRight, StringComparison.OrdinalIgnoreCase))
                .ToList();
            for (var i = 0; i < filteredCardImageNames.Count; i++)
            {
                ModdedStringName currentCard = filteredCardImageNames[i];
                int cardIndex = Array.IndexOf(Card.cardNameList, currentCard);
                bool isSelected = cardIndex == currentImageAssignedIndex;

                ImGui.PushFont(FontManager.GetBestFitFont(currentCard.Current, availArea.X, availArea.Y, FontManager.FontFamily.NotoSansJP));
                if (ImGui.Selectable($"{currentCard.Current}", isSelected, ImGuiSelectableFlags.AllowDoubleClick))
                {
                    currentImageAssignedIndex = cardIndex;
                    GameImageManager.PicPackImages[currentPreloadedImageIndex] = cardIndex;
                }
                if (isSelected && scrollToSelected)
                {
                    ImGui.SetScrollHereY(0.5f);
                    scrollToSelected = false;
                }
                ImGui.PopFont();

                if (ImGui.IsItemHovered())
                {
                    GlobalImgui.RenderTooltipCardImage(currentCard.Default);
                }
            }
            ImGui.EndListBox();
        }
        ImGui.EndChild();
        ImGui.PopFont();
    }

    public void Free()
    {

    }
}