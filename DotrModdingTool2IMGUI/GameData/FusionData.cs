using System.Collections;
using System.Text;
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
    public ModdedStringName lowerCardName;
    public ModdedStringName higherCardName;
    public ModdedStringName cardResultName;


    public FusionData()
    {

    }

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
        fusionData = (uint)(higherCardId | lowerCardId << 10 | resultId << 20);
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

    public static void ExportToCSV(string filePath)
    {
        var sb = new StringBuilder();
        sb.AppendLine(
            "LowerId,HigherId,ResultId,LowerName,HigherName,ResultName");

        foreach (var fusionData in FusionTableData.Values)
        {
            var row = new List<string>();
            row.Add(fusionData.lowerCardId.ToString());
            row.Add(fusionData.higherCardId.ToString());
            row.Add(fusionData.resultId.ToString());
            
            row.Add(fusionData.lowerCardName.Current.Replace(',',' '));
            row.Add(fusionData.higherCardName.Current.Replace(',',' '));
            row.Add(fusionData.cardResultName.Current.Replace(',',' '));
            sb.AppendLine(string.Join(",", row));
        }
        File.WriteAllText(filePath, sb.ToString());

    }

    public static void ImportFromCSV(string filePath)
    {
        FusionTableData.Clear();
        int index = 0;
        string[] lines = File.ReadAllLines(filePath);
        for (int i = 1; i < lines.Length; i++)
        {
            string line = lines[i];
            string[] values = line.Split(',');

            if (values.Length == 6) 
            {
                ushort lowerId = ushort.Parse(values[0]);
                ushort higherId = ushort.Parse(values[1]);
                ushort resultId = ushort.Parse(values[2]);
                
                FusionData data = new FusionData {
                    lowerCardId = lowerId,
                    higherCardId = higherId,
                    resultId = resultId,
                    fusionData = 0, //updated in updateFusion
                    lowerCardName = Card.cardNameList.Length > lowerId ? Card.cardNameList[lowerId] :new ModdedStringName($"Card_{lowerId}",$"Card_{lowerId}") ,
                    higherCardName = Card.cardNameList.Length > higherId ? Card.cardNameList[higherId] : new ModdedStringName($"Card_{higherId}",$"Card_{higherId}"),
                    cardResultName = Card.cardNameList.Length > resultId ? Card.cardNameList[resultId] : new ModdedStringName($"Card_{resultId}",$"Card_{resultId}")
                };
                data.UpdateFusion();
                FusionTableData[index] = data;
                index++;
            }
        }
        Console.WriteLine($"Imported {FusionTableData.Count} fusion data entries from {filePath}");
    }
}