using System.Reflection;
namespace DotrModdingTool2IMGUI;

public class Card
{
    public const ushort TotalCardCount = 854;
    public static string[] cardNameList;
    public static Dictionary<string, int> CardNameIndexDictionary = new Dictionary<string, int>();
    public const int EquipCardStartIndex = 752;
    public const int EquipCardEndIndex = 800;
    public const int EquipCardCount = EquipCardEndIndex - EquipCardStartIndex;
    public const int MonsterCardStartIndex = 0;
    public const int MonsterCardEndIndex = 682;

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
                cardNameList = streamReader.ReadToEnd().ToString().Split(Environment.NewLine, StringSplitOptions.None);
                CardNameIndexDictionary = cardNameList
                    .Select((name, index) => new KeyValuePair<string, int>(name, index))
                    .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            }
        }
    }

    public static string GetNameByIndex(int index)
    {
        if (index >= cardNameList.Length)
        {
            return "???";
        }

        return cardNameList[index];
    }
}