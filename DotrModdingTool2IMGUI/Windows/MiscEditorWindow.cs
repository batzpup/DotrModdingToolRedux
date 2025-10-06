using ImGuiNET;
namespace DotrModdingTool2IMGUI;

class MiscEditorWindow : IImGuiWindow
{
    List<string> preloadedImages;
    string[] preloadedImagesArray;
    int currentPreloadedImageIndex = 0;
    int currentImageAssignedIndex = 0;
    

    public MiscEditorWindow()
    {
        //TODO LOAD IMAGES IN DATA ACCESS
        LoadSelectedImagesString();
    }

    public void Render()
    {
        ImGui.PushFont(Fonts.MonoSpace);
        ImGui.SetCursorPos(ImGui.GetContentRegionAvail() / 2f);
        ImGui.ListBox("PreloadedImages", ref currentPreloadedImageIndex, preloadedImagesArray, 223);
        ImGui.SetCursorPos(ImGui.GetContentRegionAvail() / 2f);
        ImGui.ListBox("CardImage", ref currentPreloadedImageIndex, Card.GetCardStringArray(), Card.GetCardStringArray().Length);
        ImGui.PopFont();
    }

    void LoadSelectedImagesString()
    {
        preloadedImages.Clear();

        for (int i = 0; i < 223; i++)
        {
            preloadedImages.Add(Card.cardNameList[PreLoadImageEditor.GetPicNumber(PreLoadImageEditor.PreloadCardArtBytes[i])].Current);
        }

    }



    public void Free()
    {

    }
}