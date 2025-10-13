using System.Drawing;
using System.Numerics;
using ImGuiNET;
namespace DotrModdingTool2IMGUI;

public class StringEditorWindow : IImGuiWindow
{
    ImFontPtr font;
    ImFontPtr jpFont;
    int currentStringIndex;
    int currentSectionIndex;
    int currentCardNameIndex;
    int currentMonsterEffectIndex;
    int currentDuelistNameIndex;
    int currentDuelArenaNameIndex;
    public static ModdedStringName[] ArenaNames;

    string cardSearchString = string.Empty;

    string[] defaultArenaNames = new string[] {
        "Stonehenge",
        "Stonehenge",
        "Chester",
        "Tewkesbury",
        "Towton",
        "Isle of Man",
        "Exeter",
        "St.Albans",
        "Newcastle",
        "Lancashire",
        "Bosworth",
        "Windsor",
        "London",
        "Canterbury",
        "Strait of Dover",
        "Amiens",
        "Paris",
        "Le Mans",
        "Rennes",
        "Brest",
        "Stonehenge",
        "Stonehenge",
        "Milford Haven",
        "Dover",
        "Boulogne"
    };

    Dictionary<String, int> Sections = new Dictionary<string, int>() {
        { "System (uneditable)", 0 },
        { "Card Types", StringEditor.CardTypesStart },
        { "Monster Types", StringEditor.MonsterTypesStart },
        { "Attributes", StringEditor.AttributesStart },
        { "Players", StringEditor.PlayersStart },
        { "Terrain", StringEditor.TerrainStart },
        { "Leader Ranks", StringEditor.LeaderRanksStart },
        { "Misc Text 1", StringEditor.MiscText1 },
        { "Leader Abilities", StringEditor.LeaderAbilitiesStart },
        { "Misc Text 2", StringEditor.MiscText2 },
        { "Custom Duelist Name", StringEditor.CustomDuelistNameStart },
        { "Misc Text 3", StringEditor.MiscText3 },
        { "Duelist Name", StringEditor.DuelistNameOffsetStart },
        { "Duel Arena Names", StringEditor.DuelArenaNamesStart },
        { "Misc Text 4", StringEditor.MiscText2Start },
        { "Debug Menu", StringEditor.DebugMenu },
        { "Card Names", StringEditor.CardNamesOffsetStart },
        { "Card Effect Text", StringEditor.CardEffectTextOffsetStart },
        { "Intro Dialogue", StringEditor.LancasterIntroStart },
        { "Enemy Dialogue Red", StringEditor.YorkistsDuelistsDialogueStart },
        { "Intro Dialogue White", StringEditor.YorkistSideIntroStart },
        { "Enemy Dialogue White", StringEditor.LancasterDuelistDialogueStart },
        { "Memory Card Stuff", StringEditor.MemoryCardStuffStart },
        { "Tutorial", StringEditor.TutorialStart },

    };

    record Section(string Label, int[] Indices);

    static List<Section> SectionsLabels;

    static Dictionary<int, string> SectionLabelLookup;

    public StringEditorWindow()
    {
        font = Fonts.MonoSpace;
        jpFont = Fonts.JapaneseFont;
        EditorWindow.OnIsoLoaded += OnIsoLoaded;
        ArenaNames = new ModdedStringName[defaultArenaNames.Length];
        for (int i = 0; i < defaultArenaNames.Length; i++)
        {
            ArenaNames[i] = new ModdedStringName(defaultArenaNames[i], defaultArenaNames[i]);
        }
    }

    void OnIsoLoaded()
    {
        ReloadStrings();


    }

    public static void ReloadStrings()
    {
        for (int i = StringEditor.DuelArenaNamesStart; i <= StringEditor.DuelArenaNamesEnd; i++)
        {
            ArenaNames[i - StringEditor.DuelArenaNamesStart].Edited = StringEditor.StringTable[i];
        }

    
        SectionsLabels = new() {
            new(Enemies.EnemyNameList[2].Current, new[] { 2183, 2184, 2185 }), // weevil
            new(Enemies.EnemyNameList[3].Current, new[] { 2186, 2187, 2188, 2189 }), // rex
            new(Enemies.EnemyNameList[8].Current, new[] { 2190, 2191, 2192, 2193 }), //lab ruler
            new(Enemies.EnemyNameList[6].Current, new[] { 2194, 2195, 2196 }), //necromancer
            new(Enemies.EnemyNameList[4].Current, new[] { 2197, 2198, 2199 }), //keith
            new(Enemies.EnemyNameList[7].Current, new[] { 2200, 2201, 2202 }), //Dark ruler
            new(Enemies.EnemyNameList[10].Current, new[] { 2203, 2204, 2205, 2206 }), //Richard
            new(Enemies.EnemyNameList[9].Current, new[] { 2208, 2209, 2210, 2211, 2212, 2213, 2214, 2215, 2216, 2217, 2218 }), // Pegasus
            //new(Enemies.EnemyNameList[10].Current, new[] { 2203, 2204, 2205, 2206 }), //Richard
        };
        SectionLabelLookup = SectionsLabels
            .SelectMany(s => s.Indices.Select(i => (i, s.Label)))
            .ToDictionary(x => x.i, x => x.Label);



    }

    public void Render()
    {
        ImGui.PushFont(jpFont);
        if (!DataAccess.Instance.IsIsoLoaded)
        {
            ImGui.Text($"Please load ISO file");
            ImGui.PopFont();
            return;
        }


        string[] sectionArray = Sections.Keys.ToArray();

        Vector2 windowSize = ImGui.GetWindowSize();
        string[] CardNames = Card.GetCardStringArray();
        string[] EnemyNames = Enemies.GetEnemyNameArray();
        EnemyNames[20] = $"{StringEditor.StringTable[20 + StringEditor.DuelistNameOffsetStart]} ({Deck.DeckList[20 + 26].DeckLeader.Name.Current})";
        EnemyNames[21] = $"{StringEditor.StringTable[21 + StringEditor.DuelistNameOffsetStart]} ({Deck.DeckList[21 + 26].DeckLeader.Name.Current})";
        string[] arenas = ArenaNames.Select(c => c.Current ?? "").ToArray();

        ImGui.TextColored(new GuiColour(Color.Orange).value, "Some strings are uneditable for convenience and encoding sakes");
        if (ImGui.Button("Remove tutorial strings"))
        {
            for (int i = StringEditor.TutorialStart; i < StringEditor.StringTable.Count; i++)
                StringEditor.StringTable[i] = "~";
        }
        if (ImGui.IsItemHovered())
        {
            ImGui.BeginTooltip();
            ImGui.Text("Use this to make room for longer or more strings on other card texts\nWorks well with fast intro mod");
            ImGui.EndTooltip();

        }

        ImGui.Dummy(new Vector2(0, 10));
        ImGui.BeginChild("LeftHalfPanel", new Vector2(windowSize.X / 2.5f, 0), ImGuiChildFlags.Border);
        ImGui.Text("Index");
        ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
        if (ImGui.InputInt("##StringIndex", ref currentStringIndex, 1))
        {

            if (currentStringIndex < 0)
            {
                currentStringIndex = StringEditor.StringTable.Count - 1;
            }
            if (currentStringIndex >= StringEditor.StringTable.Count)
            {
                currentStringIndex = 0;
            }
        }
        UpdateIndexOnScrollHover();
        if (CardNames is null)
        {
            ImGui.PopFont();
            return;
        }
        ImGui.Text("Jump to card name");

        if (ImGui.BeginCombo("##CardName", CardNames[currentCardNameIndex]))
        {
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            ImGui.InputText("##SearchCardName", ref cardSearchString, 20);

            string[] filteredStrings = CardNames.Where(s => string.IsNullOrEmpty(cardSearchString) || s.Contains(cardSearchString, StringComparison.OrdinalIgnoreCase)).ToArray();

            for (int i = 0; i < filteredStrings.Length; i++)
            {
                string name = filteredStrings[i];
                int realIndex = Array.IndexOf(CardNames, name);
                bool isSelected = (currentCardNameIndex == realIndex);
                if (ImGui.Selectable(name, isSelected))
                {
                    currentCardNameIndex = realIndex;
                    currentStringIndex = StringEditor.CardNamesOffsetStart + currentCardNameIndex;
                    cardSearchString = "";
                }

            }

            ImGui.EndCombo();
        }


        ImGui.Text("Jump to card effect");
        if (ImGui.BeginCombo("##CardEffect", CardNames[currentMonsterEffectIndex]))
        {
            ImGui.SetNextItemWidth(ImGui.GetContentRegionAvail().X);
            ImGui.InputText("##SearchCardEffect", ref cardSearchString, 20);

            string[] filteredStrings =
                CardNames.Where(s => string.IsNullOrEmpty(cardSearchString) || s.Contains(cardSearchString, StringComparison.OrdinalIgnoreCase)).ToArray();


            for (int i = 0; i < filteredStrings.Length; i++)
            {
                string name = filteredStrings[i];
                int realIndex = Array.IndexOf(CardNames, name);
                bool isSelected = (currentCardNameIndex == realIndex);
                if (ImGui.Selectable(name, isSelected))
                {
                    currentMonsterEffectIndex = realIndex;
                    currentStringIndex = StringEditor.CardEffectTextOffsetStart + currentMonsterEffectIndex;
                    cardSearchString = "";
                }

            }
            ImGui.EndCombo();
        }


        ImGui.Text("Jump to duelist");
        if (ImGui.BeginCombo("##DuelistName", EnemyNames[currentDuelistNameIndex]))
        {
            for (int i = 0; i < EnemyNames.Length; i++)
            {
                bool isSelected = (currentDuelistNameIndex == i);
                if (ImGui.Selectable(EnemyNames[i], isSelected))
                {
                    currentDuelistNameIndex = i;
                    if (currentDuelistNameIndex >= 22)
                        currentStringIndex = StringEditor.CustomDuelistNameStart + currentDuelistNameIndex - 22;
                    else
                        currentStringIndex = StringEditor.DuelistNameOffsetStart + currentDuelistNameIndex;
                }
                if (isSelected)
                    ImGui.SetItemDefaultFocus();
            }
            ImGui.EndCombo();
        }


        ImGui.Text("Jump to arena names");
        if (ImGui.BeginCombo("##DuelArena", arenas[currentDuelArenaNameIndex]))
        {
            for (int i = 0; i < arenas.Length; i++)
            {
                bool isSelected = (currentDuelArenaNameIndex == i);
                if (ImGui.Selectable(arenas[i], isSelected))
                {
                    currentDuelArenaNameIndex = i;
                    currentStringIndex = StringEditor.DuelArenaNamesStart + currentDuelArenaNameIndex;
                }
                if (isSelected)
                    ImGui.SetItemDefaultFocus();
            }
            ImGui.EndCombo();
        }

        ImGui.Dummy(new Vector2(0, 20));

        ImGui.Text("Text Sections");
        if (ImGui.BeginListBox("##Section", new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetContentRegionAvail().Y - windowSize.Y / 20)))
        {
            for (int i = 0; i < sectionArray.Length; i++)
            {
                bool isSelected = (currentSectionIndex == i);
                if (ImGui.Selectable(sectionArray[i], isSelected))
                {
                    currentSectionIndex = i;
                    currentStringIndex = Sections[sectionArray[i]];
                }
                if (isSelected)
                    ImGui.SetItemDefaultFocus();
            }
            ImGui.EndListBox();
        }
        ImGui.EndChild();

        ImGui.SameLine();
        ImGui.Dummy(new Vector2(windowSize.X / 80f, 0));
        ImGui.SameLine();
        ImGui.BeginChild("RightHalfPanel", new Vector2(ImGui.GetContentRegionAvail().X - ImGui.GetStyle().ItemSpacing.X, 0), ImGuiChildFlags.Border);
        ImGui.PushStyleColor(ImGuiCol.Text, new GuiColour(Color.CornflowerBlue).value);
        ImGui.Text("Use the scroll wheel to change the index");
        ImGui.Text("Scroll does not work will editing text (click out of the box)");
        ImGui.PopStyleColor();
        if (ImGui.BeginListBox("##String", ImGui.GetContentRegionAvail()))
        {
            UpdateIndexOnScrollWindow();

            int prevIndex = (currentStringIndex - 1 + StringEditor.StringTable.Count) % StringEditor.StringTable.Count;
            int nextIndex = (currentStringIndex + 1) % StringEditor.StringTable.Count;
            RenderMultiLineBox(prevIndex, "##PrevString");
            RenderMultiLineBox(currentStringIndex, "##CurrentString");
            RenderMultiLineBox(nextIndex, "##NextString");
            ImGui.EndListBox();
        }
        ImGui.EndChild();


        ImGui.PopFont();
    }

    void UpdateIndexOnScrollWindow()
    {

        bool inputActive = ImGui.IsAnyItemActive();
        if (!inputActive && ImGui.IsWindowHovered(ImGuiHoveredFlags.ChildWindows))
        {
            float wheel = ImGui.GetIO().MouseWheel; // positive = up, negative = down
            if (wheel > 0f)
            {
                currentStringIndex = (currentStringIndex - 1 + StringEditor.StringTable.Count) % StringEditor.StringTable.Count;
            }
            else if (wheel < 0f)
            {
                currentStringIndex = (currentStringIndex + 1) % StringEditor.StringTable.Count;
            }
        }
    }

    void UpdateIndexOnScrollHover()
    {

        bool inputActive = ImGui.IsAnyItemActive();
        if (!inputActive && ImGui.IsItemHovered())
        {
            float wheel = ImGui.GetIO().MouseWheel;
            if (wheel > 0f)
            {
                currentStringIndex = (currentStringIndex - 1 + StringEditor.StringTable.Count) % StringEditor.StringTable.Count;
            }
            else if (wheel < 0f)
            {
                currentStringIndex = (currentStringIndex + 1) % StringEditor.StringTable.Count;
            }
        }
    }

    public void Free()
    {

    }

    void RenderMultiLineBox(int index, string label)
    {
        bool uneditable = StringEditor.Uneditable.Contains(index);
        string text = StringEditor.StringTable[index];
        string nameText = string.Empty;
        if (index >= StringEditor.CardEffectTextOffsetStart && index <= StringEditor.CardEffectTextOffsetEnd)
        {
            nameText = StringEditor.StringTable[StringEditor.CardNamesOffsetStart + index - StringEditor.CardEffectTextOffsetStart];
        }
        if (SectionLabelLookup.TryGetValue(index, out var sectionLabel))
        {
            nameText = sectionLabel;
        }


        ImGui.Text($"Index: {index}{(uneditable ? " (uneditable)" : "")} \n{nameText}");
        if (ImGui.InputTextMultiline(label, ref text, 4000, new Vector2(ImGui.GetContentRegionAvail().X, ImGui.GetTextLineHeight() * 8)))
        {
            if (uneditable)
                return;
            if (index >= StringEditor.DuelistNameOffsetStart && index <= StringEditor.DuelistNameOffsetEnd)
            {
                Enemies.EnemyNameList[index - StringEditor.DuelistNameOffsetStart].Edited = text;
                Enemies.RebuildStringCache();
                Map.OnEnemiesChanged();
                MusicEditorWindow.ReloadStrings();
            }
            else if (index >= StringEditor.CardNamesOffsetStart && index <= StringEditor.CardNamesOffsetEnd)
            {
                Card.cardNameList[index - StringEditor.CardNamesOffsetStart].Edited = text;
                Card.RebuildStringCache();
            }
            else if (index >= StringEditor.DuelArenaNamesStart && index <= StringEditor.DuelArenaNamesEnd)
            {
                ArenaNames[index - StringEditor.DuelArenaNamesStart].Edited = text;
            }

            StringEditor.StringTable[index] = text;
        }

    }
}