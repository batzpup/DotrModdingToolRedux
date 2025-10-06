using System.Collections;
namespace DotrModdingTool2IMGUI;

public  class MonsterEnchantData
{
    public static List<MonsterEnchantData> MonsterEnchantDataList = new List<MonsterEnchantData>();
    public BitArray Flags;
    
    public MonsterEnchantData(BitArray flags)
    {
        Flags = flags;
    }
    
    public ModdedStringName GetEquipName(int flagIndex)
    {
       return Card.GetNameByIndex(Card.EquipCardStartIndex + flagIndex);
    }
      public static byte[] Bytes
    {
        get { return MonsterEnchantDataList.SelectMany(a  => a.Flags.ToByteArray()).ToArray(); }
    }
}