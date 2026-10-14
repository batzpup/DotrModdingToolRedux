using System.Numerics;
using ImGuiNET;
namespace DotrModdingTool2IMGUI;

public enum ColourSource
{
    Solid,
    Rainbow, // HSV hue cycle
    RainbowSine, // RGB channels as offset sine waves ("breathing")
    Gradient // blend between Stops
}

// How the 0-1 animation value moves over one cycle
public enum WaveShape
{
    Sine, // smooth back and forth
    Triangle, // linear back and forth
    Sawtooth, // 0 -> 1 then jump back to 0
    Square, // hard flip between the two ends
    Hold // ramp up, pause, ramp down, pause
}

// How the colour moves between stops, applied on top of the wave
public enum Easing
{
    Linear,
    Smoothstep,
    InSine,
    OutSine,
    InOutSine,
    InQuad,
    OutQuad,
    InOutQuad,
    InCubic,
    OutCubic,
    InOutCubic,
    OutElastic,
    OutBounce
}

public enum BlendSpace
{
    Rgb,
    Hsv // travels round the colour wheel (shortest way) instead of through grey
}

// All timing is in cycles: Speed = cycles per second, Spread = cycles of offset per character, 1 cycle = one full loop
public readonly record struct TextEffect
{
    public TextEffect() { }

    // Colour
    public ColourSource Source { get; init; } = ColourSource.Solid;
    public Vector4[] Stops { get; init; } = { Vector4.One };
    public BlendSpace Blend { get; init; } = BlendSpace.Rgb;
    public float Saturation { get; init; } = 0.9f; // Rainbow
    public float Value { get; init; } = 0.95f; // Rainbow
    public float Brightness { get; init; } = 0.5f; // RainbowSine
    public float Amplitude { get; init; } = 0.5f; // RainbowSine
    public float Alpha { get; init; } = 1f; // multiplier on the final alpha

    // Animation
    public WaveShape Wave { get; init; } = WaveShape.Sine;
    public Easing Easing { get; init; } = Easing.Linear;
    public float Speed { get; init; } = 1f;
    public float Spread { get; init; } = 0.05f;
    public float PhaseOffset { get; init; } = 0f; // 0.25 turns a sine into a cosine
    public bool Reverse { get; init; } = false; // run the wave right to left
    public bool PerCharacter { get; init; } = true; // false = whole text is one colour

    // Range, characters outside [RangeStart, RangeEnd) get OutsideColour (null = style text colour) and no effects
    public int RangeStart { get; init; } = 0;
    public int RangeEnd { get; init; } = int.MaxValue;
    public Vector4? OutsideColour { get; init; } = null;

    // Motion
    public float BounceHeight { get; init; } = 0f; // pixels
    public float BounceSpeed { get; init; } = 1f;
    public float BounceSpread { get; init; } = 0.1f;
    public float Shake { get; init; } = 0f; // pixels
    public float ShakeSpeed { get; init; } = 20f; // new positions per second
    public float AlphaPulse { get; init; } = 0f; // 0-1, how far alpha dips
    public float AlphaPulseSpeed { get; init; } = 1f; // uses Spread for per character offset

    // Selectable
    public bool FillWidth { get; init; } = false; // clickable area fills the available width
    public Vector4? HighlightColour { get; init; } = null; // extra box drawn behind the text when hovered/selected

    // Presets
    public static TextEffect Solid(Vector4 colour) => new TextEffect { Source = ColourSource.Solid, Stops = new[] { colour } };

    public static TextEffect Gradient(params Vector4[] stops) => new TextEffect {
        Source = ColourSource.Gradient,
        Stops = stops,
        Wave = WaveShape.Sine,
        Speed = 2f / MathF.Tau,
        Spread = 0.4f / MathF.Tau
    };

    // Smooth hue cycle per character
    public static readonly TextEffect Rainbow = new TextEffect {
        Source = ColourSource.Rainbow,
        Wave = WaveShape.Sawtooth,
        Speed = 0.3f,
        Spread = 0.03f
    };

    public static readonly TextEffect RainbowWhole = Rainbow with { PerCharacter = false };

    public static readonly TextEffect Breathing = new TextEffect {
        Source = ColourSource.RainbowSine,
        Brightness = 0.5f,
        Amplitude = 0.8f,
        Wave = WaveShape.Sawtooth,
        Speed = 3f / MathF.Tau,
        Spread = 0.4f / MathF.Tau
    };

    public static readonly TextEffect WavyRainbow = new TextEffect {
        Source = ColourSource.RainbowSine,
        Brightness = 0.6f,
        Amplitude = 0.6f,
        Wave = WaveShape.Sawtooth,
        Speed = 1f / MathF.Tau,
        Spread = 0.4f / MathF.Tau,
        BounceHeight = 3f,
        BounceSpeed = 10f / MathF.Tau,
        BounceSpread = 4f / MathF.Tau
    };
}

public static class Ease
{
    // Returns 0-1 for where in the cycle the phase is
    public static float Wave(WaveShape shape, double phase)
    {
        float x = (float)(phase - Math.Floor(phase));
        return shape switch {
            WaveShape.Sine => (MathF.Sin(MathF.Tau * x) + 1f) * 0.5f,
            WaveShape.Triangle => 1f - MathF.Abs(2f * x - 1f),
            WaveShape.Sawtooth => x,
            WaveShape.Square => x < 0.5f ? 0f : 1f,
            WaveShape.Hold => x < 0.25f ? x * 4f : x < 0.5f ? 1f : x < 0.75f ? 1f - (x - 0.5f) * 4f : 0f,
            _ => x
        };
    }

    public static float Apply(Easing easing, float x)
    {
        switch (easing)
        {
            case Easing.Smoothstep:
                return x * x * (3f - 2f * x);
            case Easing.InSine:
                return 1f - MathF.Cos(x * MathF.PI / 2f);
            case Easing.OutSine:
                return MathF.Sin(x * MathF.PI / 2f);
            case Easing.InOutSine:
                return -(MathF.Cos(MathF.PI * x) - 1f) / 2f;
            case Easing.InQuad:
                return x * x;
            case Easing.OutQuad:
                return 1f - (1f - x) * (1f - x);
            case Easing.InOutQuad:
                return x < 0.5f ? 2f * x * x : 1f - MathF.Pow(-2f * x + 2f, 2f) / 2f;
            case Easing.InCubic:
                return x * x * x;
            case Easing.OutCubic:
                return 1f - MathF.Pow(1f - x, 3f);
            case Easing.InOutCubic:
                return x < 0.5f ? 4f * x * x * x : 1f - MathF.Pow(-2f * x + 2f, 3f) / 2f;
            case Easing.OutElastic:
                if (x <= 0f) return 0f;
                if (x >= 1f) return 1f;
                return MathF.Pow(2f, -10f * x) * MathF.Sin((x * 10f - 0.75f) * (MathF.Tau / 3f)) + 1f;
            case Easing.OutBounce:
                const float n1 = 7.5625f;
                const float d1 = 2.75f;
                if (x < 1f / d1) return n1 * x * x;
                if (x < 2f / d1) return n1 * (x -= 1.5f / d1) * x + 0.75f;
                if (x < 2.5f / d1) return n1 * (x -= 2.25f / d1) * x + 0.9375f;
                return n1 * (x -= 2.625f / d1) * x + 0.984375f;
            default:
                return x;
        }
    }
}

public static class TextRenderer
{
    public static void Text(string text, in TextEffect fx)
    {
        Vector2 textSize = ImGui.CalcTextSize(text);
        float pad = VerticalPadding(fx);
        Vector2 startPos = ImGui.GetCursorScreenPos();

        // reserve exactly the space the text (plus any bounce/shake) uses
        ImGui.Dummy(new Vector2(textSize.X, textSize.Y + pad * 2f));
        Draw(ImGui.GetWindowDrawList(), new Vector2(startPos.X, startPos.Y + pad), text, fx);
    }

    // id defaults to the text, pass one when two selectables share the same text
    public static bool Selectable(string text, bool selected, in TextEffect fx, ImGuiSelectableFlags flags = ImGuiSelectableFlags.None, string? id = null)
    {
        Vector2 textSize = ImGui.CalcTextSize(text);
        float pad = VerticalPadding(fx);
        Vector2 startPos = ImGui.GetCursorScreenPos();
        float width = fx.FillWidth ? ImGui.GetContentRegionAvail().X : textSize.X;

        ImGui.PushID(id ?? text);
        bool pressed = ImGui.Selectable("##TextRendererSelectable", selected, flags, new Vector2(width, textSize.Y + 2f + pad * 2f));
        ImGui.PopID();

        ImDrawListPtr drawList = ImGui.GetWindowDrawList();
        if (fx.HighlightColour is Vector4 highlight && (ImGui.IsItemHovered() || selected))
        {
            drawList.AddRectFilled(startPos, new Vector2(startPos.X + width, startPos.Y + textSize.Y + pad * 2f), ImGui.ColorConvertFloat4ToU32(highlight));
        }

        Draw(drawList, new Vector2(startPos.X, startPos.Y + pad), text, fx);
        return pressed;
    }

    static float VerticalPadding(in TextEffect fx) => MathF.Abs(fx.BounceHeight) + MathF.Abs(fx.Shake);

    static void Draw(ImDrawListPtr drawList, Vector2 origin, string text, in TextEffect fx)
    {
        double t = ImGui.GetTime();
        float direction = fx.Reverse ? -1f : 1f;
        uint outsideColour = ImGui.ColorConvertFloat4ToU32(fx.OutsideColour ?? ImGui.GetStyle().Colors[(int)ImGuiCol.Text]);
        Vector4 wholeColour = fx.PerCharacter ? default : ColourAt(fx, t * fx.Speed + fx.PhaseOffset);

        // running x instead of measuring every substring
        float x = 0f;
        for (int i = 0; i < text.Length; i++)
        {
            string ch = text[i].ToString();
            Vector2 pos = new Vector2(origin.X + x, origin.Y);
            x += ImGui.CalcTextSize(ch).X;

            if (i < fx.RangeStart || i >= fx.RangeEnd)
            {
                drawList.AddText(pos, outsideColour, ch);
                continue;
            }

            double charOffset = direction * i;
            Vector4 colour = fx.PerCharacter ? ColourAt(fx, t * fx.Speed - charOffset * fx.Spread + fx.PhaseOffset) : wholeColour;

            if (fx.AlphaPulse != 0f)
            {
                float pulse = (float)(Math.Sin(MathF.Tau * (t * fx.AlphaPulseSpeed - charOffset * fx.Spread)) + 1.0) * 0.5f;
                colour.W *= 1f - fx.AlphaPulse * (1f - pulse);
            }

            if (fx.BounceHeight != 0f)
            {
                pos.Y -= (float)Math.Sin(MathF.Tau * (t * fx.BounceSpeed - charOffset * fx.BounceSpread)) * fx.BounceHeight;
            }

            if (fx.Shake != 0f)
            {
                long step = (long)(t * fx.ShakeSpeed);
                pos.X += Noise(step, i, 0) * fx.Shake;
                pos.Y += Noise(step, i, 1) * fx.Shake;
            }

            drawList.AddText(pos, ImGui.ColorConvertFloat4ToU32(colour), ch);
        }
    }

    static Vector4 ColourAt(in TextEffect fx, double phase)
    {
        float f = Ease.Apply(fx.Easing, Ease.Wave(fx.Wave, phase));
        Vector4 colour;
        switch (fx.Source)
        {
            case ColourSource.Rainbow:
                colour = CustomImguiTypes.HSVtoRGB(f, fx.Saturation, fx.Value);
                break;
            case ColourSource.RainbowSine:
                float angle = MathF.Tau * f;
                colour = new Vector4(
                    fx.Brightness + fx.Amplitude * MathF.Sin(angle),
                    fx.Brightness + fx.Amplitude * MathF.Sin(angle + MathF.Tau / 3f), // +120°
                    fx.Brightness + fx.Amplitude * MathF.Sin(angle + MathF.Tau * 2f / 3f), // +240°
                    1f);
                break;
            case ColourSource.Gradient:
                colour = SampleStops(fx.Stops, f, fx.Blend);
                break;
            default:
                colour = fx.Stops.Length > 0 ? fx.Stops[0] : Vector4.One;
                break;
        }
        colour = Vector4.Clamp(colour, Vector4.Zero, Vector4.One);
        colour.W *= fx.Alpha;
        return colour;
    }

    static Vector4 SampleStops(Vector4[] stops, float f, BlendSpace blend)
    {
        if (stops.Length == 0)
            return Vector4.One;
        if (stops.Length == 1)
            return stops[0];

        // easings like OutElastic overshoot, keep it on the gradient
        f = Math.Clamp(f, 0f, 1f);
        float pos = f * (stops.Length - 1);
        int index = Math.Min((int)pos, stops.Length - 2);
        float local = pos - index;

        return blend == BlendSpace.Hsv
            ? LerpHsv(stops[index], stops[index + 1], local)
            : Vector4.Lerp(stops[index], stops[index + 1], local);
    }

    static Vector4 LerpHsv(Vector4 a, Vector4 b, float t)
    {
        Vector4 hsvA = CustomImguiTypes.RGBtoHSV(a);
        Vector4 hsvB = CustomImguiTypes.RGBtoHSV(b);

        // greys have no hue, borrow the other colour's so it doesn't swing through red
        if (hsvA.Y == 0f) hsvA.X = hsvB.X;
        if (hsvB.Y == 0f) hsvB.X = hsvA.X;

        float hueDelta = hsvB.X - hsvA.X;
        if (hueDelta > 0.5f) hueDelta -= 1f;
        else if (hueDelta < -0.5f) hueDelta += 1f;

        return CustomImguiTypes.HSVtoRGB(
            hsvA.X + hueDelta * t,
            hsvA.Y + (hsvB.Y - hsvA.Y) * t,
            hsvA.Z + (hsvB.Z - hsvA.Z) * t,
            hsvA.W + (hsvB.W - hsvA.W) * t);
    }

    // Deterministic -1 to 1 per step/char/axis so shake holds still between steps
    static float Noise(long step, int index, int axis)
    {
        uint h = (uint)(step * 73856093) ^ (uint)(index * 19349663) ^ (uint)(axis * 83492791);
        h ^= h >> 13;
        h *= 0x5bd1e995;
        h ^= h >> 15;
        return (h & 0xFFFF) / 32767.5f - 1f;
    }
}
