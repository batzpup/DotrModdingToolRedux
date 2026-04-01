using System.Numerics;
using System.Text;
using ImGuiNET;
using SkiaSharp;
namespace DotrModdingTool2IMGUI;

public class TextureEditorWindow : IImGuiWindow
{
    PaintCanvas? paintCanvas;
    PaintCanvas.Tool selectedTool = PaintCanvas.Tool.Pencil;
    float brushSize = 6f;
    float opacity = 1f;
    Vector4 brushColor = new(0, 0, 0, 1);
    bool wasPainting;
    int currentPalleteIndex = -1;
    List<SKColor> palette = new List<SKColor>();


    int currentPictureIndex;
    int currentPicPackIndex;
    int currentPicMiniIndex;
    int currentModelTexIndex;
    int currentTexEtcIndex;
    int currentTexSysIndex;
    int currentTexAnmIndex;
    int currentTexEffIndex;
    int currentTexEveIndex;
    int currentMonsterTexIndex;
    int currentIconIndex = 0;
    int[] currentMonsterIndexMap = Array.Empty<int>();
    bool cropImageOnLoad;
    string cardSearch = String.Empty;
    ImageMrgFile currentMrgFile = ImageMrgFile.Picture;


    ModdedStringName[] currentNameList = Array.Empty<ModdedStringName>();

    ModdedStringName[] iconName = new ModdedStringName[] {
        new ModdedStringName("Icon View", "Icon View"),
        new ModdedStringName("Icon Copy", "Icon Copy"),
        new ModdedStringName("Icon Delete", "Icon Delete")
    };

    byte[][] currentImageByteArray = Array.Empty<byte[]>();


    bool picMiniCropMode = false;
    SKPointI picMiniCropOrigin = new(0, 0);
    PaintCanvas? picMiniFullCanvas = null;

    static readonly HashSet<ImageMrgFile> AllImagesLoadedOnly = new() {
        ImageMrgFile.Model,
        ImageMrgFile.TexEtc,
        ImageMrgFile.TexSys,
        ImageMrgFile.TexAnm,
        ImageMrgFile.TexEff,
        ImageMrgFile.TexEve,
        ImageMrgFile.Monster,
    };

    public TextureEditorWindow()
    {
        EditorWindow.OnIsoLoaded += OnIsoLoaded;

    }

    ref int GetCurrentIndex()
    {
        if (AllImagesLoadedOnly.Contains(currentMrgFile) && !DataAccess.Instance.AllImagesLoaded)
        {
            currentMrgFile = ImageMrgFile.Picture;
        }
        switch (currentMrgFile)
        {
            case ImageMrgFile.Picture: return ref currentPictureIndex;
            case ImageMrgFile.PicPack: return ref currentPicPackIndex;
            case ImageMrgFile.PicMini: return ref currentPicMiniIndex;
            case ImageMrgFile.Model: return ref currentModelTexIndex;
            case ImageMrgFile.TexEtc: return ref currentTexEtcIndex;
            case ImageMrgFile.TexSys: return ref currentTexSysIndex;
            case ImageMrgFile.TexAnm: return ref currentTexAnmIndex;
            case ImageMrgFile.TexEff: return ref currentTexEffIndex;
            case ImageMrgFile.TexEve: return ref currentTexEveIndex;
            case ImageMrgFile.Monster: return ref currentMonsterTexIndex;
            case ImageMrgFile.Icon: return ref currentIconIndex;
            default: return ref currentPictureIndex;
        }
    }

    ModdedStringName[] GetNumberedNames(string prefix, int count)
    {
        ModdedStringName[] names = new ModdedStringName[count];
        for (int i = 0; i < count; i++)
        {
            names[i] = new ModdedStringName($"{prefix} {i}", $"{prefix} {i}");
        }
        return names;
    }

    void RefreshCurrentSource()
    {
        if (AllImagesLoadedOnly.Contains(currentMrgFile) && !DataAccess.Instance.AllImagesLoaded)
        {
            currentMrgFile = ImageMrgFile.Picture;
        }
        switch (currentMrgFile)
        {
            case ImageMrgFile.Picture:
                currentNameList = Card.cardNameList.Concat(Card.AltArtNames).ToArray();
                currentImageByteArray = GameImageManager.PictureBytes;
                break;
            case ImageMrgFile.PicMini:
                currentNameList = Card.cardNameList.Take(683).Concat(Card.AltArtNames.Take(16)).ToArray();
                currentImageByteArray = GameImageManager.PicMiniBytes;
                break;
            case ImageMrgFile.Model:
                currentNameList = GetNumberedNames("ModelTexture", 625);
                currentImageByteArray = GameImageManager.ModelTextureBytes;
                break;
            case ImageMrgFile.TexEtc:
                currentNameList = GetNumberedNames("TexEtc", DataAccess.TexEtcCount);
                currentImageByteArray = GameImageManager.TexEtcBytes;
                break;
            case ImageMrgFile.TexSys:
                currentNameList = GetNumberedNames("TexSys", DataAccess.TexSysCount);
                currentImageByteArray = GameImageManager.TexSysBytes;
                break;
            case ImageMrgFile.TexAnm:
                currentNameList = GetNumberedNames("TexAnm", DataAccess.TexAnmCount);
                currentImageByteArray = GameImageManager.TexAnmBytes;
                break;
            case ImageMrgFile.TexEff:
                currentNameList = GetNumberedNames("TexEff", DataAccess.TexEffCount);
                currentImageByteArray = GameImageManager.TexEffBytes;
                break;
            case ImageMrgFile.TexEve:
                currentNameList = GetNumberedNames("TexEve", DataAccess.TexEveCount);
                currentImageByteArray = GameImageManager.TexEveBytes;
                break;
            case ImageMrgFile.Monster:
                currentNameList = GetNumberedNames("Monster", DataAccess.MonsterModelCount);
                currentMonsterIndexMap = Enumerable.Range(0, DataAccess.MonsterModelCount)
                    .Where(i => !GameImageManager.MonsterModelExlusions.Contains(i))
                    .ToArray();
                currentNameList = currentMonsterIndexMap
                    .Select(i => new ModdedStringName($"Monster {i}", $"Monster {i}"))
                    .ToArray();
                currentImageByteArray = GameImageManager.MonsterModelBytes;
                break;
            case ImageMrgFile.Icon:
                currentNameList = iconName;
                break;
            default:
                currentNameList = Card.cardNameList;
                currentImageByteArray = GameImageManager.PictureBytes;
                break;
        }
    }


    public void Render()
    {
        if (!DataAccess.Instance.IsIsoLoaded) return;

        ref int currentIndex = ref GetCurrentIndex();
        if (paintCanvas == null)
        {
            paintCanvas = new PaintCanvas(256, 256, SKColors.White);
        }

        if (!DataAccess.Instance.AllImagesLoaded && AllImagesLoadedOnly.Contains(currentMrgFile))
        {
            currentMrgFile = ImageMrgFile.Picture;
            currentIndex = ref GetCurrentIndex();
            RefreshCurrentSource();
        }

        bool isPicMini = currentMrgFile == ImageMrgFile.PicMini;

        paintCanvas.ForceRefresh();
        paintCanvas.Update();


        ImGui.BeginGroup();

        ImGui.Text("Tool");
        if (ImGui.RadioButton("Pencil", selectedTool == PaintCanvas.Tool.Pencil))
            selectedTool = PaintCanvas.Tool.Pencil;
        if (ImGui.RadioButton("Eraser", selectedTool == PaintCanvas.Tool.Eraser))
            selectedTool = PaintCanvas.Tool.Eraser;
        if (ImGui.RadioButton("Fill", selectedTool == PaintCanvas.Tool.Fill))
            selectedTool = PaintCanvas.Tool.Fill;
        if (ImGui.RadioButton("Eyedrop", selectedTool == PaintCanvas.Tool.Eyedropper))
            selectedTool = PaintCanvas.Tool.Eyedropper;

        ImGui.Spacing();
        ImGui.Text("Brush Size");
        ImGui.SetNextItemWidth(150 * EditorWindow.AspectRatio.X);
        ImGui.SliderFloat("##bs", ref brushSize, 1f, 64f);

        ImGui.Text("Eraser Opacity");
        ImGui.SetNextItemWidth(150 * EditorWindow.AspectRatio.X);
        ImGui.SliderFloat("##op", ref opacity, 0f, 1f);

        ImGui.Spacing();
        ImGui.Text("Color");
        if (ImGui.ColorEdit4("##col", ref brushColor,
                ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.AlphaBar))
        {
            if (paintCanvas != null && currentPalleteIndex >= 0 && currentPalleteIndex < palette.Count)
            {
                SKColor oldColour = palette[currentPalleteIndex];
                SKColor newColour = new SKColor(
                    (byte)(brushColor.X * 255),
                    (byte)(brushColor.Y * 255),
                    (byte)(brushColor.Z * 255),
                    (byte)(brushColor.W * 255));

                GameImageManager.CurrentTexture.Palette[currentPalleteIndex] =
                    (uint)((newColour.Alpha << 24) | (newColour.Red << 16) | (newColour.Green << 8) | newColour.Blue);

                ImageCreator.RemapColourInBitmap(paintCanvas.Bitmap, oldColour, newColour);
                palette = ImageCreator.ExtractPalette();
                paintCanvas.ForceRefresh();
            }
        }

        ImGui.Spacing();
        if (ImGui.Button("Undo") && paintCanvas.CanUndo) paintCanvas.Undo();
        ImGui.SameLine();
        if (ImGui.Button("Redo") && paintCanvas.CanRedo) paintCanvas.Redo();

        ImGui.Spacing();
        ImGui.Spacing();

        if (ImGui.Button("Load All Textures"))
        {
            currentMrgFile = ImageMrgFile.Picture;
            RefreshCurrentSource();
            GetCurrentIndex();
            ImageCreator.CreateImageFromBytes(currentImageByteArray[currentIndex], currentMrgFile, "", false);
            paintCanvas.LoadBitmap(GameImageManager.CurrentTexture.Bitmap);
            paintCanvas.ClearStack();
            palette = ImageCreator.ExtractPalette();
            EditorWindow._modalPopup.Show($"Loading images in background make take a few seconds",
                "Image Loader", null, ImGuiModalPopup.ShowType.OneButton);
            Task.Run(async () =>
            {
                await DataAccess.Instance.LoadAllImages();
                EditorWindow._modalPopup.Show($"Images Loaded",
                    "Loader", null, ImGuiModalPopup.ShowType.OneButton);

            });

        }


        if (ImGui.Button("Save Pic Buffer"))
        {
            if (paintCanvas == null || picMiniCropMode && picMiniFullCanvas == null)
                return;

            if (currentMrgFile == ImageMrgFile.Icon)
            {
                GameImageManager.IconImageBytes[currentIndex] = ImageSaver.SaveImageToBytes(GameImageManager.IconImageBytes[currentIconIndex], currentMrgFile, paintCanvas.Bitmap);
            }
            else
            {
                currentImageByteArray[currentIndex] = ImageSaver.SavePaletteToBytes(currentImageByteArray[currentIndex]);
                currentImageByteArray[currentIndex] = ImageSaver.SaveImageToBytes(currentImageByteArray[currentIndex], currentMrgFile, paintCanvas.Bitmap);
            }
        }

        if (ImGui.RadioButton("Crop Loaded Image", cropImageOnLoad))
        {
            cropImageOnLoad = !cropImageOnLoad;
        }


        if (ImGui.Button("Load"))
        {

            paintCanvas?.loadImage(currentImageByteArray, currentIndex, cropImageOnLoad);
            palette = ImageCreator.ExtractPalette();
        }


        if (ImGui.Button("Export"))
        {
            paintCanvas?.SaveAs();
        }
        if (ImGui.Button("Export All loaded images"))
        {
            _ = paintCanvas?.SaveAll();
        }


        ImGui.EndGroup();
        ImGui.SameLine();

        Vector2 setSize = ImGui.GetContentRegionAvail();
        ImGui.BeginChild("pallete", new Vector2(setSize.X / 2f, setSize.Y));

        float regionWidth = ImGui.GetContentRegionAvail().X;
        int columns = 16;
        float padding = 3f;
        float swatchSize = (regionWidth - padding * (columns - 1)) / columns;

        ImGui.Text("Palette");
        ImGui.Spacing();

        var drawList = ImGui.GetWindowDrawList();
        Vector2 cursor = ImGui.GetCursorScreenPos();
        float startX = cursor.X;

        for (int i = 0; i < palette.Count; i++)
        {
            SKColor c = palette[i];
            uint col = ImGui.ColorConvertFloat4ToU32(
                new Vector4(c.Red / 255f, c.Green / 255f, c.Blue / 255f, c.Alpha / 255f));

            Vector2 swatchMin = new Vector2(
                startX + (i % columns) * (swatchSize + padding),
                cursor.Y + (i / columns) * (swatchSize + padding));
            Vector2 swatchMax = swatchMin + new Vector2(swatchSize, swatchSize);

            SKColor currentBrushColour = new SKColor(
                (byte)(brushColor.X * 255),
                (byte)(brushColor.Y * 255),
                (byte)(brushColor.Z * 255),
                (byte)(brushColor.W * 255));

            if (c.Alpha < 255)
            {
                drawList.AddRectFilled(swatchMin, swatchMax, 0xFFAAAAAA);
                drawList.AddRectFilled(swatchMin,
                    swatchMin + new Vector2(swatchSize / 2, swatchSize / 2), 0xFF555555);
                drawList.AddRectFilled(
                    swatchMin + new Vector2(swatchSize / 2, swatchSize / 2), swatchMax, 0xFF555555);
            }

            drawList.AddRectFilled(swatchMin, swatchMax, col);

            if (c == currentBrushColour)
            {
                drawList.AddRect(swatchMin - new Vector2(1, 1), swatchMax + new Vector2(1, 1),
                    0xFFFFFFFF, 0, ImDrawFlags.None, 2f);
                drawList.AddRect(swatchMin - new Vector2(2, 2), swatchMax + new Vector2(2, 2),
                    0xFF000000, 0, ImDrawFlags.None, 1f);
            }

            ImGui.SetCursorScreenPos(swatchMin);
            ImGui.InvisibleButton($"##swatch_{i}", new Vector2(swatchSize, swatchSize));

            if (ImGui.IsItemClicked())
            {
                brushColor = new Vector4(c.Red / 255f, c.Green / 255f, c.Blue / 255f, c.Alpha / 255f);
                currentPalleteIndex = i;
            }

            if (ImGui.IsItemHovered())
            {
                drawList.AddRect(swatchMin - new Vector2(1, 1), swatchMax + new Vector2(1, 1),
                    0xFFFFFFFF, 0, ImDrawFlags.None, 1.5f);
                ImGui.SetTooltip($"#{c.Red:X2}{c.Green:X2}{c.Blue:X2}{c.Alpha:X2}");
            }
        }

        int rows = (int)Math.Ceiling(palette.Count / (float)columns);
        ImGui.SetCursorScreenPos(new Vector2(startX, cursor.Y + rows * (swatchSize + padding) + 4f));

        ImGui.EndChild();
        ImGui.SameLine();

        ImGui.BeginChild("Canvas", ImGui.GetContentRegionAvail());

        if (isPicMini)
        {
            if (ImGui.RadioButton("Edit PicMini", !picMiniCropMode))
            {
                picMiniCropMode = false;
            }

            ImGui.SameLine();
            if (ImGui.RadioButton("Crop from Picture", picMiniCropMode))
            {
                picMiniCropMode = true;
                int pictureIndex = currentIndex;
                if (pictureIndex > 682)
                {
                    pictureIndex += 171;
                }

                LoadPicMiniFullSizeBackground(pictureIndex, currentIndex);
            }
        }
        ImGui.SetNextItemWidth(-1);
        if (ImGui.BeginCombo("##MrgFile", currentMrgFile.ToString()))
        {
            foreach (ImageMrgFile val in Enum.GetValues<ImageMrgFile>())
            {
                if (val == ImageMrgFile.PicPack)
                    continue;

                if (!DataAccess.Instance.AllImagesLoaded && AllImagesLoadedOnly.Contains(val))
                    continue;

                bool selected = val == currentMrgFile;
                if (ImGui.Selectable(val.ToString(), selected))
                {
                    currentPalleteIndex = -1;
                    currentMrgFile = val;
                    picMiniCropMode = false;
                    cardSearch = "";
                    RefreshCurrentSource();
                    currentIndex = ref GetCurrentIndex();

                    if (val == ImageMrgFile.Icon)
                    {
                        GameImageManager.CurrentTexture.Bitmap = PS2Icon.ExtractIconTexture(
                            GameImageManager.IconImageBytes[PS2Icon.CurrentTextureIndex]);
                        PS2Icon.BuildPaletteFromBitmap(GameImageManager.CurrentTexture.Bitmap);
                    }
                    else
                    {
                        ImageCreator.CreateImageFromBytes(currentImageByteArray[currentIndex], currentMrgFile, "", false);
                    }

                    paintCanvas.LoadBitmap(GameImageManager.CurrentTexture.Bitmap);
                    paintCanvas.ClearStack();
                    palette = ImageCreator.ExtractPalette();
                }
            }
            ImGui.EndCombo();
        }

        ImGui.SetNextItemWidth(-1);
        string comboLabel = currentMrgFile == ImageMrgFile.Monster
            ? currentNameList[Array.IndexOf(currentMonsterIndexMap, currentIndex)].Current
            : currentNameList[currentIndex].Current;
        if (ImGui.BeginCombo("##CardId", comboLabel))
        {
            ImGui.SetNextItemWidth(-1);
            if (ImGui.IsWindowAppearing()) ImGui.SetKeyboardFocusHere();

            if (ImGui.InputText("##CardSearch", ref cardSearch, 128))
            {
            }
            ImGui.Separator();

            (string Current, int OriginalIndex)[] filteredList;

            if (currentMrgFile == ImageMrgFile.Icon)
            {
                filteredList = currentNameList
                    .Select((name, i) => (name.Current, OriginalIndex: i))
                    .ToArray();
            }
            else if (currentMrgFile == ImageMrgFile.Monster)
            {
                filteredList = currentNameList
                    .Select((name, i) => (name.Current, OriginalIndex: currentMonsterIndexMap[i]))
                    .Where(x => x.Current.Contains(cardSearch, StringComparison.OrdinalIgnoreCase))
                    .ToArray();
            }
            else
            {
                filteredList = currentNameList
                    .Select((name, i) => (name.Current, OriginalIndex: i))
                    .Where(x => x.Current.Contains(cardSearch, StringComparison.OrdinalIgnoreCase))
                    .ToArray();
            }


            foreach (var (name, originalIndex) in filteredList)
            {
                bool selected = originalIndex == currentIndex;
                if (ImGui.Selectable(name, selected))
                {
                    currentPalleteIndex = -1;
                    cardSearch = "";
                    currentIndex = originalIndex;
                    picMiniCropMode = false;

                    if (currentMrgFile == ImageMrgFile.Icon)
                    {
                        PS2Icon.CurrentTextureIndex = originalIndex;
                        GameImageManager.CurrentTexture.Bitmap = PS2Icon.ExtractIconTexture(
                            GameImageManager.IconImageBytes[PS2Icon.CurrentTextureIndex]);
                        PS2Icon.BuildPaletteFromBitmap(GameImageManager.CurrentTexture.Bitmap);
                    }
                    else
                    {
                        ImageCreator.CreateImageFromBytes(currentImageByteArray[currentIndex], currentMrgFile, "", false);
                    }

                    paintCanvas.LoadBitmap(GameImageManager.CurrentTexture.Bitmap);
                    paintCanvas.ClearStack();
                    palette = ImageCreator.ExtractPalette();
                }
            }
            ImGui.EndCombo();
        }


        Vector2 canvasPos = ImGui.GetCursorScreenPos();
        Vector2 availableSize = ImGui.GetContentRegionAvail();

        if (isPicMini && picMiniCropMode && picMiniFullCanvas != null)
        {
            RenderPicMiniCropSelector(canvasPos, availableSize);
        }
        else
        {
            float aspect = (float)paintCanvas.Width / paintCanvas.Height;
            Vector2 displaySize = availableSize.X / aspect <= availableSize.Y
                ? new Vector2(availableSize.X, availableSize.X / aspect)
                : new Vector2(availableSize.Y * aspect, availableSize.Y);

            ImGui.Image(paintCanvas.GetTexturePtr(), displaySize);
            HandleCanvasInput(canvasPos, displaySize, palette);
        }

        ImGui.EndChild();

        if (!ImGui.IsAnyItemActive())
        {
            bool ctrl = ImGui.GetIO().KeyCtrl;
            if (ctrl && ImGui.IsKeyPressed(ImGuiKey.Z)) paintCanvas.Undo();
            if (ctrl && ImGui.IsKeyPressed(ImGuiKey.X)) paintCanvas.Redo();
            if (ImGui.IsKeyPressed(ImGuiKey.Q)) selectedTool = PaintCanvas.Tool.Pencil;
            if (ImGui.IsKeyPressed(ImGuiKey.W)) selectedTool = PaintCanvas.Tool.Eraser;
            if (ImGui.IsKeyPressed(ImGuiKey.E)) selectedTool = PaintCanvas.Tool.Fill;
            if (ImGui.IsKeyPressed(ImGuiKey.R)) selectedTool = PaintCanvas.Tool.Eyedropper;
        }
    }


    void HandleCanvasInput(Vector2 canvasPos, Vector2 displaySize, List<SKColor> palette)
    {
        bool isHovered = ImGui.IsItemHovered();
        bool mouseDown = ImGui.IsMouseDown(ImGuiMouseButton.Left);
        bool mouseClicked = ImGui.IsMouseClicked(ImGuiMouseButton.Left);

        if (isHovered)
        {
            Vector2 mousePos = ImGui.GetMousePos();
            float u = (mousePos.X - canvasPos.X) / displaySize.X;
            float v = (mousePos.Y - canvasPos.Y) / displaySize.Y;
            float bx = u * paintCanvas.Width;
            float by = v * paintCanvas.Height;
            int ibx = (int)Math.Clamp(bx, 0, paintCanvas.Width - 1);
            int iby = (int)Math.Clamp(by, 0, paintCanvas.Height - 1);

            paintCanvas.ActiveTool = selectedTool;
            paintCanvas.BrushSize = brushSize;
            paintCanvas.Opacity = opacity;
            paintCanvas.ActiveColor = new SKColor(
                (byte)(brushColor.X * 255),
                (byte)(brushColor.Y * 255),
                (byte)(brushColor.Z * 255),
                (byte)(brushColor.W * 255));

            if (mouseDown)
            {
                if (mouseClicked)
                {
                    if (selectedTool == PaintCanvas.Tool.Fill)
                    {
                        paintCanvas.FloodFill(ibx, iby);
                        wasPainting = true;
                    }
                    else if (selectedTool == PaintCanvas.Tool.Eyedropper)
                    {
                        SKColor picked = paintCanvas.Bitmap.GetPixel(ibx, iby);
                        int foundIndex = palette.FindIndex(c =>
                            c.Red == picked.Red && c.Green == picked.Green && c.Blue == picked.Blue);

                        if (foundIndex >= 0)
                        {
                            currentPalleteIndex = foundIndex;
                            SKColor paletteColour = palette[foundIndex];
                            brushColor = new Vector4(
                                paletteColour.Red / 255f,
                                paletteColour.Green / 255f,
                                paletteColour.Blue / 255f,
                                paletteColour.Alpha / 255f);
                        }
                        selectedTool = PaintCanvas.Tool.Pencil;
                    }
                    else
                    {
                        paintCanvas.BeginStroke();
                        paintCanvas.PaintAt(bx, by);
                        wasPainting = true;
                    }
                }
                else if (wasPainting &&
                         (selectedTool == PaintCanvas.Tool.Pencil ||
                          selectedTool == PaintCanvas.Tool.Eraser))
                {
                    paintCanvas.PaintAt(bx, by);
                }
            }
            else if (wasPainting)
            {
                paintCanvas.EndStroke();
                wasPainting = false;
            }

            ImGui.SetMouseCursor(ImGuiMouseCursor.None);
            var dl = ImGui.GetWindowDrawList();

            if (selectedTool == PaintCanvas.Tool.Eyedropper)
            {
                float pixelW = displaySize.X / paintCanvas.Width;
                float pixelH = displaySize.Y / paintCanvas.Height;
                Vector2 snappedScreen = new Vector2(
                    canvasPos.X + ibx * pixelW,
                    canvasPos.Y + iby * pixelH);

                dl.AddRect(snappedScreen, snappedScreen + new Vector2(pixelW, pixelH),
                    0xFFFFFFFF, 0, ImDrawFlags.None, 1.5f);
                dl.AddRect(snappedScreen - new Vector2(1, 1),
                    snappedScreen + new Vector2(pixelW + 1, pixelH + 1),
                    0xFF000000, 0, ImDrawFlags.None, 0.5f);

                float cx = snappedScreen.X + pixelW / 2f;
                float cy = snappedScreen.Y + pixelH / 2f;
                dl.AddLine(new Vector2(cx - pixelW * 2, cy), new Vector2(cx + pixelW * 2, cy), 0xFF000000, 1f);
                dl.AddLine(new Vector2(cx, cy - pixelH * 2), new Vector2(cx, cy + pixelH * 2), 0xFF000000, 1f);
                dl.AddLine(new Vector2(cx - pixelW * 2, cy), new Vector2(cx + pixelW * 2, cy), 0xFFFFFFFF, 0.5f);
                dl.AddLine(new Vector2(cx, cy - pixelH * 2), new Vector2(cx, cy + pixelH * 2), 0xFFFFFFFF, 0.5f);

                SKColor hovered = paintCanvas.Bitmap.GetPixel(ibx, iby);
                ImGui.BeginTooltip();
                ImGui.ColorButton("##preview", new Vector4(
                        hovered.Red / 255f, hovered.Green / 255f,
                        hovered.Blue / 255f, hovered.Alpha / 255f),
                    ImGuiColorEditFlags.NoTooltip, new Vector2(pixelW * 4f, pixelW * 4f));
                ImGui.SameLine();
                ImGui.Text($"#{hovered.Red:X2}{hovered.Green:X2}{hovered.Blue:X2}{hovered.Alpha:X2}");
                ImGui.EndTooltip();
            }
            else
            {
                float radius = brushSize * (displaySize.X / paintCanvas.Width) / 2f;
                dl.AddCircle(mousePos, radius,
                    ImGui.GetColorU32(new Vector4(1, 1, 1, 0.8f)), 32, 1.5f);
                dl.AddCircle(mousePos, radius + 1f,
                    ImGui.GetColorU32(new Vector4(0, 0, 0, 0.5f)), 32, 0.5f);
            }
        }
        else if (wasPainting)
        {
            paintCanvas.EndStroke();
            wasPainting = false;
        }
    }


    void RenderPicMiniCropSelector(Vector2 canvasPos, Vector2 availableSize)
    {
        var dl = ImGui.GetWindowDrawList();
        int fullW = picMiniFullCanvas!.Width;
        int fullH = picMiniFullCanvas!.Height;

        float aspect = (float)fullW / fullH;
        Vector2 displaySize = availableSize.X / aspect <= availableSize.Y
            ? new Vector2(availableSize.X, availableSize.X / aspect)
            : new Vector2(availableSize.Y * aspect, availableSize.Y);

        ImGui.Image(picMiniFullCanvas.GetTexturePtr(), displaySize);

        float scaleX = displaySize.X / fullW;
        float scaleY = displaySize.Y / fullH;

        if (ImGui.IsItemHovered() && ImGui.IsMouseDown(ImGuiMouseButton.Left))
        {
            Vector2 mouse = ImGui.GetMousePos();
            int imgX = (int)((mouse.X - canvasPos.X) / scaleX);
            int imgY = (int)((mouse.Y - canvasPos.Y) / scaleY);

            int newX = Math.Clamp(imgX - 20, 0, fullW - 40);
            int newY = Math.Clamp(imgY - 16, 0, fullH - 32);

            if (newX != picMiniCropOrigin.X || newY != picMiniCropOrigin.Y)
            {
                picMiniCropOrigin.X = newX;
                picMiniCropOrigin.Y = newY;

                using var cropped = new SKBitmap(40, 32, SKColorType.Bgra8888, SKAlphaType.Unpremul);
                using var cropCanvas = new SKCanvas(cropped);
                var srcRect = new SKRectI(picMiniCropOrigin.X, picMiniCropOrigin.Y,
                    picMiniCropOrigin.X + 40, picMiniCropOrigin.Y + 32);
                cropCanvas.DrawBitmap(picMiniFullCanvas.Bitmap, srcRect, new SKRect(0, 0, 40, 32));


                cropCanvas.Flush();
                paintCanvas.LoadBitmap(cropped);
                ImageCreator.BuildPaletteFromBitmap(picMiniFullCanvas.Bitmap, currentImageByteArray[GetCurrentIndex()]);
                palette = ImageCreator.ExtractPalette();

            }
        }


        Vector2 cropMin = canvasPos + new Vector2(picMiniCropOrigin.X * scaleX, picMiniCropOrigin.Y * scaleY);
        Vector2 cropMax = cropMin + new Vector2(40 * scaleX, 32 * scaleY);
        dl.AddRectFilled(cropMin, cropMax, 0x33FFFFFF);
        dl.AddRect(cropMin, cropMax, 0xFFFFFFFF, 0, ImDrawFlags.None, 2f);
        dl.AddRect(cropMin - new Vector2(1, 1), cropMax + new Vector2(1, 1), 0xFF000000, 0, ImDrawFlags.None, 1f);
        ImGui.SetTooltip($"Crop origin: ({picMiniCropOrigin.X}, {picMiniCropOrigin.Y})");

    }

    void LoadPicMiniFullSizeBackground(int pictureIndex, int byteArrayIndex)
    {
        var pictureBytes = GameImageManager.PictureBytes[pictureIndex];
        ImageCreator.CreateImageFromBytes(pictureBytes, ImageMrgFile.Picture, "", false);

        picMiniFullCanvas?.Free();
        picMiniFullCanvas = new PaintCanvas(GameImageManager.CurrentTexture.Bitmap);


        ImageCreator.CreateImageFromBytes(currentImageByteArray[byteArrayIndex], currentMrgFile, "", false);
        picMiniCropOrigin.X = Math.Clamp(picMiniCropOrigin.X, 0, picMiniFullCanvas.Width - 40);
        picMiniCropOrigin.Y = Math.Clamp(picMiniCropOrigin.Y, 0, picMiniFullCanvas.Height - 32);
    }

    void OnIsoLoaded()
    {
        RefreshCurrentSource();
        ref int index = ref GetCurrentIndex();

        if (currentMrgFile == ImageMrgFile.Icon)
        {
            GameImageManager.CurrentTexture.Bitmap = PS2Icon.ExtractIconTexture(
                GameImageManager.IconImageBytes[PS2Icon.CurrentTextureIndex]);
            PS2Icon.BuildPaletteFromBitmap(GameImageManager.CurrentTexture.Bitmap);
        }
        else
        {
            ImageCreator.CreateImageFromBytes(currentImageByteArray[index], currentMrgFile, "", false);
        }

        paintCanvas = new PaintCanvas(GameImageManager.CurrentTexture.Bitmap);
        palette = ImageCreator.ExtractPalette();
    }

    public void Free()
    {
    }
}