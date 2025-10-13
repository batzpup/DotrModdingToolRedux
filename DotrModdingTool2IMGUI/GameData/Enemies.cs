namespace DotrModdingTool2IMGUI;

public class Enemies
{
    public static ModdedStringName[] EnemyNameList;
    static string[] currentEnemyNameCache;
    static string[] defaultEnemyNameCache;
    public static Action OnStringRebuilt;

    static string[] defaultNameList = new string[26] {
        "Simon McMooran", // 0
        "Seto", // 1
        "Weevil Underwood", // 2
        "Rex Raptor", // 3
        "Keith", // 4
        "Ishtar", // 5
        "Necromancer", // 6
        "Darkness-ruler", // 7
        "Labyrinth-ruler", // 8
        "Pegasus Crawford", // 9
        "Richard Slysheen of York", // 10
        "Tea", // 11
        "T.Tristan Grey", // 12
        "Margaret Mai Beaufort", // 13
        "Mako", // 14
        "Joey", // 15
        "J.Shadi Morton", // 16
        "Jasper Dice Tudor", // 17
        "Bakura", // 18
        "Yugi", // 19
        "Manawyddan fab Llyr (vs. White Rose)", // 20
        "Manawyddan fab Llyr (vs. Red Rose)", // 21
        "Deck Master K", // 22
        "Deck Master I", // 23
        "Deck Master T", // 24
        "Deck Master S" // 25
    };


    public static void ReloadStrings()
    {
        EnemyNameList = new ModdedStringName[defaultNameList.Length];
        for (int i = 0; i < defaultNameList.Length; i++)
        {
            if (i < 22)
            {
                EnemyNameList[i] = new ModdedStringName("", "") {
                    Default = defaultNameList[i],
                    Edited = StringEditor.StringTable[i + StringEditor.DuelistNameOffsetStart]
                };

            }
            else
            {
                EnemyNameList[i] = new ModdedStringName("", "") {
                    Default = defaultNameList[i],
                    Edited = StringEditor.StringTable[i - 22 + StringEditor.CustomDuelistNameStart]
                };
            }
        }
        RebuildStringCache();
    }

    public static void RebuildStringCache(bool both = false)
    {
        currentEnemyNameCache = EnemyNameList.Select(c => c.Current ?? "").ToArray();
        if (both)
        {
            defaultEnemyNameCache = EnemyNameList.Select(c => c.Default ?? "").ToArray();
        }

    }

    public static string[] GetEnemyNameArray(bool getDef = false)
    {
        if (UserSettings.UseDefaultNames || getDef)
        {
            return defaultEnemyNameCache;
        }
        return currentEnemyNameCache;

    }

    public static void LoadEnemies(byte[] bytes)
    {
        ReloadStrings();
        for (int bi = 0; bi < bytes.Length; bi += DataAccess.EnemyAiByteLength)
        {
            byte[] aiBytes = new byte[] { bytes[bi], bytes[bi + 1], bytes[bi + 2], bytes[bi + 3] };
            EnemyList.Add(new Enemy(bi / 4, aiBytes));
        }
        RebuildStringCache(true);
    }


    public static byte[] AiBytes
    {
        get { return EnemyList.SelectMany(x => x.AI.Bytes).ToArray(); }
    }


    public static List<Enemy> EnemyList { get; set; } = new List<Enemy>();
}

public class Enemy
{
    public int Index { get; }

    public ModdedStringName Name
    {
        get { return GetEnemyNameByIndex(Index); }
    }

    public Ai AI { get; set; }


    public Enemy(int index, byte[] aiBytes)
    {
        Index = index;
        AI = new Ai(aiBytes);
    }

    public static ModdedStringName GetEnemyNameByIndex(int index)
    {
        return Enemies.EnemyNameList.ElementAtOrDefault(index) == null ? new ModdedStringName("???", "???") : Enemies.EnemyNameList[index];

    }

    public int AiId
    {
        get { return this.AI.Id; }

        set
        {
            Ai ai = Ai.All.Find(x => x.Id == value);
            byte[] bytes = (byte[])ai.Bytes.Clone();
            // I'm not entirely sure why the bytes need flipped here, but they do.
            Array.Reverse(bytes);
            this.AI = new Ai(bytes);
        }
    }

    public string AiName
    {
        get { return this.AI.Name; }
    }
}