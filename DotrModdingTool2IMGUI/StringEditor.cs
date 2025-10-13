using System.Text;
using System.Text.Json;
namespace DotrModdingTool2IMGUI;

public static class StringEditor
{
    public static StringDecoder StringDecoder = new StringDecoder();
    public static StringEncoder StringEncoder = new StringEncoder();

    public static Dictionary<int, string> StringTable;
    public static bool ShouldDumpText = false;

    public static List<byte> OffsetBytes = new List<byte>();
    public static List<byte> StringBytes = new List<byte>();

    public const char PNamePlaceholder = '\uFFF2';
    // start of offset for index 30
    public const int StringTableSize = 3073;
    public static int FirstEnglishOffset = 540;


    public const int CardTypesStart = 30;
    public const int MonsterTypesStart = 37;
    public const int AttributesStart = 59;
    public const int PlayersStart = 68;
    public const int TerrainStart = 71;
    public const int LeaderRanksStart = 82;
    public const int MiscText1 = 95;
    public const int LeaderAbilitiesStart = 99;
    public const int MiscText2 = 114;
    public const int CustomDuelistNameStart = 141;
    public const int CustomDuelistNameEnd = 144;
    public const int MiscText3 = 145;
    public const int DuelistNameOffsetStart = 196;
    public const int DuelistNameOffsetEnd = 217;
    public const int DuelArenaNamesStart = 218;
    public const int DuelArenaNamesEnd = 242;
    public const int MiscText2Start = 244;
    public const int DebugMenu = 284;
    public const int CardNamesOffsetStart = 320;
    public const int CardNamesOffsetEnd = 1173;
    public const int CardEffectTextOffsetStart = 1174;
    public const int CardEffectTextOffsetEnd = 2027;
    public const int LancasterIntroStart = 2029;
    public const int YorkistsDuelistsDialogueStart = 2183;

    public const int LancasterWinPassword = 2332;
    public const int YorkistSideIntroStart = 2334;
    public const int LancasterDuelistDialogueStart = 2357;
    
    public const int MemoryCardStuffStart = 2482;
    public const int TutorialStart = 2524;

    // non-English indexes that shouldn't be messed with
    public static List<int> Uneditable = new List<int>() {
        36, 58, 81, 170, 171, 172, 173, 243, 284, 285,
        286, 287, 288, 289, 290, 291, 292, 293, 294, 295, 296, 297,
        298, 299, 300, 301, 302, 303, 304, 305, 306, 307, 308, 309,
        310, 311, 312, 313, 314, 315, 316, 317, 318, 319, 2028, 2480, 2481
    };

    

    static StringEditor()
    {
        for (int i = 0; i < 30; i++)
        {
            Uneditable.Add(i);
        }
        for (int i = DebugMenu; i < CardNamesOffsetStart; i++)
        {
            Uneditable.Add(i);
        }
    }

    public static void ReloadFromISO()
    {
        Run();
        ReloadStrings();
    }

    public static void Run()
    {
        StringTable = new Dictionary<int, string>();
        StringDecoder = new StringDecoder();
        StringEncoder = new StringEncoder();
        OffsetBytes = new List<byte>();
        StringBytes = new List<byte>();
        StringDecoder.Run();
    }

    public static void ReloadStrings()
    {

        Card.ReloadStrings();
        Enemies.ReloadStrings();
        Map.ReloadStrings();
        Effects.ReloadStrings();
        //Arena names
        StringEditorWindow.ReloadStrings();
        MusicEditorWindow.ReloadStrings();
    }

    static void ReloadStringsWithNewTable(Dictionary<int, string> newStringTable)
    {
        if (newStringTable.Count != StringTableSize)
        {
            throw new Exception("Invalid string table size");
        }
        StringTable = newStringTable;
        ReloadStrings();
    }

    public static void ExportStringsToJSON(string isoPath)
    {
        var options = new JsonSerializerOptions {
            WriteIndented = true,
            Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping

        };
        string jsonOutput = JsonSerializer.Serialize(StringTable, options);
        File.WriteAllText(isoPath, jsonOutput, Encoding.UTF8);
    }

    public static bool ImportStringsToJSON(string path)
    {
        string json = File.ReadAllText(path, Encoding.UTF8);
        var newStringTable = JsonSerializer.Deserialize<Dictionary<int, string>>(json);
        if (newStringTable == null)
        {
            throw new Exception("Failed to parse JSON: string table object is null.");
            return false;
        }
        ReloadStringsWithNewTable(newStringTable);

        return true;
    }
}