using System.Numerics;
using ImGuiNET;
using NativeFileDialogSharp;
using ImGui = ImGuiNET.ImGui;
namespace DotrModdingTool2IMGUI;

public class MapEditorWindow : IImGuiWindow
{
    DataAccess dataAccess;
    ImFontPtr largerFont;


    //Maps data
    DotrMap currentMap;
    int currentMapIndex;
    int treasureComboIndex;
    uint treasureHighlightColour = ImGui.ColorConvertFloat4ToU32(new Vector4(0.0f, 0.8f, 0.0f, 0.4f));
    IntPtr currentMapPaletteImage;
    Terrain currentPaletteTerrain = Terrain.Normal;
    bool isIsoLoaded = false;
    Vector2 paletteImageSize = new Vector2(128, 128);
    TreasureCard currentTreasureCard;

    string[] duelistMaps = new string[] {
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

    public MapEditorWindow(ImFontPtr fontPtr)
    {
        dataAccess = DataAccess.Instance;
        largerFont = fontPtr;
        LoadDefaultMapsAll();
        currentMapPaletteImage = GlobalImages.Instance.Terrain[ETerrainImages.Normal];


    }

    public void Render()
    {
        Vector2 availableSpace = new Vector2(ImGui.GetWindowSize().X / 3, ImGui.GetContentRegionAvail().Y);

        ImGui.BeginChild("leftPanel", availableSpace);
        DrawMapExtras();
        DrawTreasureDetails();
        DrawMapPalette();
        ImGui.EndChild();
        ImGui.SameLine();
        DrawMap();


    }

    public void LoadTreasureCardData()
    {
        byte[] treasureCardBytes = dataAccess.GetTreasureCardData();
        TreasureCards.Instance.InitTreasureData(treasureCardBytes);
    }

    void DrawTreasureDetails()
    {
        if (!DataAccess.Instance.IsIsoLoaded)
        {
            ImGui.Text($" Please load an iso to see treasures");
            return;
        }

        ImGui.NewLine();
        if (currentMapIndex < DataAccess.TreasureCardCount)
        {
            if (Enum.IsDefined(typeof(EEnemyImages), currentMapIndex))
            {
                ImGui.Image(GlobalImages.Instance.Enemies[(EEnemyImages)currentMapIndex], new Vector2(128, 128));
                ImGui.SameLine();
            }

        }

        if (currentMapIndex < DataAccess.TreasureCardCount)
        {
            currentTreasureCard = TreasureCards.Instance.Treasures.First(i => i.EnemyIndex == currentMapIndex);
            if (currentTreasureCard.CardIndex != 999)
            {
                ImGui.Dummy(new Vector2(ImGui.GetContentRegionAvail().X / 32, 0));
                ImGui.SameLine();
                ImGui.Image(GlobalImages.Instance.Cards[currentTreasureCard.CardName], new Vector2(128, 128));
            }
            ImGui.Text($"{currentTreasureCard.EnemyName}'s Treasure:");
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            if (ImGui.BeginCombo("##Hidden Card", currentTreasureCard.CardName, ImGuiComboFlags.HeightLarge))
            {
                foreach (var card in CardConstant.List)
                {
                    bool isSelected = TreasureCards.Instance.Treasures[treasureComboIndex].CardIndex == card.Index;
                    if (ImGui.Selectable(card.Name, isSelected))
                    {
                        currentTreasureCard.CardIndex = card.Index;
                    }
                    if (isSelected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        GlobalImgui.RenderTooltipCardImage(card.Name);
                    }
                }
                ImGui.EndCombo();
            }
        }
    }

    public void Free()
    {

    }


    void DrawMapPalette()
    {

        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(5f, 5f));
        float availableWidth = ImGui.GetContentRegionAvail().X;
        float cellSize = availableWidth / ImGui.GetStyle().CellPadding.X - 20;
        Vector2 imageSize = new Vector2(cellSize, cellSize);


        Vector2 textSize = ImGui.CalcTextSize($"Current tile:\n{currentPaletteTerrain}");
        float textX = (availableWidth - textSize.X) * 0.5f + ImGui.GetStyle().CellPadding.X * 3;
        ImGui.SetCursorPosX(ImGui.GetCursorPosX() + textX);
        ImGui.Text($"Current tile:\n{currentPaletteTerrain}");

        ImGui.BeginTable("MapImageGrid", 5, ImGuiTableFlags.SizingStretchSame);

        ImGui.TableNextColumn();
        ImGui.TableNextColumn();
        ImGui.TableNextColumn();


        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        ImGui.TableNextColumn();
        ImGui.TableNextColumn();
        ImGui.Image(currentMapPaletteImage, imageSize);

        ImGui.TableNextColumn();


        ImGui.TableNextRow();
        ImGui.TableNextColumn();
        if (ImGui.ImageButton("Forest_Image", GlobalImages.Instance.Terrain[ETerrainImages.Forest], imageSize))
        {
            currentPaletteTerrain = Terrain.Forest;
            currentMapPaletteImage = GlobalImages.Instance.Terrain[ETerrainImages.Forest];
        }

        ImGui.TableNextColumn();
        if (ImGui.ImageButton("Wasteland_Image", GlobalImages.Instance.Terrain[ETerrainImages.Wasteland], imageSize))
        {
            currentPaletteTerrain = Terrain.Wasteland;
            currentMapPaletteImage = GlobalImages.Instance.Terrain[ETerrainImages.Wasteland];
        }


        ImGui.TableNextColumn();
        if (ImGui.ImageButton("Mountain_Image", GlobalImages.Instance.Terrain[ETerrainImages.Mountain], imageSize))
        {
            currentPaletteTerrain = Terrain.Mountain;
            currentMapPaletteImage = GlobalImages.Instance.Terrain[ETerrainImages.Mountain];
        }


        ImGui.TableNextColumn();
        if (ImGui.ImageButton("Meadow_Image", GlobalImages.Instance.Terrain[ETerrainImages.Meadow], imageSize))
        {
            currentPaletteTerrain = Terrain.Meadow;
            currentMapPaletteImage = GlobalImages.Instance.Terrain[ETerrainImages.Meadow];
        }


        ImGui.TableNextColumn();
        if (ImGui.ImageButton("Sea_Image", GlobalImages.Instance.Terrain[ETerrainImages.Sea], imageSize))
        {
            currentPaletteTerrain = Terrain.Sea;
            currentMapPaletteImage = GlobalImages.Instance.Terrain[ETerrainImages.Sea];
        }

        ImGui.TableNextColumn();
        if (ImGui.ImageButton("Dark_Image", GlobalImages.Instance.Terrain[ETerrainImages.Dark], imageSize))
        {
            currentPaletteTerrain = Terrain.Dark;
            currentMapPaletteImage = GlobalImages.Instance.Terrain[ETerrainImages.Dark];
        }

        ImGui.TableNextColumn();
        if (ImGui.ImageButton("Toon_Image", GlobalImages.Instance.Terrain[ETerrainImages.Toon], imageSize))
        {
            currentPaletteTerrain = Terrain.Toon;
            currentMapPaletteImage = GlobalImages.Instance.Terrain[ETerrainImages.Toon];
        }

        ImGui.TableNextColumn();
        if (ImGui.ImageButton("Normal_Image", GlobalImages.Instance.Terrain[ETerrainImages.Normal], imageSize))
        {
            currentPaletteTerrain = Terrain.Normal;
            currentMapPaletteImage = GlobalImages.Instance.Terrain[ETerrainImages.Normal];
        }

        ImGui.TableNextColumn();
        if (ImGui.ImageButton("Lab_Image", GlobalImages.Instance.Terrain[ETerrainImages.Labyrinth], imageSize))
        {
            currentPaletteTerrain = Terrain.Labyrinth;
            currentMapPaletteImage = GlobalImages.Instance.Terrain[ETerrainImages.Labyrinth];
        }

        ImGui.TableNextColumn();
        if (ImGui.ImageButton("Crush_Image", GlobalImages.Instance.Terrain[ETerrainImages.Crush], imageSize))
        {
            currentPaletteTerrain = Terrain.Crush;
            currentMapPaletteImage = GlobalImages.Instance.Terrain[ETerrainImages.Crush];
        }
        ImGui.EndTable();
        ImGui.PopStyleVar();
    }

    void DrawMapExtras()
    {

        ImGui.Indent(ImGui.GetContentRegionAvail().X / 32);
        ImGui.Text("Maps");
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);


        if (ImGui.BeginListBox("##Maps", new Vector2(0, ImGui.GetTextLineHeightWithSpacing() * 8)))
        {
            for (int i = 0; i < duelistMaps.Length; i++)
            {
                bool isSelected = (currentMapIndex == i);
                if (ImGui.Selectable(duelistMaps[i], isSelected))
                {
                    currentMapIndex = i;
                }
                if (isSelected)
                {
                    ImGui.SetItemDefaultFocus();
                }
                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    DrawMiniMap(i, 256, 256);
                    ImGui.EndTooltip();
                }

            }
            ImGui.EndListBox();
        }
        ImGui.Dummy(new Vector2(0, ImGui.GetContentRegionAvail().Y / 128));
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 3);
        if (ImGui.Button("Load Default Maps"))
        {
            LoadDefaultMapsAll();
        }
        ImGui.SameLine();
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 3);
        if (ImGui.Button("Make Current Map Default"))
        {
            LoadDefaultMap(currentMapIndex);
        }
        ImGui.SameLine();
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 3);
        if (ImGui.Button("Save Current Map"))
        {
            Console.WriteLine("Saving current Maps");
            SaveCurrentMap();
        }
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 3);
        if (ImGui.Button("Save all maps to file"))
        {
            var result = Dialog.FileSave("txt");
            if (result.IsOk)
            {
                int x;
                int y;
                string textData = "";
                for (int mapIndex = 0; mapIndex < dataAccess.maps.Length; mapIndex++)
                {
                    DotrMap map = dataAccess.maps[mapIndex];

                    for (int tileIndex = 0; tileIndex < map.tiles.Length; tileIndex++)
                    {
                        x = tileIndex % 7;
                        y = tileIndex / 7;
                        textData += ((int)map.tiles[x, y]).ToString();
                    }
                }
                File.WriteAllText(result.Path, textData);
            }

        }
        ImGui.SameLine();
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X / 3);
        if (ImGui.Button("Load all maps from file"))
        {
            var result = Dialog.FileOpen("txt");
            if (result.IsOk)
            {
                int x;
                int y;
                string mapTextData = File.ReadAllText(result.Path);
                int index = 0;
                for (int mapIndex = 0; mapIndex < dataAccess.maps.Length; mapIndex++)
                {
                    DotrMap map = dataAccess.maps[mapIndex];
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
        }

    }

    void DrawMap()
    {
        currentMap = dataAccess.maps[currentMapIndex];

        ImGui.BeginChild("MapDisplay");
        ImGui.Text("Left click and/or drag to paint with the current tile.");
        ImGui.Text("Right click to move the hidden card.");
        Vector2 availableSpace = ImGui.GetContentRegionAvail();
        Vector2 spacing = ImGui.GetStyle().ItemSpacing;
        float maxCellWidth = availableSpace.X - (spacing.X * 6) / 7;
        float maxCellHeight = availableSpace.Y / 9 - availableSpace.Y / 64;
        float imageSize = MathF.Min(maxCellWidth, maxCellHeight);

        Vector2 tileSize = new Vector2(imageSize, imageSize);
        Vector2 mousePos = ImGui.GetMousePos();

        ImGui.Dummy(new Vector2(0, availableSpace.Y / 32));
        ImGui.Indent(availableSpace.X / 8f);
        //ImageSize + padding * 3 tiles
        ImGui.Indent((tileSize.X + 17) * 3);
        ImGui.Image(GlobalImages.Instance.Terrain[ETerrainImages.WhiteRose], tileSize);
        ImGui.Unindent((tileSize.X + 17) * 3);
        bool isMouseDragging = ImGui.IsMouseDown(0);

        int x;
        int y;
        for (int i = 0; i < 49; i++)
        {
            x = i % 7;
            y = i / 7;
            Vector2 tilePos = ImGui.GetCursorScreenPos();
            if (ImGui.ImageButton($"Map{x},{y}", GlobalImages.Instance.Terrain[Enum.Parse<ETerrainImages>(currentMap.tiles[x, y].ToString())],
                    tileSize))
            {
                currentMap.tiles[x, y] = currentPaletteTerrain;
            }
            if (DataAccess.Instance.IsIsoLoaded && currentMapIndex < 22)
            {
                if (currentTreasureCard.Column == x && currentTreasureCard.Row == y)
                {
                    var temp = ImGui.GetForegroundDrawList();
                    temp.AddRectFilled(tilePos + spacing / 2f,
                        new Vector2(tilePos.X + tileSize.X + spacing.X / 2f, tilePos.Y + tileSize.Y + spacing.Y / 2f + 1), treasureHighlightColour);
                }
            }
            bool isInSquare = mousePos.X >= tilePos.X && mousePos.X < tilePos.X + tileSize.X &&
                              mousePos.Y >= tilePos.Y && mousePos.Y < tilePos.Y + tileSize.Y;
            if (isInSquare)
            {
                if (isMouseDragging)
                {
                    currentMap.tiles[x, y] = currentPaletteTerrain;
                }
                if (DataAccess.Instance.IsIsoLoaded && currentMapIndex < 22)
                {
                    if (ImGui.GetIO().MouseClicked[1])
                    {
                        currentTreasureCard.Column = (byte)x;
                        currentTreasureCard.Row = (byte)y;
                    }
                }
            }
            if (x < 6) ImGui.SameLine();

        }
        ImGui.Indent((tileSize.X + 17) * 3);
        ImGui.Image(GlobalImages.Instance.Terrain[ETerrainImages.RedRose], tileSize);
        ImGui.Unindent((tileSize.X + 17) * 3);
        ImGui.EndChild();
    }


    public void LoadMapData()
    {
        dataAccess.LoadMapsFromIso();
        currentMap = dataAccess.maps[0];
    }

    void LoadDefaultMapsAll()
    {

        for (int i = 0; i < dataAccess.maps.Length; i++)
        {
            dataAccess.maps[i] = new DotrMap(VanillaMapBytes.Maps[i]);
        }
        currentMap = dataAccess.maps[currentMapIndex];
        TreasureCards.Instance.Treasures = new List<TreasureCard>(TreasureCards.Instance.DefaultTreasures);
    }

    void LoadDefaultMap(int index)
    {
        dataAccess.maps[index] = new DotrMap(VanillaMapBytes.Maps[index]);
        currentMap = dataAccess.maps[index];
        if (index > 0 && index < 22)
        {
            TreasureCards.Instance.Treasures[index] = new TreasureCard(TreasureCards.Instance.DefaultTreasures[index].Bytes);
        }

    }

    void DrawMiniMap(int mapIndex, float width, float height)
    {
        currentMap = dataAccess.maps[mapIndex];
        Vector2 tileSize = new Vector2(width / 7, height / 7);

        for (int i = 0; i < 49; i++)
        {
            int x = i % 7;
            int y = i / 7;

            ImGui.SetCursorPos(new Vector2(x * tileSize.X + tileSize.X / 4, y * tileSize.Y));
            ImGui.Image(GlobalImages.Instance.Terrain[Enum.Parse<ETerrainImages>(currentMap.tiles[x, y].ToString())], tileSize);
        }
    }

    public void SaveAllMaps()
    {
        dataAccess.SaveMaps();
        dataAccess.SaveAllTreasureCards();
    }

    public void SaveCurrentMap()
    {
        dataAccess.SaveMap(currentMapIndex);
        if (currentMapIndex < DataAccess.TreasureCardCount)
        {
            dataAccess.SaveTreasureCard(currentMapIndex, currentTreasureCard.Bytes);
        }
    }
}