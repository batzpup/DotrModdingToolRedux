using System.Reflection;
namespace DotrModdingTool2IMGUI;

public class Card
{
    public const ushort TotalCardCount = 854;
    public const int EquipCardStartIndex = 752;
    public const int EquipCardEndIndex = 800;
    public const int EquipCardCount = EquipCardEndIndex - EquipCardStartIndex;
    public const int MonsterCardStartIndex = 0;
    public const int MonsterCardEndIndex = 682;

    public static ModdedStringName[] cardNameList;

    static string[] currentCardNameCache;
    static string[] defaultCardNameCache;

    public static readonly List<ModdedStringName> AltArtNames = new List<ModdedStringName> {
        new ModdedStringName("AA Blue-Eyes White Dragon", "AA Blue-Eyes White Dragon"),
        new ModdedStringName("AA Flame Swordsman", "AA Flame Swordsman"),
        new ModdedStringName("AA Summoned Skull", "AA Summoned Skull"),
        new ModdedStringName("AA Dark Magician", "AA Dark Magician"),
        new ModdedStringName("AA Gaia The Fierce Knight", "AA Gaia The Fierce Knight"),
        new ModdedStringName("AA Celtic Guardian", "AA Celtic Guardian"),
        new ModdedStringName("AA Kuriboh", "AA Kuriboh"),
        new ModdedStringName("AA Tiger Axe", "AA Tiger Axe"),
        new ModdedStringName("AA Thousand Dragon", "AA Thousand Dragon"),
        new ModdedStringName("AA Red Eyes Black Dragon 1", "AA Red Eyes Black Dragon 1"),
        new ModdedStringName("AA Red Eyes Black Dragon 2", "AA Red Eyes Black Dragon 2"),
        new ModdedStringName("AA Blue-Eyes Ultimate Dragon", "AA Blue-Eyes Ultimate Dragon"),
        new ModdedStringName("AA Pendulum Machine", "AA Pendulum Machine"),
        new ModdedStringName("AA Launcher Spider", "AA Launcher Spider"),
        new ModdedStringName("AA Gemini Elf", "AA Gemini Elf"),
        new ModdedStringName("AA The Unhappy Maiden", "AA The Unhappy Maiden"),
        new ModdedStringName("AA Crush Card", "AA Crush Card"), // Picture only
    };

    static Card()
    {
        Assembly assembly = Assembly.GetExecutingAssembly();
        string resourceName = $"{assembly.GetName().Name}.GameData.CardList.txt";
        using (Stream? stream = assembly.GetManifestResourceStream(resourceName))
        {
            if (stream is null)
            {
                Console.Error.WriteLine($"No resource exists with the name {resourceName}");
                throw new Exception($"Cannot find{resourceName} ");
            }
            using (StreamReader streamReader = new StreamReader(stream))
            {
                var defaultNameList = streamReader.ReadToEnd().ToString().Split(Environment.NewLine, StringSplitOptions.None);
                cardNameList = new ModdedStringName[defaultNameList.Length];
                for (var index = 0; index < defaultNameList.Length; index++)
                {
                    var name = defaultNameList[index];
                    cardNameList[index] = new ModdedStringName(name, name);
                }

                //  CardNameIndexDictionary = cardNameList
                //      .Select((name, index) => new KeyValuePair<string, int>(name, index))
                //      .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
        }
        RebuildStringCache(true);
    }

    public static void ReloadStrings()
    {
        for (int i = 0; i < Card.cardNameList.Length; i++)
        {
            Card.cardNameList[i].Edited = StringEditor.StringTable[i + StringEditor.CardNamesOffsetStart];
        }
        RebuildStringCache();

    }

    public static void RebuildStringCache(bool both = false)
    {
        currentCardNameCache = cardNameList.Select(c => c.Current ?? "").ToArray();
        if (both)
        {
            defaultCardNameCache = cardNameList.Select(c => c.Default ?? "").ToArray();
        }

    }

    public static string[] GetCardStringArray(bool getDef = false)
    {
        if (UserSettings.UseDefaultNames || getDef)
        {
            return defaultCardNameCache;
        }
        return currentCardNameCache;

    }

    public static ModdedStringName GetNameByIndex(int index)
    {
        if (index >= cardNameList.Length)
        {
            return new ModdedStringName("???????", "???????");
        }
        return cardNameList[index];
    }
}