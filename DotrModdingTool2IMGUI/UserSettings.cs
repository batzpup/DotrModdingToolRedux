using System.Drawing;
using System.Globalization;
using System.Numerics;
using System.Text;
namespace DotrModdingTool2IMGUI;

public static class UserSettings
{
    public static bool UseDefaultNames = false;
    public static bool UseOriginalImages = false;
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
    public static int LastDeckEditorFontSize { get; set; } = 24;
    public static string? LastIsoPath { get; set; }
    public static bool AutoChangelog = false;


    public static void SaveSettings()
    {
        StringBuilder sb = new StringBuilder();

        sb.AppendLine("[Settings]");
        sb.AppendLine($"LastDeckEditorFontSize={LastDeckEditorFontSize}");
        sb.AppendLine($"deckEditorUseColours={deckEditorUseColours}");
        sb.AppendLine($"performanceMode={performanceMode}");
        sb.AppendLine($"ToggleImageTooltips={ToggleImageTooltips}");
        sb.AppendLine($"UseDefaultNames={UseDefaultNames}");
        sb.AppendLine($"UseOriginalImages={UseOriginalImages}");
        sb.AppendLine($"LastIsoPath={LastIsoPath}");
        sb.AppendLine($"AutoChangelog={AutoChangelog}");

        /* Locale management on Save. Failing that may create severe and hard-to-find color glitches, depending on float locales.
         * Pretty trivial. Basically telling the CS engine to shut it and force dots. */
        sb.AppendLine("[Colors]");
        sb.AppendLine($"DeckEditorHighlightcolour=" +
                      $"{DeckEditorHighlightcolour.X.ToString().Replace(',','.')},{DeckEditorHighlightcolour.Y.ToString().Replace(',','.')}," +
                      $"{DeckEditorHighlightcolour.Z.ToString().Replace(',','.')},{DeckEditorHighlightcolour.W.ToString().Replace(',','.')}");
        sb.AppendLine($"FusionTableBgColour=" +
                      $"{FusionTableBgColour.X.ToString().Replace(',','.')},{FusionTableBgColour.Y.ToString().Replace(',','.')}," +
                      $"{FusionTableBgColour.Z.ToString().Replace(',','.')},{FusionTableBgColour.W.ToString().Replace(',','.')}");
        sb.AppendLine($"FusionDropdownColour=" +
                      $"{FusionDropdownColour.X.ToString().Replace(',','.')},{FusionDropdownColour.Y.ToString().Replace(',','.')}," +
                      $"{FusionDropdownColour.Z.ToString().Replace(',','.')},{FusionDropdownColour.W.ToString().Replace(',','.')}");
        sb.AppendLine($"CustomSlotTableBgColour=" +
                      $"{CustomSlotTableBgColour.X.ToString().Replace(',','.')},{CustomSlotTableBgColour.Y.ToString().Replace(',','.')}," +
                      $"{CustomSlotTableBgColour.Z.ToString().Replace(',','.')},{CustomSlotTableBgColour.W.ToString().Replace(',','.')}");
        sb.AppendLine($"CustomSlotDropdownColour=" +
                      $"{CustomSlotDropdownColour.X.ToString().Replace(',','.')},{CustomSlotDropdownColour.Y.ToString().Replace(',','.')}," +
                      $"{CustomSlotDropdownColour.Z.ToString().Replace(',','.')},{CustomSlotDropdownColour.W.ToString().Replace(',','.')}");
        sb.AppendLine($"CardEditorDifferenceHighlightColour=" +
                      $"{CardEditorDifferenceHighlightColour.X.ToString().Replace(',','.')},{CardEditorDifferenceHighlightColour.Y.ToString().Replace(',','.')}," +
                      $"{CardEditorDifferenceHighlightColour.Z.ToString().Replace(',','.')},{CardEditorDifferenceHighlightColour.W.ToString().Replace(',','.')}");

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
            {
                continue;
            }

            var parts = line.Split('=', 2);
            if (parts.Length == 2)
            {
                config[parts[0].Trim()] = parts[1].Trim();
            }
        }

        /* Locale Management on Load. Not trivial ; failing that straight up crashes the Tool.
         * '.' is now native on Save ; UserSettings on ',' systems will be corrupt anyway
         * (unless the user only ever used select few strict round vectors (R, G, B, A all in {0, 1})).
         * Therefore, simply try loading as if '.' and ignore corrupt Settings from ','.
         * That shall both reset to Default actual valid Setting fields,
         * and let the Tool properly Save with '.' on next closure.
         * All cases, keeping a Console WriteLine might be use as Debug. */

        // Master Settings - Should not brick unless the user messed up

        if (config.TryGetValue("LastDeckEditorFontSize", out var fontSize))
            try { LastDeckEditorFontSize = int.Parse(fontSize); }
            catch (Exception ex) {Console.WriteLine("Ignored Corrupt UserSetting : " + "LastDeckEditorFontSize"); }

        if (config.TryGetValue("LastIsoPath", out var isoPath))
            try { LastIsoPath = isoPath; }
            catch (Exception ex) {Console.WriteLine("Ignored Corrupt UserSetting : " + "LastIsoPath"); }

        if (config.TryGetValue("AutoChangelog", out var autoChangelog))
            try { AutoChangelog = bool.Parse(autoChangelog); }
            catch (Exception ex) {Console.WriteLine("Ignored Corrupt UserSetting : " + "AutoChangelog"); }

        if (config.TryGetValue("deckEditorUseColours", out var useColors))
            try { deckEditorUseColours = bool.Parse(useColors); }
            catch (Exception ex) {Console.WriteLine("Ignored Corrupt UserSetting : " + "deckEditorUseColours"); }

        if (config.TryGetValue("performanceMode", out var pMode))
            try { performanceMode = bool.Parse(pMode); }
            catch (Exception ex) {Console.WriteLine("Ignored Corrupt UserSetting : " + "performanceMode"); }

        if (config.TryGetValue("ToggleImageTooltips", out var toolTipImages))
            try { ToggleImageTooltips = bool.Parse(toolTipImages); }
            catch (Exception ex) {Console.WriteLine("Ignored Corrupt UserSetting : " + "ToggleImageTooltips"); }

        if (config.TryGetValue("UseDefaultNames", out var useDefaultNames))
            try { UseDefaultNames = bool.Parse(useDefaultNames); }
            catch (Exception ex) {Console.WriteLine("Ignored Corrupt UserSetting : " + "UseDefaultNames"); }

        if (config.TryGetValue("UseOriginalImages", out var useOriginalImages))
            try { UseOriginalImages = bool.Parse(useOriginalImages); }
            catch (Exception ex) {Console.WriteLine("Ignored Corrupt UserSetting : " + "UseOriginalImages"); }

        // Colors - Probable brick at least once on many users

        if (config.TryGetValue("DeckEditorHighlightcolour", out var deckHighlight))
            try { DeckEditorHighlightcolour = ParseColor(deckHighlight); }
            catch (Exception ex) {Console.WriteLine("Ignored Corrupt UserSetting : " + "DeckEditorHighlightcolour"); }

        if (config.TryGetValue("FusionTableBgColour", out var fusionBg))
            try { FusionTableBgColour = ParseColor(fusionBg); }
            catch (Exception ex) {Console.WriteLine("Ignored Corrupt UserSetting : " + "FusionTableBgColour"); }

        if (config.TryGetValue("FusionDropdownColour", out var fusionDropdown))
            try { FusionDropdownColour = ParseColor(fusionDropdown); }
            catch (Exception ex) {Console.WriteLine("Ignored Corrupt UserSetting : " + "FusionDropdownColour"); }

        if (config.TryGetValue("CustomSlotTableBgColour", out var customSlotBg))
            try { CustomSlotTableBgColour = ParseColor(customSlotBg); }
            catch (Exception ex) {Console.WriteLine("Ignored Corrupt UserSetting : " + "CustomSlotTableBgColour"); }

        if (config.TryGetValue("CustomSlotDropdownColour", out var customeSlotDropdown))
            try { CustomSlotDropdownColour = ParseColor(customeSlotDropdown); }
            catch (Exception ex) {Console.WriteLine("Ignored Corrupt UserSetting : " + "CustomSlotDropdownColour"); }

        if (config.TryGetValue("CardEditorDifferenceHighlightColour", out var cardEditorDifColour))
            try { CardEditorDifferenceHighlightColour = ParseColor(cardEditorDifColour); }
            catch (Exception ex) {Console.WriteLine("Ignored Corrupt UserSetting : " + "CardEditorDifferenceHighlightColour"); }

    }


    static Vector4 ParseColor(string colorStr)
    {
        string[] parts = colorStr.Split(',');
        if (parts.Length > 4)                  // Basically guaranteed on old UserSettings from ',' systems
        {
            throw new FormatException("Invalid Color Data");
        }
        
        // Non-trivial locale management 
        float[] parsedparts = new float[parts.Length];
        float currentparse;
        for (int i = 0 ; i < 4 ; i++)          // Throws Index Exception on incomplete colors
        {
            // Convert back to CS locale BS at runtime, so it does not affect our UserSettings
            try
            {
                // Load as '.' for '.' systems : float.Parse("0.5") ; not appending "f", as it actually would throw.
                currentparse = float.Parse(parts[i]);                       // Throws on ',' systems
            }
            catch (FormatException ex)
            {
                // Load as ',' for ',' systems : float.Parse("0,5") ; not appending "f", as it actually would throw.
                currentparse = float.Parse(parts[i].Replace('.',','));      // If that also throws, Exception is legit
            }
            
            // Test values - namely, throw on Float stuff going beyond 1
            if (Math.Abs(currentparse) > 1)    // Still likely on old ',' files, as it may grab Float data from truncated Settings
            {
                throw new FormatException("Invalid Color Data");
            }
            
            // At this point, data is valid
            parsedparts[i] = currentparse;
        }
        
        //Console.WriteLine(parsedparts[0] + " " + parsedparts[1] + " " + parsedparts[2] + " " + parsedparts[3]);
        return new Vector4(parsedparts);
    }
}