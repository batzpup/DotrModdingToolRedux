using System.Numerics;
using ImGuiNET;
using NativeFileDialogSharp;
using Raylib_cs;
using ImGui = ImGuiNET.ImGui;
namespace DotrModdingTool2IMGUI;

public class MapEditorWindow : IImGuiWindow
{
    DataAccess dataAccess;



    //Maps data
    DotrMap currentMap;
    int currentMapIndex;
    int treasureComboIndex;
    uint treasureHighlightColour = ImGui.ColorConvertFloat4ToU32(new Vector4(0.0f, 0.8f, 0.0f, 0.4f));
    IntPtr currentMapPaletteImage;
    Terrain currentPaletteTerrain = Terrain.Normal;

    Vector2 paletteImageSize = new Vector2(128, 128);
    TreasureCard currentTreasureCard;


    public MapEditorWindow(ImFontPtr fontPtr)
    {
        dataAccess = DataAccess.Instance;
        LoadDefaultMapsAll();
        currentMapPaletteImage = GlobalImages.Instance.Terrain[ETerrainImages.Normal];
        EditorWindow.OnIsoLoaded += onIsoLoaded;
    }

    void onIsoLoaded()
    {
    }


    public void Render()
    {
        ImGui.BeginChild("leftPanel", new Vector2(ImGui.GetContentRegionAvail().X * 0.33f, ImGui.GetContentRegionAvail().Y));
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
            ImGui.Text($" Please load an ISO to see treasures");
            return;
        }

        ImGui.NewLine();
        float availableWidth = ImGui.GetContentRegionAvail().X;
        if (currentMapIndex < DataAccess.TreasureCardCount)
        {
            if (Enum.IsDefined(typeof(EEnemyImages), currentMapIndex))
            {
                ImGui.Image(GlobalImages.Instance.Enemies[(EEnemyImages)currentMapIndex], ImageHelper.DefaultImageSize);
                ImGui.SameLine();
            }

        }

        if (currentMapIndex < DataAccess.TreasureCardCount)
        {
            currentTreasureCard = TreasureCards.Instance.Treasures.First(i => i.EnemyIndex == currentMapIndex);
            if (currentTreasureCard.CardIndex != 999)
            {
                ImGui.Dummy(new Vector2(availableWidth / 32, 0));
                ImGui.SameLine();

                ImGui.Image(GlobalImages.Instance.OriginalCards[currentTreasureCard.CardName.Default], ImageHelper.DefaultImageSize);
            }
            ImGui.Text($"{currentTreasureCard.EnemyName}'s Treasure:");
            ImGui.SetNextItemWidth(availableWidth);
            if (ImGui.BeginCombo("##Hidden Card", currentTreasureCard.CardName.Current, ImGuiComboFlags.HeightLarge))
            {
                foreach (var card in CardConstant.List)
                {
                    bool isSelected = TreasureCards.Instance.Treasures[treasureComboIndex].CardIndex == card.Index;

                    if (ImGui.Selectable(card.Name.Current, isSelected))
                    {
                        currentTreasureCard.CardIndex = card.Index;
                    }
                    if (isSelected)
                    {
                        ImGui.SetItemDefaultFocus();
                    }
                    if (ImGui.IsItemHovered())
                    {
                        GlobalImgui.RenderTooltipCardImage(card.Name.Default);
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
        string[] maps = Map.DuelistMaps.Select(c => c.Current ?? "").ToArray();

        int visibleItems = 7;
        float itemHeight = ImGui.GetTextLineHeightWithSpacing();
        float listHeight = visibleItems * itemHeight + ImGui.GetStyle().FramePadding.Y * 2;

        if (ImGui.BeginListBox("##Maps", new Vector2(-1, listHeight)))
        {
            for (int i = 0; i < maps.Length; i++)
            {
                bool selected = (i == currentMapIndex);
                if (ImGui.Selectable(maps[i], selected))
                    currentMapIndex = i;

                if (ImGui.IsItemHovered())
                {
                    if (UserSettings.ToggleImageTooltips)
                    {
                        ImGui.BeginTooltip();
                        DrawMiniMap(i, Raylib.GetScreenWidth() / 8f, Raylib.GetScreenWidth() / 8f);
                        ImGui.EndTooltip();
                    }
                }

                if (selected)
                    ImGui.SetItemDefaultFocus();
            }
            ImGui.EndListBox();
        }
        ImGui.PushFont(FontManager.GetBestFitFont("Make Current Map Default", false, FontManager.FontFamily.NotoSansJP));
        ImGui.Dummy(new Vector2(0, ImGui.GetContentRegionAvail().Y / 128));
        float availableSpace = ImGui.GetContentRegionAvail().X;
        float lineWidth = 0;

        GlobalImgui.AutoWrappingButton("Load Default Maps", () => LoadDefaultMapsAll(), ref lineWidth, availableSpace);
        GlobalImgui.AutoWrappingButton("Make Current Map Default", () => LoadDefaultMap(currentMapIndex), ref lineWidth, availableSpace);
        GlobalImgui.AutoWrappingButton("Save Current Map", () =>
        {
            Console.WriteLine("Saving current Maps");
            SaveCurrentMap();
        }, ref lineWidth, availableSpace, !DataAccess.Instance.IsIsoLoaded);
        GlobalImgui.AutoWrappingButton("Save all maps to file", () =>
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
        }, ref lineWidth, availableSpace);

        GlobalImgui.AutoWrappingButton("Load all maps from file", () =>
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
        }, ref lineWidth, availableSpace);

        ImGui.PopFont();
    }
    

  
    void DrawMap()
    {
        currentMap = dataAccess.maps[currentMapIndex];

        ImGui.BeginChild("MapDisplay");

        ImGui.Text("Left click and/or drag to paint with the current tile.");
        ImGui.Text("Right click to move the hidden card.");

        Vector2 availableSpace = ImGui.GetContentRegionAvail();
        Vector2 spacing = ImGui.GetStyle().ItemSpacing;

        float leftIndent = availableSpace.X / 8f;

        float totalHorizontalSpacing =
            (spacing.X * 6) +
            (ImGui.GetStyle().FramePadding.X * 2 * 7) +
            leftIndent;

        float maxCellWidth = (availableSpace.X - totalHorizontalSpacing) / 7f;

        float totalVerticalSpacing =
            (spacing.Y * 8) +
            (ImGui.GetStyle().FramePadding.Y * 2 * 9);

        float maxCellHeight =
            (availableSpace.Y - totalVerticalSpacing - availableSpace.Y / 32f) / 9f;

        float imageSize = MathF.Max(1, MathF.Min(maxCellWidth, maxCellHeight));

        Vector2 tileSize = new Vector2(imageSize, imageSize);

        Vector2 mousePos = ImGui.GetMousePos();

        ImGui.Dummy(new Vector2(0, availableSpace.Y / 16f));
        ImGui.Indent(leftIndent);

            
        Vector2 gridOrigin = ImGui.GetCursorScreenPos();

        Vector2 step = tileSize + spacing;

        bool isMouseDragging = ImGui.IsMouseDown(0);
        var drawList = ImGui.GetForegroundDrawList();

        for (int i = 0; i < 49; i++)
        {
            int x = i % 7;
            int y = i / 7;

            Vector2 tilePos = new Vector2(
                gridOrigin.X + x * step.X,
                gridOrigin.Y + y * step.Y
            );

            var tex = GlobalImages.Instance.Terrain[
                Enum.Parse<ETerrainImages>(currentMap.tiles[x, y].ToString())
            ];

            ImGui.SetCursorScreenPos(tilePos);

            if (ImGui.ImageButton($"Map{x},{y}", tex, tileSize))
            {
                currentMap.tiles[x, y] = currentPaletteTerrain;
            }


            if (DataAccess.Instance.IsIsoLoaded && currentMapIndex < 22)
            {

                if (currentTreasureCard.Column == x && currentTreasureCard.Row == y)
                {
                    drawList.AddRectFilled(
                        tilePos + spacing * 0.5f,
                        tilePos + tileSize + spacing * 0.5f,
                        treasureHighlightColour
                    );
                }
            }


            bool isInSquare =
                mousePos.X >= tilePos.X &&
                mousePos.X < tilePos.X + tileSize.X &&
                mousePos.Y >= tilePos.Y &&
                mousePos.Y < tilePos.Y + tileSize.Y;

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
        }



        for (int i = 0; i < 7; i++)
        {

            string label = i.ToString();
            Vector2 textSize = ImGui.CalcTextSize(label);

            Vector2 topPos = new Vector2(
                gridOrigin.X + i * step.X + tileSize.X * 0.5f - textSize.X * 0.5f,
                gridOrigin.Y - ImGui.GetFontSize() - spacing.Y * 0.5f
            );


            Vector2 bottomPos = new Vector2(
                gridOrigin.X - spacing.X * 2,
                gridOrigin.Y + i * step.Y + tileSize.Y * 0.5f - textSize.Y * 0.5f
            );

            drawList.AddText(bottomPos, ImGui.GetColorU32(ImGuiCol.Text), label);
            if (i != 3)
            {
                drawList.AddText(topPos, ImGui.GetColorU32(ImGuiCol.Text), label);
            }

        }


        float gridWidth = 7 * step.X - spacing.X;
        float gridHeight = 7 * step.Y - spacing.Y;

        Vector2 gridCenter = new Vector2(
            gridOrigin.X + gridWidth * 0.5f,
            gridOrigin.Y + gridHeight * 0.5f
        );


        Vector2 bottomImagePos = new Vector2(
            gridCenter.X - tileSize.X * 0.5f,
            gridCenter.Y + (gridWidth / 7) * 4 - tileSize.Y * 0.5f - spacing.X
        );
        Vector2 topImagePos = new Vector2(
            gridCenter.X - tileSize.X * 0.5f,
            gridCenter.Y - (gridWidth / 7) * 4 - tileSize.Y * 0.5f + spacing.X
        );

        drawList.AddImage(
            GlobalImages.Instance.Terrain[ETerrainImages.WhiteRose],
            topImagePos + spacing * 0.5f,
            topImagePos + tileSize + spacing * 0.5f
        );

        drawList.AddImage(
            GlobalImages.Instance.Terrain[ETerrainImages.RedRose],
            bottomImagePos + spacing * 0.5f,
            bottomImagePos + tileSize + spacing * 0.5f
        );

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