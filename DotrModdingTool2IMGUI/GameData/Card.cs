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
            Card.cardNameList[i].Edited =StringEditor.StringTable[i + StringEditor.CardNamesOffsetStart];
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
            return new ModdedStringName("???", "???");
        }
        return cardNameList[index];
    }
}