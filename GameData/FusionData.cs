using System.Collections;
namespace DotrModdingTool2IMGUI;

public class FusionData
{
    public static Dictionary<int, FusionData> FusionTableData = new();

    public static byte[] Bytes
    {
        get { return FusionTableData.SelectMany(a => BitConverter.GetBytes(a.Value.fusionData)).ToArray(); }
    }


    public ushort lowerCardId;
    public ushort higherCardId;
    public ushort resultId;
    public uint fusionData;
    public string lowerCardName;
    public string higherCardName;
    public string cardResultName;


    public FusionData(byte[] data)
    {

        fusionData = (uint)(data[0] | (data[1] << 8) | (data[2] << 16) | (data[3] << 24));
        higherCardId = (ushort)(fusionData & 0x3FF); // First 10 bits
        lowerCardId = (ushort)((fusionData >> 10) & 0x3FF); // Next 10 bits
        resultId = (ushort)((fusionData >> 20) & 0x3FF); // Last 10 bits
        lowerCardName = Card.cardNameList[lowerCardId];
        higherCardName = Card.cardNameList[higherCardId];
        cardResultName = Card.cardNameList[resultId];
    }

    public void UpdateFusion()
    {

        OrderFusionMaterials();
        fusionData = (uint)(higherCardId |lowerCardId  << 10 | resultId << 20);
        lowerCardName = Card.cardNameList[lowerCardId];
        higherCardName = Card.cardNameList[higherCardId];
        cardResultName = Card.cardNameList[resultId];
    }

    void OrderFusionMaterials()
    {
        if (lowerCardId > higherCardId)
        {
            //Swap
            lowerCardId = (ushort)(higherCardId ^ lowerCardId);
            higherCardId = (ushort)(lowerCardId ^ higherCardId);
            lowerCardId = (ushort)(higherCardId ^ lowerCardId);
            //Apparently this is a thing
            (lowerCardName, higherCardName) = (higherCardName, lowerCardName);

        }
    }
}