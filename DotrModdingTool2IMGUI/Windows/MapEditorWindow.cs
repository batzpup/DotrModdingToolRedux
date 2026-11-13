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

    // UI + Polish Color Palette
    int DynamicPaletteColumnCount;
    ImFontPtr DynamicTextFont;
    int DynamicTextFontSize;
    Vector4 WarningColor = new Vector4(192f / 256f, 100f / 256f,  24f / 256f, 0.80f); //Hex #C06418
    Vector4 WarningHover = new Vector4(234f / 256f,  96f / 256f,  72f / 256f, 0.92f); //Hex #EA6048
    Vector4 LoadColor = new Vector4(0.00f, 0.80f, 0.00f, 0.80f);
    Vector4 LoadHover = new Vector4(0.24f, 0.85f, 0.24f, 0.92f);
    Vector4 SaveColor    = new Vector4(192f / 256f,  16f / 256f, 176f / 256f, 0.80f); //Hex #C010B0
    Vector4 SaveHover    = new Vector4(198f / 256f,  64f / 256f, 188f / 256f, 0.92f); //Hex #C640BC

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
        if (!DataAccess.Instance.IsIsoLoaded)
        {
            ImGui.Text($"Please load ISO file");
            return;
        }
        
        ImGui.BeginChild("leftPanel", new Vector2(ImGui.GetContentRegionAvail().X * 0.33f, ImGui.GetContentRegionAvail().Y), ImGuiChildFlags.None, ImGuiWindowFlags.AlwaysVerticalScrollbar);
        float indent = ImGui.GetContentRegionAvail().X / 32;
        ImGui.Indent(indent);
        DrawMapExtras();
        DrawTreasureDetails();
        DrawMapPalette();
        ImGui.Unindent(indent);
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
        ImGui.PushStyleVar(ImGuiStyleVar.CellPadding, new Vector2(3f, 3f));

        float availableWidth = ImGui.GetContentRegionAvail().X;
        float mapXPad = ImGui.GetStyle().CellPadding.X;

        ImGui.NewLine();
        ImGui.Text("Map Editor Palette");
        
        // Dynamic Icon and Font Size Support
        float cellSize = availableWidth / 5 - 12;                      // 5 as base Palette Column amount
        float cellResize = Math.Clamp(cellSize, 36, 72);
        Vector2 imageSize = new Vector2(cellResize, cellResize);
        //Console.WriteLine("<MapPalette> Image" + imageSize + " Pad " + mapXPad);
        
        DynamicPaletteColumnCount = ((availableWidth >= 270) ? 5 : 4); // CR every 4 when 5 does not fit
        if      (availableWidth < 270) { DynamicTextFontSize = 20; }
        else if (availableWidth < 290) { DynamicTextFontSize = 22; }
        else if (availableWidth < 330) { DynamicTextFontSize = 24; }
        else if (availableWidth < 370) { DynamicTextFontSize = 28; }
        else                           { DynamicTextFontSize = 32; }
        DynamicTextFont = FontManager.GetFont(FontManager.FontFamily.NotoSansJP, DynamicTextFontSize);
        //Console.WriteLine("<MapPalette> Room " + availableWidth + " Font " + DynamicTextFontSize);

        /* Fixing layout for Current Tile into a Table similar to the Palette below. See below.
         * (Ignore) Alignment padding on Column 0 ; Tile Icon on Column 1, Tile Text on Columns 2+. */
        if (ImGui.BeginTable("MapCurrentTile", 3, ImGuiTableFlags.None, ImGui.GetContentRegionAvail()))
        {
            ImGui.TableSetupColumn("##CurrentTilePadding", 0, mapXPad * 2);
            ImGui.TableSetupColumn("##CurrentTileIcon", 0, availableWidth * 0.26f);
            ImGui.TableSetupColumn("##CurrentTileText", 0, ImGui.GetContentRegionAvail().X);

            ImGui.TableNextRow();
            // Nothing in Column 0 ; it's only there to fix ImGui alignment

            ImGui.TableSetColumnIndex(1);
            // Fixing ImGui alignment issues and giving room for Text padding
            ImGui.Dummy(new Vector2(0, 6));
            ImGui.Image(currentMapPaletteImage, imageSize);

            ImGui.TableNextColumn();
            // Fixing ImGui alignment issues
            float mapYTextPad = 2 + Math.Min(availableWidth / 220, 2.2f);
            ImGui.Dummy(new Vector2(0, mapYTextPad));
            //Console.WriteLine("<MapPaletteText> Pad " + mapYTextPad);
            ImGui.PushFont(DynamicTextFont);
            ImGui.Text($"Current Tile :\n{currentPaletteTerrain}");
            ImGui.PopFont();

            ImGui.EndTable();
        }

        /* The fact ImGui automatically handles new Rows with TableNextColumn calls on last Column,
         * allows pretty easy dynamic Table column counting depending on window size. */
        if (ImGui.BeginTable("MapTileSelect", DynamicPaletteColumnCount, ImGuiTableFlags.None))
        {
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
        }

        ImGui.PopStyleVar();
    }

    void DrawMapExtras()
    {
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
        
        ImGui.Dummy(new Vector2(0, ImGui.GetContentRegionAvail().Y / 128));
        float availableSpace = ImGui.GetContentRegionAvail().X;
        Vector2 fullTextSize = ImGui.CalcTextSize("Restore current Map to Default");
        Vector2 mapButtonSize = new Vector2( Math.Max(availableSpace * 0.8f, fullTextSize.X + 12), fullTextSize.Y + 8);
        ImGui.PushStyleVar(ImGuiStyleVar.ButtonTextAlign, new Vector2(0.0f, 0.5f)); // Align Text to Left
        
        // Moving Load from File between Map List and Restore Vanilla for safety
        // Bits of extra polish with colors and size
        ImGui.BeginDisabled(!DataAccess.Instance.IsIsoLoaded);
        ImGui.PushStyleColor(ImGuiCol.Button, LoadColor);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, LoadHover);
        if (ImGui.Button("Load all Maps from File", mapButtonSize))
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
        ImGui.PopStyleColor(2);
        
        ImGui.PushStyleColor(ImGuiCol.Button, WarningColor);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, WarningHover);
        if (ImGui.Button("Restore all Maps to Default", mapButtonSize))
        {
            LoadDefaultMapsAll();
        }
        if (ImGui.Button("Restore current Map to Default", mapButtonSize))
        {
            LoadDefaultMap(currentMapIndex);
        }
        ImGui.PopStyleColor(2);
        
        // Separating Restore Vanilla and Save for safety
        ImGui.NewLine();

        ImGui.PushStyleColor(ImGuiCol.Button, SaveColor);
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, SaveHover);
        if (ImGui.Button("Save current Map", mapButtonSize))
        {
            SaveCurrentMap();
        }
        if (ImGui.Button("Save all Maps to File", mapButtonSize))
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
        ImGui.PopStyleColor(2);
        
        ImGui.PopStyleVar();
        ImGui.EndDisabled();
    }
    

  
    void DrawMap()
    {
        currentMap = dataAccess.maps[currentMapIndex];

        ImGui.BeginChild("MapDisplay");

        ImGui.Text("Left click and/or drag to paint with the current tile.");
        ImGui.Text("Right click to move the hidden card.");

        // Align map away from text
        ImGui.NewLine();

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