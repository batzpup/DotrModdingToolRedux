using System.Drawing;
using System.Numerics;
using System.Text;
namespace DotrModdingTool2IMGUI;

public static class UserSettings
{
    public static bool UseDefaultNames = false;
    public static bool deckEditorUseColours { get; set; } = false;
    public static bool performanceMode = false;
    public static bool ToggleImageTooltips = true;
    static string file = "userSettings.ini";
    public static Vector4 FusionTableBgColour = new Vector4(0, 0, 0, 0);
    public static Vector4 DeckEditorHighlightcolour = new GuiColour(8, 153, 154, 155).value;
    public static Vector4 FusionDropdownColour = new GuiColour(Color.DimGray).value;
    public static Vector4 CustomSlotTableBgColour = new Vector4(0, 0, 0, 0);
    public static Vector4 CustomSlotDropdownColour = new GuiColour(Color.DimGray).value;
    public static Vector4 CardEditorDifferenceHighlightColour = new GuiColour(Color.Orange).value;
    public static Vector4 StringDropDownColour { get; set; }
    public static string? LastIsoPath { get; set; }
    

    public static void SaveSettings()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("[Settings]");
        sb.AppendLine($"deckEditorUseColours={deckEditorUseColours}");
        sb.AppendLine($"performanceMode={performanceMode}");
        sb.AppendLine($"ToggleImageTooltips={ToggleImageTooltips}");
        sb.AppendLine($"UseDefaultNames={UseDefaultNames}");
        sb.AppendLine($"LastIsoPath={LastIsoPath}");

        sb.AppendLine("[Colors]");
        sb.AppendLine(
            $"DeckEditorHighlightcolour={DeckEditorHighlightcolour.X},{DeckEditorHighlightcolour.Y},{DeckEditorHighlightcolour.Z},{DeckEditorHighlightcolour.W}");
        sb.AppendLine($"FusionTableBgColour={FusionTableBgColour.X},{FusionTableBgColour.Y},{FusionTableBgColour.Z},{FusionTableBgColour.W}");
        sb.AppendLine($"FusionDropdownColour={FusionDropdownColour.X},{FusionDropdownColour.Y},{FusionDropdownColour.Z},{FusionDropdownColour.W}");
        sb.AppendLine(
            $"CustomSlotTableBgColour={CustomSlotTableBgColour.X},{CustomSlotTableBgColour.Y},{CustomSlotTableBgColour.Z},{CustomSlotTableBgColour.W}");
        sb.AppendLine(
            $"CustomSlotDropdownColour={CustomSlotDropdownColour.X},{CustomSlotDropdownColour.Y},{CustomSlotDropdownColour.Z},{CustomSlotDropdownColour.W}");
        sb.AppendLine(
            $"CardEditorDifferenceHighlightColour={CardEditorDifferenceHighlightColour.X},{CardEditorDifferenceHighlightColour.Y},{CardEditorDifferenceHighlightColour.Z},{CardEditorDifferenceHighlightColour.W}");

        File.WriteAllText(file, sb.ToString());
    }

    public static void LoadSettings()
    {
        if (!File.Exists(file))
            return;

        var config = new Dictionary<string, string>();

        foreach (var line in File.ReadLines(file))
        {
            if (string.IsNullOrWhiteSpace(line) || line.StartsWith("["))
                continue;

            var parts = line.Split('=', 2);
            if (parts.Length == 2)
                config[parts[0].Trim()] = parts[1].Trim();
        }


        if (config.TryGetValue("LastIsoPath", out var isoPath))
            LastIsoPath = isoPath;
        
        if (config.TryGetValue("deckEditorUseColours", out var useColors))
            deckEditorUseColours = bool.Parse(useColors);

        if (config.TryGetValue("performanceMode", out var pMode))
            performanceMode = bool.Parse(pMode);

        if (config.TryGetValue("ToggleImageTooltips", out var toolTipImages))
            ToggleImageTooltips = bool.Parse(toolTipImages);

        if (config.TryGetValue("UseDefaultNames", out var UseDefaultNames))
            ToggleImageTooltips = bool.Parse(UseDefaultNames);

        if (config.TryGetValue("DeckEditorHighlightcolour", out var deckHighlight))
            DeckEditorHighlightcolour = ParseColor(deckHighlight);

        if (config.TryGetValue("FusionTableBgColour", out var fusionBg))
            FusionTableBgColour = ParseColor(fusionBg);

        if (config.TryGetValue("FusionDropdownColour", out var fusionDropdown))
            FusionDropdownColour = ParseColor(fusionDropdown);

        if (config.TryGetValue("CustomSlotTableBgColour", out var customSlotBg))
            CustomSlotTableBgColour = ParseColor(customSlotBg);

        if (config.TryGetValue("CustomSlotDropdownColour", out var customeSlotDropdown))
            CustomSlotDropdownColour = ParseColor(customeSlotDropdown);
        if (config.TryGetValue("CardEditorDifferenceHighlightColour", out var cardEditorDifColour))
            CardEditorDifferenceHighlightColour = ParseColor(cardEditorDifColour);

    }


    static Vector4 ParseColor(string colorStr)
    {
        string[] parts = colorStr.Split(',');
        return new Vector4(
            float.Parse(parts[0]),
            float.Parse(parts[1]),
            float.Parse(parts[2]),
            float.Parse(parts[3])
        );
    }
}