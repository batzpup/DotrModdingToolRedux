using System.Drawing;
using System.Numerics;
using ImGuiNET;
namespace DotrModdingTool2IMGUI;

public class DestinyDrawEditorWindow : IImGuiWindow
{
    ImGuiTableFlags flags = ImGuiTableFlags.Resizable | ImGuiTableFlags.Reorderable | ImGuiTableFlags.Hideable |
                            ImGuiTableFlags.Sortable |
                            ImGuiTableFlags.SortMulti | ImGuiTableFlags.BordersV | ImGuiTableFlags.RowBg |
                            ImGuiTableFlags.Borders |
                            ImGuiTableFlags.ScrollX |
                            ImGuiTableFlags.ScrollY |
                            ImGuiTableFlags.BordersInnerH;

    public class PoolWeights
    {
        public int slot1;
        public int slot2;
    }

    //TODO update to have its own or a general dropdown background or something
    Vector4 tableBgColour = UserSettings.FusionTableBgColour;
    Vector4 searchColour = UserSettings.FusionDropdownColour;

    string filter1Text = "";
    string filter2Text = "";
    string filter3Text = "";

    bool lowerFocusInput = true;
    bool higherFocusInput = true;
    bool resultFocusInput = true;

    public int slot1Bonus;
    public int slot2Bonus;

    public static Dictionary<CardKind.CardKindEnum, PoolWeights> DestinyDrawModifiers = new() {
        { CardKind.CardKindEnum.Dragon, new() { slot1 = 0, slot2 = 0 } },
        { CardKind.CardKindEnum.Spellcaster, new() { slot1 = 2, slot2 = 1 } },
        { CardKind.CardKindEnum.Zombie, new() { slot1 = 12, slot2 = 6 } },
        { CardKind.CardKindEnum.Warrior, new() { slot1 = 2, slot2 = 1 } },
        { CardKind.CardKindEnum.BeastWarrior, new() { slot1 = 5, slot2 = 3 } },
        { CardKind.CardKindEnum.Beast, new() { slot1 = 8, slot2 = 4 } },
        { CardKind.CardKindEnum.WingedBeast, new() { slot1 = 6, slot2 = 3 } },
        { CardKind.CardKindEnum.Fiend, new() { slot1 = 0, slot2 = 0 } },
        { CardKind.CardKindEnum.Fairy, new() { slot1 = 12, slot2 = 6 } },
        { CardKind.CardKindEnum.Insect, new() { slot1 = 10, slot2 = 5 } },
        { CardKind.CardKindEnum.Dinosaur, new() { slot1 = 14, slot2 = 7 } },
        { CardKind.CardKindEnum.Reptile, new() { slot1 = 15, slot2 = 8 } },
        { CardKind.CardKindEnum.Fish, new() { slot1 = 8, slot2 = 4 } },
        { CardKind.CardKindEnum.SeaSerpent, new() { slot1 = 14, slot2 = 7 } },
        { CardKind.CardKindEnum.Machine, new() { slot1 = 3, slot2 = 1 } },
        { CardKind.CardKindEnum.Thunder, new() { slot1 = 10, slot2 = 5 } },
        { CardKind.CardKindEnum.Aqua, new() { slot1 = 2, slot2 = 1 } },
        { CardKind.CardKindEnum.Pyro, new() { slot1 = 13, slot2 = 6 } },
        { CardKind.CardKindEnum.Rock, new() { slot1 = 10, slot2 = 5 } },
        { CardKind.CardKindEnum.Plant, new() { slot1 = 10, slot2 = 5 } },
        { CardKind.CardKindEnum.Immortal, new() { slot1 = -5, slot2 = -2 } },
    };

    public void Render()
    {
        if (!DataAccess.Instance.IsIsoLoaded)
        {
            ImGui.Text("Please load ISO file");
            ImGui.PopFont();
            return;
        }
        ImGui.PushFont(FontManager.GetFont(FontManager.FontFamily.NotoSansJP, 30));
        ImGui.BeginChild("leftSide", new Vector2(ImGui.GetContentRegionAvail().X / 10 * 6, ImGui.GetContentRegionAvail().Y));
        ImGui.Indent(ImGui.GetContentRegionAvail().X / 128);
        ImGui.Dummy(new Vector2(0, ImageHelper.DefaultImageSize.Y * 0.15f));

        ImGui.PushFont(FontManager.GetBestFitFont("Destiny draw table"));
        ImGui.SetCursorPosX(ImGui.GetContentRegionAvail().X / 2 - ImGui.CalcTextSize("Destiny draw table").X / 2f);
        ImGui.Text("Destiny draw table");
        ImGui.PopFont();


        if (ImGui.BeginTable("DestinyTable", 4, flags))
        {
            ImGui.TableSetupColumn(String.Empty, ImGuiTableColumnFlags.WidthFixed, ImGui.CalcTextSize("Slot 1    ").X);
            ImGui.TableSetupColumn("Slot 1", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("Slot 2", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("Slot 3", ImGuiTableColumnFlags.WidthStretch);

            ImGui.TableHeadersRow();

            for (var i = 0; i < DestinyDrawData.DestinyCardPools.Length; i++)
            {
                var pool = DestinyDrawData.DestinyCardPools[i];
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text($"Pool {i + 1}");
                ImGui.TableSetColumnIndex(1);


                ImGui.PushItemWidth(-1);
                if (ImGui.BeginCombo($"##slot1_{i}", Card.cardNameList[pool[0]].Current))
                {
                    if (lowerFocusInput)
                    {
                        ImGui.SetKeyboardFocusHere();
                        lowerFocusInput = false;
                    }
                    ImGui.PushStyleColor(ImGuiCol.FrameBg, searchColour);
                    ImGui.InputText($"##searchInputLower_{i}", ref filter1Text, 64);
                    ImGui.PopStyleColor();

                    List<ModdedStringName> filteredList = Card.cardNameList
                        .Where(cardName => cardName.Current.Contains(filter1Text, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    bool anyVisible = false;
                    foreach (var cardName in filteredList)
                    {
                        int index = Array.IndexOf(Card.cardNameList, cardName);
                        bool isSelected = pool[0] == index;
                        if (ImGui.Selectable(cardName.Current, isSelected))
                        {
                            pool[0] = index;
                            filter1Text = "";
                        }
                        if (ImGui.IsItemVisible())
                        {
                            anyVisible = true;
                        }
                        if (ImGui.IsItemHovered())
                        {
                            if (UserSettings.ToggleImageTooltips)
                            {
                                ImGui.BeginTooltip();
                                ImGui.Text("Card Preview");
                                ImGui.Image(GlobalImages.Instance.Cards[cardName.Default], new Vector2(128, 128));
                                ImGui.EndTooltip();
                            }
                        }
                    }
                    if (!anyVisible)
                    {
                        filter1Text = "";
                        lowerFocusInput = true;
                    }
                    ImGui.EndCombo();
                }
                if (ImGui.IsItemHovered())
                {
                    GlobalImgui.RenderTooltipCardImage(Card.cardNameList[pool[0]].Default);
                }
                ImGui.Image(GlobalImages.Instance.Cards[Card.cardNameList[pool[0]].Default], ImageHelper.DefaultImageSize);
                ImGui.TableSetColumnIndex(2);


                ImGui.PushItemWidth(-1);
                if (ImGui.BeginCombo($"##slot2_{i}", Card.cardNameList[pool[1]].Current))
                {
                    if (lowerFocusInput)
                    {
                        ImGui.SetKeyboardFocusHere();
                        lowerFocusInput = false;
                    }
                    ImGui.PushStyleColor(ImGuiCol.FrameBg, searchColour);
                    ImGui.InputText($"##searchInputLower_{i}", ref filter2Text, 64);
                    ImGui.PopStyleColor();

                    List<ModdedStringName> filteredList = Card.cardNameList
                        .Where(cardName => cardName.Current.Contains(filter2Text, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    bool anyVisible = false;
                    foreach (var cardName in filteredList)
                    {
                        int index = Array.IndexOf(Card.cardNameList, cardName);
                        bool isSelected = pool[1] == index;
                        if (ImGui.Selectable(cardName.Current, isSelected))
                        {
                            pool[1] = index;
                            filter2Text = "";
                        }
                        if (ImGui.IsItemVisible())
                        {
                            anyVisible = true;
                        }
                        if (ImGui.IsItemHovered())
                        {
                            if (UserSettings.ToggleImageTooltips)
                            {
                                ImGui.BeginTooltip();
                                ImGui.Text("Card Preview");
                                ImGui.Image(GlobalImages.Instance.Cards[cardName.Default], new Vector2(128, 128));
                                ImGui.EndTooltip();
                            }
                        }
                    }
                    if (!anyVisible)
                    {
                        filter2Text = "";
                        lowerFocusInput = true;
                    }
                    ImGui.EndCombo();

                }
                if (ImGui.IsItemHovered())
                {
                    GlobalImgui.RenderTooltipCardImage(Card.cardNameList[pool[1]].Default);
                }
                ImGui.Image(GlobalImages.Instance.Cards[Card.cardNameList[pool[1]].Default], ImageHelper.DefaultImageSize);
                ImGui.TableSetColumnIndex(3);


                ImGui.PushItemWidth(-1);
                if (ImGui.BeginCombo($"##slot3_{i}", Card.cardNameList[pool[2]].Current))
                {
                    if (lowerFocusInput)
                    {
                        ImGui.SetKeyboardFocusHere();
                        lowerFocusInput = false;
                    }
                    ImGui.PushStyleColor(ImGuiCol.FrameBg, searchColour);
                    ImGui.InputText($"##searchInputLower_{i}", ref filter3Text, 64);
                    ImGui.PopStyleColor();

                    List<ModdedStringName> filteredList = Card.cardNameList
                        .Where(cardName => cardName.Current.Contains(filter3Text, StringComparison.OrdinalIgnoreCase))
                        .ToList();
                    bool anyVisible = false;
                    foreach (var cardName in filteredList)
                    {
                        int index = Array.IndexOf(Card.cardNameList, cardName);
                        bool isSelected = pool[2] == index;
                        if (ImGui.Selectable(cardName.Current, isSelected))
                        {
                            pool[2] = index;
                            filter3Text = "";
                        }
                        if (ImGui.IsItemVisible())
                        {
                            anyVisible = true;
                        }
                        if (ImGui.IsItemHovered())
                        {
                            if (UserSettings.ToggleImageTooltips)
                            {
                                ImGui.BeginTooltip();
                                ImGui.Text("Card Preview");
                                ImGui.Image(GlobalImages.Instance.Cards[cardName.Default], new Vector2(128, 128));
                                ImGui.EndTooltip();
                            }
                        }
                    }
                    if (!anyVisible)
                    {
                        filter3Text = "";
                        lowerFocusInput = true;
                    }
                    ImGui.EndCombo();
                }
                if (ImGui.IsItemHovered())
                {
                    GlobalImgui.RenderTooltipCardImage(Card.cardNameList[pool[2]].Default);
                }
                ImGui.Image(GlobalImages.Instance.Cards[Card.cardNameList[pool[2]].Default], ImageHelper.DefaultImageSize);
            }

            ImGui.EndTable();
        }

        ImGui.Unindent(ImGui.GetContentRegionAvail().X / 128);
        ImGui.EndChild();
        ImGui.SameLine();
        ImGui.BeginChild("rightSide", ImGui.GetContentRegionAvail());

        ImGui.Dummy(new Vector2(0, ImageHelper.DefaultImageSize.Y * 0.15f));
        ImGui.TextColored(new GuiColour(Color.Red).value, "Saving not implemented on modifiers yet ");
        ImGui.SetCursorPosX(ImGui.GetContentRegionAvail().X / 2f - ImGui.CalcTextSize("Leader kind modifiers").X / 2f);
        ImGui.Text("Leader kind modifiers");
        float rowHeight = ImGui.GetTextLineHeightWithSpacing() + ImGui.GetStyle().CellPadding.Y;
        float tableHeight = rowHeight * 24.2f + ImGui.GetStyle().CellPadding.Y * 2;
        if (ImGui.BeginTable("KindModifiers", 3, flags, new Vector2(0, tableHeight)))
        {
            ImGui.TableSetupColumn("Kind", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("slot 1 bonus", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableSetupColumn("slot 2 bonus", ImGuiTableColumnFlags.WidthStretch);
            ImGui.TableHeadersRow();
            for (int i = 0; i <= 20; i++)
            {
                CardKind.CardKindEnum type = (CardKind.CardKindEnum)i;
                ImGui.TableNextRow();
                ImGui.TableSetColumnIndex(0);
                ImGui.Text($"{type}");

                ImGui.TableSetColumnIndex(1);
                ImGui.PushItemWidth(-1);
                if (ImGui.InputInt($"##card1{i}", ref DestinyDrawModifiers[type].slot1, 0))
                {
                    DestinyDrawModifiers[type].slot1 = Math.Clamp(DestinyDrawModifiers[type].slot1, -100, 100);
                }

                ImGui.TableSetColumnIndex(2);
                ImGui.PushItemWidth(-1);
                if (ImGui.InputInt($"##card2{i}", ref DestinyDrawModifiers[type].slot2, 0))
                {
                    DestinyDrawModifiers[type].slot2 = Math.Clamp(DestinyDrawModifiers[type].slot2, -100, 100);
                }

            }
            ImGui.EndTable();

        }
        ImGui.Text("Genral bonuses");

        ImGui.Text("Slot 1");
        ImGui.SameLine();
        ImGui.Text("Slot 2");
        ImGui.PushItemWidth(ImGui.CalcTextSize("10000").X);
        if (ImGui.InputInt("##Slot1Bonus", ref slot1Bonus, 0))
        {
            slot1Bonus = Math.Clamp(slot1Bonus, 0, 100);

        }


        ImGui.PushItemWidth(ImGui.CalcTextSize("10000").X);
        ImGui.SameLine();
        if (ImGui.InputInt("##Slot2Bonus", ref slot2Bonus, 0))
        {
            slot1Bonus = Math.Clamp(slot2Bonus, 0, 100);
        }

        ImGui.EndChild();
        ImGui.PopFont();
    }

    public void Free()
    {

    }
}