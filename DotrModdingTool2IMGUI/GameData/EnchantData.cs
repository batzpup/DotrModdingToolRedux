namespace DotrModdingTool2IMGUI;

public class EnchantData
{
    public static List<byte> EnchantIds = new List<byte>();
    public static List<ushort> EnchantScores = new List<ushort>();
    public static byte[] EquipScoreBytes => EnchantScores.SelectMany(BitConverter.GetBytes).ToArray();

     public static ModdedStringName GetEquipName(int index)
    {
       return Card.GetNameByIndex(Card.EquipCardStartIndex + index);
    }

    public static string GetEnchantIdName(int id)
    {
        return Enum.GetName<EnchantId>((EnchantId)id) ?? "Bad Enchant";
    }
       public static string GetEnchantScoreName(int id)
    {
        return Enum.GetName<EnchantScore>((EnchantScore)id) ?? "Buff Amount";
    }
}

public enum EnchantId : byte
{
    Stat_Buff,
    Additional_Effect,
    Cursebreaker,
    Paralyzing_Potion,
    Fusion_Equips
}

public enum EnchantScore : ushort
{
    Riryoku,
    Multiply,
    Sword_Of_Dragon_Soul,
    Enchanted_Javelin,
    Anti_Magic_Fragrance,
    Crush_Card,
    Elegant_Egotist,
    Cocoon_Of_Evolution,
    Metalmorph,
    Insect_Imitation,
    CBreaker_Or_ParalyzingPot = 666
}