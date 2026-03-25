using System.Numerics;
using ImGuiNET;

namespace DotrModdingTool2IMGUI;

class ImageEditorWindow : IImGuiWindow
{
    List<ModdedStringName> preloadedImages = new List<ModdedStringName>();

    List<(int preloadIndex, ModdedStringName name)> filteredPreloadImages = new List<(int, ModdedStringName)>();
    List<ModdedStringName> filteredCardImageNames = new();
    int currentPreloadedImageIndex = 0;
    int currentImageAssignedIndex = 0;

    string cardSearchLeft = "";
    string cardSearchRight = "";
    bool scrollToSelected;

    bool useDefaultNames;


    public ImageEditorWindow()
    {
        EditorWindow.OnIsoLoaded += OnIsoLoaded;

    }

    public void Render()
    {
        if (!DataAccess.Instance.IsIsoLoaded)
        {
            return;
        }
        if (ImGui.BeginTabBar("ImageEditorTabBar"))
        {
            if (ImGui.BeginTabItem("Preloaded Image Editor"))
            {
                RenderPreloadImagerEditor();
                ImGui.EndTabItem();
            }
            if (ImGui.BeginTabItem("Image Texture Editor"))
            {
                RenderTextureEditor();
                ImGui.EndTabItem();
            }

            ImGui.EndTabBar();
        }



    }

    void RenderTextureEditor()
    {

    }

    void RenderPreloadImagerEditor()
    {
        Vector2 windowPos = ImGui.GetWindowPos();
        Vector2 windowSize = ImGui.GetWindowSize();
        float windowBottom = windowPos.Y + windowSize.Y - 110f * EditorWindow.AspectRatio.Y;
        float availableHeight = windowBottom - ImGui.GetCursorPosY();

        ImGui.PushFont(FontManager.GetFont(FontManager.FontFamily.SpaceMono, 32));
        ImGui.BeginChild("leftHandMisc", new Vector2(ImGui.GetContentRegionAvail().X / 2f, ImGui.GetContentRegionAvail().Y));
        ImGui.Text("Preloaded Images");
        ImGui.Checkbox("Use Default names", ref useDefaultNames);
        ImGui.Text("Card Search");
        ImGui.InputText("##CardSearchLeft", ref cardSearchLeft, 32);
        ImGui.SetNextItemWidth(-1);
        if (ImGui.BeginListBox("##preloadedCards", new Vector2(0, availableHeight)))
        {
            Vector2 availArea = ImGui.GetContentRegionAvail();
            if (useDefaultNames)
            {
                filteredPreloadImages = PreLoadImageEditor.Images
                    .Where(kvp => PreLoadImageEditor.PreloadDefaultImageNameList[kvp.Key].Current
                        .Contains(cardSearchLeft, StringComparison.OrdinalIgnoreCase))
                    .Select(kvp => (preloadIndex: kvp.Key, name: PreLoadImageEditor.PreloadDefaultImageNameList[kvp.Key]))
                    .ToList();
            }
            else
            {
                filteredPreloadImages = PreLoadImageEditor.Images
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
                    currentImageAssignedIndex = PreLoadImageEditor.Images[preloadIndex];
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
        ImGui.Text("Card Search");
        ImGui.InputText("##CardSearchRight", ref cardSearchRight, 32);
        ImGui.SetNextItemWidth(-1);
        if (ImGui.BeginListBox("##CardsImages", new Vector2(0, availableHeight)))
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
                    PreLoadImageEditor.Images[currentPreloadedImageIndex] = cardIndex;
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

    void OnIsoLoaded()
    {
        preloadedImages.Clear();

        for (int i = 0; i < 223; i++)
        {
            preloadedImages.Add(Card.cardNameList[PreLoadImageEditor.GetPicNumber(PreLoadImageEditor.PreloadCardArtBytes[i])]);
        }

        currentImageAssignedIndex = PreLoadImageEditor.Images[currentPreloadedImageIndex];

    }


    public void Free()
    {

    }
}