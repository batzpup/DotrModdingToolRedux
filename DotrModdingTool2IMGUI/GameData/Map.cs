namespace DotrModdingTool2IMGUI;

public class Map
{
    public static ModdedStringName[] DuelistMaps;
    static string[] currentMapNameCache;
    static string[] defaultMapNameCache;

    public static string[] defaultDuelistMaps = new string[46] {
        "Tutorial",
        "Seto",
        "Weevil",
        "Rex",
        "Keith",
        "Ishtar",
        "Necromancer",
        "Darkness-ruler",
        "Labyrinth-ruler",
        "Pegasus",
        "Richard",
        "Tea",
        "Tristan",
        "Mai",
        "Mako",
        "Joey",
        "Shadi",
        "Jasper",
        "Bakura",
        "Yugi",
        "Skull Knight",
        "Chakra",
        "Default Map 00",
        "Default Map 01",
        "Default Map 02",
        "Default Map 03",
        "Default Map 04",
        "Default Map 05",
        "Default Map 06",
        "Default Map 07",
        "Default Map 08",
        "Default Map 09",
        "Default Map 10",
        "Default Map 11",
        "Default Map 12",
        "Default Map 13",
        "Default Map 14",
        "Default Map 15",
        "Default Map 16",
        "Default Map 17",
        "Default Map 18",
        "Default Map 19",
        "Default Map 20",
        "Default Map 21",
        "Default Map 22",
        "Default Map 23",
    };

    static Map()
    {
        DuelistMaps = new ModdedStringName[defaultDuelistMaps.Length];
        for (int i = 0; i < DuelistMaps.Length; i++)
        {
            DuelistMaps[i] = new ModdedStringName(defaultDuelistMaps[i], defaultDuelistMaps[i]);
        }
    }

    public static void Initialise()
    {
        EditorWindow.OnIsoLoaded += onIsoLoaded;

    }

    public static void ImportMapFile(string path)
    {
        int x;
        int y;
        string mapTextData = File.ReadAllText(path);
        int index = 0;
        for (int mapIndex = 0; mapIndex < DataAccess.Instance.maps.Length; mapIndex++)
        {
            DotrMap map = DataAccess.Instance.maps[mapIndex];
            for (int tileIndex = 0; tileIndex < map.tiles.Length; tileIndex++)
            {
                if (index < mapTextData.Length && char.IsDigit(mapTextData[index]))
                {
                    x = tileIndex % 7;
                    y = tileIndex / 7;
                    map.tiles[x, y] = (Terrain)(mapTextData[index] - '0');
                    index++;
                }
            }
        }
    }

    public static void ExportMapsToFile(string path)
    {
        int x;
        int y;
        string textData = "";
        for (int mapIndex = 0; mapIndex < DataAccess.Instance.maps.Length; mapIndex++)
        {
            DotrMap map = DataAccess.Instance.maps[mapIndex];

            for (int tileIndex = 0; tileIndex < map.tiles.Length; tileIndex++)
            {
                x = tileIndex % 7;
                y = tileIndex / 7;
                textData += ((int)map.tiles[x, y]).ToString();
            }
        }
        File.WriteAllText(path, textData);
    }

    static void onIsoLoaded()
    {
        ReloadStrings();
    }

    public static void ReloadStrings()
    {

        for (int i = 0; i < DuelistMaps.Length; i++)
        {
            if (i > 0 && i < 20)
            {
                DuelistMaps[i] = new ModdedStringName(defaultDuelistMaps[i], StringEditor.StringTable[i + StringEditor.DuelistNameOffsetStart]);

            }
            else if (i == 20 || i == 21)
            {
                DuelistMaps[i] = new ModdedStringName(defaultDuelistMaps[i], $"{StringEditor.StringTable[i + StringEditor.DuelistNameOffsetStart]} ({Deck.DeckList[i + 26].DeckLeader.Name.Current})");
            }
            else
            {
                DuelistMaps[i] = new ModdedStringName(defaultDuelistMaps[i], defaultDuelistMaps[i]);
            }
        }
        RebuildStringCache();
    }

    public static void OnEnemiesChanged()
    {
        for (int i = 0; i < DuelistMaps.Length; i++)
        {
            if (i > 0 && i < 20)
            {
                DuelistMaps[i].Edited = Enemies.EnemyNameList[i].Edited;

            }
            else if (i == 20 || i == 21)
            {

                DuelistMaps[i].Edited = $"{Enemies.EnemyNameList[i]} ({Deck.DeckList[i + 26].DeckLeader.Name.Current})";
            }
        }
        RebuildStringCache();
    }

    public static void RebuildStringCache(bool both = false)
    {
        currentMapNameCache = DuelistMaps.Select(c => c.Current ?? "").ToArray();
        if (both)
        {
            defaultMapNameCache = DuelistMaps.Select(c => c.Default ?? "").ToArray();
        }

    }

    public static string[] GetCardStringArray(bool getDef = false)
    {
        if (UserSettings.UseDefaultNames || getDef)
        {
            return defaultMapNameCache;
        }
        return currentMapNameCache;

    }
}