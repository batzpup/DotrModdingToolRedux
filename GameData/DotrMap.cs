using ImGuiNET;
namespace DotrModdingTool2IMGUI;

public class DotrMap
{
    public Terrain[,] tiles;
  

    public DotrMap()
    {
        tiles = new Terrain[7, 7];
        int x;
        int y;
        for (var i = 0; i < 49; i++)
        {
            x = i % 7;
            y = i / 7;
            tiles[x, y] = Terrain.Normal;
        }
    }

    public DotrMap(byte[] arr)
    {
        // loads a map from a byte array.
        tiles = new Terrain[7, 7];
        int x;
        int y;
        for (var i = 0; i < 49; i++)
        {
            x = i % 7;
            y = i / 7;
            try
            {
                tiles[x,y] = (Terrain)arr[i];
            }
            catch
            {
              
            }
        }
       
    }
}