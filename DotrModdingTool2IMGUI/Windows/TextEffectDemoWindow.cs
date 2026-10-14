using System.Globalization;
using System.Numerics;
using System.Text;
using ImGuiNET;
namespace DotrModdingTool2IMGUI;

public class TextEffectDemoWindow : IImGuiWindow
{
    public bool IsOpen = false;

    readonly ImFontPtr font;

    static readonly Vector4 Purple = new Vector4(0.6f, 0.2f, 0.9f, 1f);
    static readonly Vector4 Blue = new Vector4(0.39f, 0.58f, 0.93f, 1f);
    static readonly Vector4 Red = new Vector4(0.9f, 0.15f, 0.15f, 1f);
    static readonly Vector4 Gold = new Vector4(1f, 0.8f, 0.2f, 1f);
    static readonly Vector4 Green = new Vector4(0.2f, 0.85f, 0.4f, 1f);
    static readonly Vector4 Pink = new Vector4(1f, 0.45f, 0.75f, 1f);
    static readonly Vector4 Grey = new Vector4(0.55f, 0.55f, 0.55f, 1f);
    static readonly Vector4 White = Vector4.One;

    record Example(string Sample, string Code, TextEffect Effect);

    record Section(string Title, Example[] Examples);

    static readonly Section[] Sections = BuildSections();

    // Selectables section state
    bool selectableToggled = false;

    Playground playground = new Playground();

    public TextEffectDemoWindow(ImFontPtr font)
    {
        this.font = font;
    }

    static Section[] BuildSections()
    {
        return new[] {
            new Section("Presets", new[] {
                new Example("Rainbow", "TextEffect.Rainbow", TextEffect.Rainbow),
                new Example("Rainbow Whole", "TextEffect.RainbowWhole", TextEffect.RainbowWhole),
                new Example("Breathing", "TextEffect.Breathing", TextEffect.Breathing),
                new Example("Wavy Rainbow", "TextEffect.WavyRainbow", TextEffect.WavyRainbow),
                new Example("Gradient", "TextEffect.Gradient(purple, blue)", TextEffect.Gradient(Purple, Blue)),
                new Example("Solid", "TextEffect.Solid(gold)", TextEffect.Solid(Gold)),
            }),
            new Section("Colour sources", new[] {
                new Example("Multi stop gradient", "TextEffect.Gradient(red, gold, green, blue)", TextEffect.Gradient(Red, Gold, Green, Blue)),
                new Example("Red to blue in RGB", "TextEffect.Gradient(red, blue)", TextEffect.Gradient(Red, Blue)),
                new Example("Red to blue in HSV", "TextEffect.Gradient(red, blue) with { Blend = BlendSpace.Hsv }", TextEffect.Gradient(Red, Blue) with { Blend = BlendSpace.Hsv }),
                new Example("Pastel rainbow", "TextEffect.Rainbow with { Saturation = 0.45f, Value = 1f }", TextEffect.Rainbow with { Saturation = 0.45f, Value = 1f }),
                new Example("Dim breathing", "TextEffect.Breathing with { Brightness = 0.35f, Amplitude = 0.35f }", TextEffect.Breathing with { Brightness = 0.35f, Amplitude = 0.35f }),
                new Example("Half transparent", "TextEffect.Gradient(purple, blue) with { Alpha = 0.5f }", TextEffect.Gradient(Purple, Blue) with { Alpha = 0.5f }),
            }),
            new Section("Wave shapes", Enum.GetValues<WaveShape>()
                .Select(wave => new Example(wave.ToString(), $"TextEffect.Gradient(purple, gold) with {{ Wave = WaveShape.{wave}, Speed = 0.5f }}",
                    TextEffect.Gradient(Purple, Gold) with { Wave = wave, Speed = 0.5f }))
                .ToArray()),
            new Section("Easings (triangle wave)", Enum.GetValues<Easing>()
                .Select(easing => new Example(easing.ToString(), $"TextEffect.Gradient(purple, gold) with {{ Wave = WaveShape.Triangle, Easing = Easing.{easing}, Speed = 0.5f }}",
                    TextEffect.Gradient(Purple, Gold) with { Wave = WaveShape.Triangle, Easing = easing, Speed = 0.5f }))
                .ToArray()),
            new Section("Timing", new[] {
                new Example("Slow", "TextEffect.Rainbow with { Speed = 0.05f }", TextEffect.Rainbow with { Speed = 0.05f }),
                new Example("Fast", "TextEffect.Rainbow with { Speed = 1.5f }", TextEffect.Rainbow with { Speed = 1.5f }),
                new Example("No spread", "TextEffect.Rainbow with { Spread = 0f }", TextEffect.Rainbow with { Spread = 0f }),
                new Example("Wide spread", "TextEffect.Rainbow with { Spread = 0.12f }", TextEffect.Rainbow with { Spread = 0.12f }),
                new Example("Reversed", "TextEffect.Rainbow with { Reverse = true }", TextEffect.Rainbow with { Reverse = true }),
                new Example("Whole text", "TextEffect.Gradient(purple, gold) with { PerCharacter = false }", TextEffect.Gradient(Purple, Gold) with { PerCharacter = false }),
                new Example("Sine", "TextEffect.Gradient(red, blue) with { PerCharacter = false }", TextEffect.Gradient(Red, Blue) with { PerCharacter = false }),
                new Example("Cosine", "TextEffect.Gradient(red, blue) with { PerCharacter = false, PhaseOffset = 0.25f }", TextEffect.Gradient(Red, Blue) with { PerCharacter = false, PhaseOffset = 0.25f }),
            }),
            new Section("Character range", new[] {
                new Example("Only this word is animated", "TextEffect.Gradient(purple, gold) with { RangeStart = 5, RangeEnd = 9 }", TextEffect.Gradient(Purple, Gold) with { RangeStart = 5, RangeEnd = 9 }),
                new Example("Name: rest of the line", "TextEffect.Rainbow with { RangeEnd = 4, OutsideColour = grey }", TextEffect.Rainbow with { RangeEnd = 4, OutsideColour = Grey }),
            }),
            new Section("Motion", new[] {
                new Example("Bounce", "TextEffect.Solid(white) with { BounceHeight = 4f, BounceSpeed = 1f, BounceSpread = 0.1f }",
                    TextEffect.Solid(White) with { BounceHeight = 4f, BounceSpeed = 1f, BounceSpread = 0.1f }),
                new Example("Shake", "TextEffect.Solid(red) with { Shake = 1.5f }", TextEffect.Solid(Red) with { Shake = 1.5f }),
                new Example("Slow shake", "TextEffect.Solid(red) with { Shake = 2f, ShakeSpeed = 6f }", TextEffect.Solid(Red) with { Shake = 2f, ShakeSpeed = 6f }),
                new Example("Alpha pulse", "TextEffect.Solid(pink) with { AlphaPulse = 0.8f, AlphaPulseSpeed = 0.5f, Spread = 0.05f }",
                    TextEffect.Solid(Pink) with { AlphaPulse = 0.8f, AlphaPulseSpeed = 0.5f, Spread = 0.05f }),
                new Example("Everything", "TextEffect.Rainbow with { BounceHeight = 3f, BounceSpeed = 1.5f, AlphaPulse = 0.5f, Shake = 0.5f }",
                    TextEffect.Rainbow with { BounceHeight = 3f, BounceSpeed = 1.5f, AlphaPulse = 0.5f, Shake = 0.5f }),
            }),
        };
    }

    public void Render()
    {
        if (!IsOpen)
            return;

        ImGui.SetNextWindowSize(new Vector2(1100, 800), ImGuiCond.FirstUseEver);
        ImGui.PushFont(font);
        if (ImGui.Begin("Text Effects Demo", ref IsOpen))
        {
            if (ImGui.BeginTabBar("TextEffectDemoTabs"))
            {
                if (ImGui.BeginTabItem("Examples"))
                {
                    RenderExamples();
                    ImGui.EndTabItem();
                }
                if (ImGui.BeginTabItem("Playground"))
                {
                    RenderPlayground();
                    ImGui.EndTabItem();
                }
                ImGui.EndTabBar();
            }
        }
        ImGui.End();
        ImGui.PopFont();
    }

    void RenderExamples()
    {
        ImGui.BeginChild("ExamplesScroll");
        foreach (Section section in Sections)
        {
            if (!ImGui.CollapsingHeader(section.Title, ImGuiTreeNodeFlags.DefaultOpen))
                continue;

            if (ImGui.BeginTable(section.Title, 2, ImGuiTableFlags.RowBg | ImGuiTableFlags.BordersInnerV))
            {
                ImGui.TableSetupColumn("Effect", ImGuiTableColumnFlags.WidthFixed, 380f);
                ImGui.TableSetupColumn("Code", ImGuiTableColumnFlags.WidthStretch);
                foreach (Example example in section.Examples)
                {
                    ImGui.TableNextRow();
                    ImGui.TableNextColumn();
                    TextRenderer.Text(example.Sample, example.Effect);
                    ImGui.TableNextColumn();
                    ImGui.TextDisabled(example.Code);
                    if (ImGui.IsItemClicked())
                    {
                        ImGui.SetClipboardText(example.Code);
                    }
                    if (ImGui.IsItemHovered())
                    {
                        ImGui.SetTooltip("Click to copy");
                    }
                }
                ImGui.EndTable();
            }
        }

        if (ImGui.CollapsingHeader("Selectables", ImGuiTreeNodeFlags.DefaultOpen))
        {
            ImGui.TextDisabled("TextRenderer.Selectable(text, selected, effect, flags, id)");
            TextRenderer.Selectable("Text width selectable", false, TextEffect.Gradient(Purple, Blue));
            TextRenderer.Selectable("Fill width selectable", false, TextEffect.Gradient(Purple, Blue) with { FillWidth = true });
            TextRenderer.Selectable("Custom highlight", false, TextEffect.Rainbow with { HighlightColour = new Vector4(1f, 0.8f, 0.2f, 0.25f) });
            if (TextRenderer.Selectable(selectableToggled ? "Selected (click me)" : "Not selected (click me)", selectableToggled, TextEffect.Breathing,
                    ImGuiSelectableFlags.None, "ToggleSelectable"))
            {
                selectableToggled = !selectableToggled;
            }
            ImGui.TextDisabled("Same text needs an id to stay separate:");
            TextRenderer.Selectable("Duplicate", false, TextEffect.Gradient(Red, Gold), ImGuiSelectableFlags.None, "Duplicate1");
            TextRenderer.Selectable("Duplicate", false, TextEffect.Gradient(Green, Blue), ImGuiSelectableFlags.None, "Duplicate2");
        }
        ImGui.EndChild();
    }

    void RenderPlayground()
    {
        Playground p = playground;

        ImGui.Text("Load:");
        ImGui.SameLine();
        if (ImGui.Button("Rainbow")) p.LoadFrom(TextEffect.Rainbow);
        ImGui.SameLine();
        if (ImGui.Button("Breathing")) p.LoadFrom(TextEffect.Breathing);
        ImGui.SameLine();
        if (ImGui.Button("Wavy Rainbow")) p.LoadFrom(TextEffect.WavyRainbow);
        ImGui.SameLine();
        if (ImGui.Button("Gradient")) p.LoadFrom(TextEffect.Gradient(Purple, Blue));
        ImGui.SameLine();
        if (ImGui.Button("Reset")) playground = p = new Playground();

        TextEffect effect = p.ToEffect();

        // Preview
        ImGui.Separator();
        ImGui.InputText("Preview text", ref p.Text, 128);
        ImGui.Checkbox("Show as selectable", ref p.AsSelectable);
        ImGui.Spacing();
        if (p.AsSelectable)
            TextRenderer.Selectable(p.Text, false, effect, ImGuiSelectableFlags.None, "PlaygroundPreview");
        else
            TextRenderer.Text(p.Text, effect);
        ImGui.Spacing();
        ImGui.Separator();

        float half = ImGui.GetContentRegionAvail().X * 0.5f;
        ImGui.BeginChild("PlaygroundControls", new Vector2(half, 0));
        ImGui.PushItemWidth(half * 0.55f);

        ImGui.SeparatorText("Colour");
        EnumCombo("Source", ref p.Source);
        if (p.Source == ColourSource.Gradient || p.Source == ColourSource.Solid)
        {
            if (p.Source == ColourSource.Gradient)
            {
                ImGui.SliderInt("Stops", ref p.StopCount, 1, Playground.MaxStops);
                EnumCombo("Blend", ref p.Blend);
            }
            int count = p.Source == ColourSource.Solid ? 1 : p.StopCount;
            for (int i = 0; i < count; i++)
            {
                ImGui.ColorEdit4($"Stop {i + 1}", ref p.Stops[i], ImGuiColorEditFlags.NoInputs);
            }
        }
        if (p.Source == ColourSource.Rainbow)
        {
            ImGui.SliderFloat("Saturation", ref p.Saturation, 0f, 1f);
            ImGui.SliderFloat("Value", ref p.Value, 0f, 1f);
        }
        if (p.Source == ColourSource.RainbowSine)
        {
            ImGui.SliderFloat("Brightness", ref p.Brightness, 0f, 1f);
            ImGui.SliderFloat("Amplitude", ref p.Amplitude, 0f, 1f);
        }
        ImGui.SliderFloat("Alpha", ref p.Alpha, 0f, 1f);

        ImGui.SeparatorText("Animation");
        EnumCombo("Wave", ref p.Wave);
        EnumCombo("Easing", ref p.Easing);
        ImGui.SliderFloat("Speed", ref p.Speed, 0f, 3f);
        ImGui.SliderFloat("Spread", ref p.Spread, 0f, 0.3f);
        ImGui.SliderFloat("Phase offset", ref p.PhaseOffset, 0f, 1f);
        ImGui.Checkbox("Reverse", ref p.Reverse);
        ImGui.SameLine();
        ImGui.Checkbox("Per character", ref p.PerCharacter);

        ImGui.SeparatorText("Range");
        ImGui.Checkbox("Limit range", ref p.UseRange);
        if (p.UseRange)
        {
            ImGui.SliderInt("Range start", ref p.RangeStart, 0, Math.Max(p.Text.Length, 1));
            ImGui.SliderInt("Range end", ref p.RangeEnd, 0, Math.Max(p.Text.Length, 1));
            ImGui.Checkbox("Outside colour", ref p.UseOutsideColour);
            if (p.UseOutsideColour)
            {
                ImGui.SameLine();
                ImGui.ColorEdit4("##OutsideColour", ref p.OutsideColour, ImGuiColorEditFlags.NoInputs);
            }
        }

        ImGui.SeparatorText("Motion");
        ImGui.SliderFloat("Bounce height", ref p.BounceHeight, 0f, 10f);
        ImGui.SliderFloat("Bounce speed", ref p.BounceSpeed, 0f, 3f);
        ImGui.SliderFloat("Bounce spread", ref p.BounceSpread, 0f, 0.5f);
        ImGui.SliderFloat("Shake", ref p.Shake, 0f, 5f);
        ImGui.SliderFloat("Shake speed", ref p.ShakeSpeed, 1f, 60f);
        ImGui.SliderFloat("Alpha pulse", ref p.AlphaPulse, 0f, 1f);
        ImGui.SliderFloat("Alpha pulse speed", ref p.AlphaPulseSpeed, 0f, 3f);

        ImGui.SeparatorText("Selectable");
        ImGui.Checkbox("Fill width", ref p.FillWidth);
        ImGui.Checkbox("Highlight colour", ref p.UseHighlight);
        if (p.UseHighlight)
        {
            ImGui.SameLine();
            ImGui.ColorEdit4("##HighlightColour", ref p.Highlight, ImGuiColorEditFlags.NoInputs | ImGuiColorEditFlags.AlphaBar);
        }

        ImGui.PopItemWidth();
        ImGui.EndChild();

        ImGui.SameLine();
        ImGui.BeginChild("PlaygroundCode", Vector2.Zero);
        string code = p.ToCode();
        if (ImGui.Button("Copy code"))
        {
            ImGui.SetClipboardText(code);
        }
        ImGui.InputTextMultiline("##PlaygroundCode", ref code, (uint)code.Length + 1, ImGui.GetContentRegionAvail(), ImGuiInputTextFlags.ReadOnly);
        ImGui.EndChild();
    }

    static void EnumCombo<T>(string label, ref T value) where T : struct, Enum
    {
        int index = Array.IndexOf(EnumCache<T>.Values, value);
        if (ImGui.Combo(label, ref index, EnumCache<T>.Names, EnumCache<T>.Names.Length))
        {
            value = EnumCache<T>.Values[index];
        }
    }

    static class EnumCache<T> where T : struct, Enum
    {
        public static readonly T[] Values = Enum.GetValues<T>();
        public static readonly string[] Names = Enum.GetNames<T>();
    }

    // Mutable mirror of TextEffect so ImGui controls can edit it by ref
    class Playground
    {
        public const int MaxStops = 6;

        public string Text = "Text effects are fun!";
        public bool AsSelectable = false;

        public ColourSource Source = ColourSource.Gradient;
        public int StopCount = 2;
        public Vector4[] Stops = { Purple, Blue, Pink, Gold, Green, Red };
        public BlendSpace Blend = BlendSpace.Rgb;
        public float Saturation = 0.9f;
        public float Value = 0.95f;
        public float Brightness = 0.5f;
        public float Amplitude = 0.5f;
        public float Alpha = 1f;

        public WaveShape Wave = WaveShape.Sine;
        public Easing Easing = Easing.Linear;
        public float Speed = 0.3f;
        public float Spread = 0.05f;
        public float PhaseOffset = 0f;
        public bool Reverse = false;
        public bool PerCharacter = true;

        public bool UseRange = false;
        public int RangeStart = 0;
        public int RangeEnd = 4;
        public bool UseOutsideColour = false;
        public Vector4 OutsideColour = Grey;

        public float BounceHeight = 0f;
        public float BounceSpeed = 1f;
        public float BounceSpread = 0.1f;
        public float Shake = 0f;
        public float ShakeSpeed = 20f;
        public float AlphaPulse = 0f;
        public float AlphaPulseSpeed = 1f;

        public bool FillWidth = false;
        public bool UseHighlight = false;
        public Vector4 Highlight = new Vector4(1f, 1f, 1f, 0.15f);

        int ActiveStopCount => Source == ColourSource.Solid ? 1 : StopCount;

        public TextEffect ToEffect()
        {
            return new TextEffect {
                Source = Source,
                Stops = Stops[..ActiveStopCount],
                Blend = Blend,
                Saturation = Saturation,
                Value = Value,
                Brightness = Brightness,
                Amplitude = Amplitude,
                Alpha = Alpha,
                Wave = Wave,
                Easing = Easing,
                Speed = Speed,
                Spread = Spread,
                PhaseOffset = PhaseOffset,
                Reverse = Reverse,
                PerCharacter = PerCharacter,
                RangeStart = UseRange ? RangeStart : 0,
                RangeEnd = UseRange ? RangeEnd : int.MaxValue,
                OutsideColour = UseRange && UseOutsideColour ? OutsideColour : null,
                BounceHeight = BounceHeight,
                BounceSpeed = BounceSpeed,
                BounceSpread = BounceSpread,
                Shake = Shake,
                ShakeSpeed = ShakeSpeed,
                AlphaPulse = AlphaPulse,
                AlphaPulseSpeed = AlphaPulseSpeed,
                FillWidth = FillWidth,
                HighlightColour = UseHighlight ? Highlight : null
            };
        }

        public void LoadFrom(TextEffect fx)
        {
            Source = fx.Source;
            if (fx.Stops.Length > 0)
            {
                StopCount = Math.Min(fx.Stops.Length, MaxStops);
                Array.Copy(fx.Stops, Stops, StopCount);
            }
            Blend = fx.Blend;
            Saturation = fx.Saturation;
            Value = fx.Value;
            Brightness = fx.Brightness;
            Amplitude = fx.Amplitude;
            Alpha = fx.Alpha;
            Wave = fx.Wave;
            Easing = fx.Easing;
            Speed = fx.Speed;
            Spread = fx.Spread;
            PhaseOffset = fx.PhaseOffset;
            Reverse = fx.Reverse;
            PerCharacter = fx.PerCharacter;
            UseRange = fx.RangeStart != 0 || fx.RangeEnd != int.MaxValue;
            RangeStart = fx.RangeStart;
            RangeEnd = UseRange ? fx.RangeEnd : 4;
            UseOutsideColour = fx.OutsideColour.HasValue;
            OutsideColour = fx.OutsideColour ?? Grey;
            BounceHeight = fx.BounceHeight;
            BounceSpeed = fx.BounceSpeed;
            BounceSpread = fx.BounceSpread;
            Shake = fx.Shake;
            ShakeSpeed = fx.ShakeSpeed;
            AlphaPulse = fx.AlphaPulse;
            AlphaPulseSpeed = fx.AlphaPulseSpeed;
            FillWidth = fx.FillWidth;
            UseHighlight = fx.HighlightColour.HasValue;
            Highlight = fx.HighlightColour ?? new Vector4(1f, 1f, 1f, 0.15f);
        }

        // Only writes the values that differ from a default TextEffect
        public string ToCode()
        {
            TextEffect d = new TextEffect();
            TextEffect fx = ToEffect();
            StringBuilder sb = new StringBuilder("new TextEffect {\n");

            void Line(string s) => sb.Append("    ").Append(s).Append(",\n");

            if (fx.Source != d.Source) Line($"Source = ColourSource.{fx.Source}");
            if (fx.Source == ColourSource.Gradient || fx.Source == ColourSource.Solid)
                Line($"Stops = new[] {{ {string.Join(", ", fx.Stops.Select(V4))} }}");
            if (fx.Source == ColourSource.Gradient && fx.Blend != d.Blend) Line($"Blend = BlendSpace.{fx.Blend}");
            if (fx.Source == ColourSource.Rainbow)
            {
                if (fx.Saturation != d.Saturation) Line($"Saturation = {F(fx.Saturation)}");
                if (fx.Value != d.Value) Line($"Value = {F(fx.Value)}");
            }
            if (fx.Source == ColourSource.RainbowSine)
            {
                if (fx.Brightness != d.Brightness) Line($"Brightness = {F(fx.Brightness)}");
                if (fx.Amplitude != d.Amplitude) Line($"Amplitude = {F(fx.Amplitude)}");
            }
            if (fx.Alpha != d.Alpha) Line($"Alpha = {F(fx.Alpha)}");
            if (fx.Wave != d.Wave) Line($"Wave = WaveShape.{fx.Wave}");
            if (fx.Easing != d.Easing) Line($"Easing = Easing.{fx.Easing}");
            if (fx.Speed != d.Speed) Line($"Speed = {F(fx.Speed)}");
            if (fx.Spread != d.Spread) Line($"Spread = {F(fx.Spread)}");
            if (fx.PhaseOffset != d.PhaseOffset) Line($"PhaseOffset = {F(fx.PhaseOffset)}");
            if (fx.Reverse != d.Reverse) Line("Reverse = true");
            if (fx.PerCharacter != d.PerCharacter) Line("PerCharacter = false");
            if (fx.RangeStart != d.RangeStart) Line($"RangeStart = {fx.RangeStart}");
            if (fx.RangeEnd != d.RangeEnd) Line($"RangeEnd = {fx.RangeEnd}");
            if (fx.OutsideColour is Vector4 outside) Line($"OutsideColour = {V4(outside)}");
            if (fx.BounceHeight != d.BounceHeight)
            {
                Line($"BounceHeight = {F(fx.BounceHeight)}");
                if (fx.BounceSpeed != d.BounceSpeed) Line($"BounceSpeed = {F(fx.BounceSpeed)}");
                if (fx.BounceSpread != d.BounceSpread) Line($"BounceSpread = {F(fx.BounceSpread)}");
            }
            if (fx.Shake != d.Shake)
            {
                Line($"Shake = {F(fx.Shake)}");
                if (fx.ShakeSpeed != d.ShakeSpeed) Line($"ShakeSpeed = {F(fx.ShakeSpeed)}");
            }
            if (fx.AlphaPulse != d.AlphaPulse)
            {
                Line($"AlphaPulse = {F(fx.AlphaPulse)}");
                if (fx.AlphaPulseSpeed != d.AlphaPulseSpeed) Line($"AlphaPulseSpeed = {F(fx.AlphaPulseSpeed)}");
            }
            if (fx.FillWidth != d.FillWidth) Line("FillWidth = true");
            if (fx.HighlightColour is Vector4 highlight) Line($"HighlightColour = {V4(highlight)}");

            sb.Append('}');
            return sb.ToString();
        }

        static string F(float value) => value.ToString("0.###", CultureInfo.InvariantCulture) + "f";

        static string V4(Vector4 v) => $"new Vector4({F(v.X)}, {F(v.Y)}, {F(v.Z)}, {F(v.W)})";
    }

    public void Free()
    {

    }
}
