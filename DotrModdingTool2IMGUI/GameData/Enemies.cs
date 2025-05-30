namespace DotrModdingTool2IMGUI;

public class Enemies
{
    public static string[] nameList = new string[] {
        "Simon McMooran?",
        "Seto",
        "Weevil Underwood",
        "Rex Raptor",
        "Keith",
        "Ishtar",
        "Necromancer",
        "Darkness-ruler",
        "Labyrinth-ruler",
        "Pegasus Crawford",
        "Richard Slysheen of York",
        "Tea",
        "T. Tristan Grey",
        "Margaret Mai Beaufort",
        "Mako",
        "Joey",
        "J. Shadi Morton",
        "Jasper Dice Tudor",
        "Bakura",
        "Yugi",
        "Manawyddan fab Llyr (vs. White Rose)",
        "Manawyddan fab Llyr (vs. Red Rose)",
        "Deck Master K",
        "Deck Master I",
        "Deck Master T",
        "Deck Master S"
    };
    public static void LoadEnemies(byte[] bytes)
    {
        for (int  bi = 0; bi < bytes.Length;  bi += DataAccess.EnemyAiByteLength)
        {
            byte[] aiBytes = new byte[] { bytes[bi], bytes[bi + 1], bytes[bi + 2], bytes[bi + 3] };
            EnemyList.Add(new Enemy(bi/4, aiBytes));
        }
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
    public string Name { get; }
    public Ai AI { get; set; }

    

    public Enemy(int index, byte[] aiBytes)
    {
        Index = index;
        Name = GetEnemyNameByIndex(index);
        AI = new Ai(aiBytes);
    }

    public static string GetEnemyNameByIndex(int index)
    {
        return Enemies.nameList.ElementAtOrDefault(index) == null ? "???" : Enemies.nameList[index];
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