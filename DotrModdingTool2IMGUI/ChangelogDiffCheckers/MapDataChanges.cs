using System.Text;
namespace DotrModdingTool2IMGUI.ChangelogDiffCheckers;

public class MapSnapshot
{
    public List<DotrMap> Maps;

    public MapSnapshot(DotrMap[] maps)
    {
        Maps = maps.Select(m => CloneMap(m)).ToList();
    }

    DotrMap CloneMap(DotrMap original)
    {
        return new DotrMap(original.Bytes);

    }
}

public class MapDiffChecker : IDiffChecker<MapSnapshot>
{
    static readonly Dictionary<Terrain, string> TerrainSymbols = new() {
        { Terrain.Forest, "Fr" },
        { Terrain.Wasteland, "Wl" },
        { Terrain.Mountain, "Mt" },
        { Terrain.Meadow, "Me" },
        { Terrain.Sea, "Se" },
        { Terrain.Dark, "Da" },
        { Terrain.Toon, "Tn" },
        { Terrain.Normal, "Nm" },
        { Terrain.Labyrinth, "Lb" },
        { Terrain.Crush, "Cr" }
    };

    public string GetTextMap(DotrMap map)
    {
        var sb = new StringBuilder();

        for (int y = 0; y < 7; y++)
        {
            for (int x = 0; x < 7; x++)
            {
                var terrain = map.tiles[x, y];
                string symbol = TerrainSymbols.GetValueOrDefault(terrain, "??");
                sb.Append(symbol);
                sb.Append(' ');
            }
            sb.AppendLine();
        }

        return sb.ToString();
    }

    public DiffResult CompareSnapshots(MapSnapshot oldSnapshot, MapSnapshot currentSnapshot)
    {
        DiffResult result = new DiffResult { Name = "Maps" };
        for (int i = 0; i < oldSnapshot.Maps.Count; i++)
        {
            DotrMap oldMap = oldSnapshot.Maps[i];
            DotrMap currentMap = currentSnapshot.Maps[i];


            if (!oldMap.Bytes.SequenceEqual(currentMap.Bytes))
            {
                string title = $"{Map.DuelistMaps[i].Current}:";
                string[] oldLines = GetTextMap(oldMap).Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                string[] newLines = GetTextMap(currentMap).Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
                var sb = new StringBuilder();

                for (int y = 0; y < 7; y++)
                {
                    string left = y < oldLines.Length ? oldLines[y] : "";
                    string right = y < newLines.Length ? newLines[y] : "";
                    left = left.PadRight(14);
                    if (y == 3)
                    {
                        sb.AppendLine($"{left}  ->  {right}");
                    }
                    else
                    {
                        sb.AppendLine($"{left}      {right}");
                    }
                }
                string indentedBlock = IndentAllLines(sb.ToString().TrimEnd(), "  ");

                int firstNewline = indentedBlock.IndexOf('\n');
                if (firstNewline > 0)
                {
                    string firstLine = indentedBlock[..firstNewline].TrimStart(' ', '\t');
                    indentedBlock = firstLine + indentedBlock[firstNewline..];
                }

                result.Add(title, indentedBlock);
            }
        }

        return result;
    }


    static string IndentAllLines(string text, string indent)
    {
        var lines = text.Split(new[] { "\r\n", "\n" }, StringSplitOptions.None);
        for (int i = 0; i < lines.Length; i++)
        {
            lines[i] = indent + lines[i];
        }
        return string.Join(Environment.NewLine, lines);
    }
}