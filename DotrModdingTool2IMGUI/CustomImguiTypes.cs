using System.Drawing;
using System.Numerics;
using ImGuiNET;
namespace DotrModdingTool2IMGUI;

public static class CustomImguiTypes
{
    //AI code to investigate and learn from

    public static Vector4 HSVtoRGB(float h, float s, float v, float a = 1.0f)
    {
        h = h - (float)Math.Floor(h); // wrap to [0,1)
        float r = 0, g = 0, b = 0;
        float i = (float)Math.Floor(h * 6f);
        float f = h * 6f - i;
        float p = v * (1f - s);
        float q = v * (1f - f * s);
        float t = v * (1f - (1f - f) * s);
        switch (((int)i) % 6)
        {
            case 0:
                r = v;
                g = t;
                b = p;
                break;
            case 1:
                r = q;
                g = v;
                b = p;
                break;
            case 2:
                r = p;
                g = v;
                b = t;
                break;
            case 3:
                r = p;
                g = q;
                b = v;
                break;
            case 4:
                r = t;
                g = p;
                b = v;
                break;
            case 5:
                r = v;
                g = p;
                b = q;
                break;
        }
        return new Vector4(r, g, b, a);
    }


    public static void RenderRainbowTextWhole(string text)
    {
        double hueOffset = 0.0; 
        float speed = 0.3f;
        float saturation = 0.9f;
        float value = 0.95f;
        float alpha = 1.0f;

        double t = ImGui.GetTime(); // seconds as double
        hueOffset = (t * speed) % 1.0; // speed cycles
        Vector4 color = HSVtoRGB((float)hueOffset, saturation, value, alpha);
        ImGui.TextColored(color, text);
    }

    //Smooth cycle
    public static void RenderRainbowTextPerChar(string text)
    {
        double hueOffset = 0.0; // persistent between frames
        float speed = 0.3f; // cycles per second
        float saturation = 0.9f;
        float value = 0.95f;
        float alpha = 1.0f;

        double t = ImGui.GetTime();
        float baseHue = (float)((t * speed) % 1.0);
        float huePerChar = 0.03f; // spacing between characters in hue
        for (int i = 0; i < text.Length; i++)
        {
            float h = baseHue - i * huePerChar;
            Vector4 col = HSVtoRGB(h, saturation, value, alpha);
            // Use ImGui.SameLine with no spacing so characters are adjacent
            ImGui.PushStyleColor(ImGuiCol.Text, col);
            ImGui.TextUnformatted(text[i].ToString());
            ImGui.PopStyleColor();
            if (i != text.Length - 1)
                ImGui.SameLine(0, 0);
        }
    }

    //breathing
    public static void RenderRainbowTextPerChar_Sine(string text)
    {
        double t = ImGui.GetTime(); // seconds
        const float speed = 3.0f; // overall animation speed
        const float spread = 0.4f; // hue spacing between characters
        const float brightness = 0.5f; // base brightness
        const float amplitude = 0.8f; // how intense the colors swing (0.5 = full)
        const float alpha = 1.0f;

        for (int i = 0; i < text.Length; i++)
        {
            // phase advances per character and over time
            float phase = (float)(t * speed - i * spread);

            // 120° phase offsets (2π/3 radians) between RGB components
            float r = brightness + amplitude * (float)Math.Sin(phase + 0.0f);
            float g = brightness + amplitude * (float)Math.Sin(phase + 2.094f); // +120°
            float b = brightness + amplitude * (float)Math.Sin(phase + 4.188f); // +240°

            Vector4 color = new Vector4(r, g, b, alpha);

            ImGui.PushStyleColor(ImGuiCol.Text, color);
            ImGui.TextUnformatted(text[i].ToString());
            ImGui.PopStyleColor();

            if (i != text.Length - 1)
                ImGui.SameLine(0, 0);
        }
    }

    public static void RenderWavyRainbowText(string text)
    {
        double t = ImGui.GetTime(); // seconds since start

        float speed = 1f; // animation speed
        float spread = 0.4f; // how far apart color phases are per char
        float amplitude = 0.6f; // color amplitude
        float brightness = 0.6f; // base brightness
        float waveHeight = 3.0f; // how high the letters bounce (in pixels)
        float waveSpeed = 10.0f; // speed of the vertical wave motion
        float alpha = 1.0f;

        ImDrawListPtr drawList = ImGui.GetWindowDrawList();
        Vector2 startPos = ImGui.GetCursorScreenPos();

        // measure text width to avoid layout conflicts
        ImGui.Dummy(new Vector2(ImGui.CalcTextSize(text).X, waveHeight * 2f));

        for (int i = 0; i < text.Length; i++)
        {
            float phase = (float)(t * speed - i * spread);

            // --- rainbow color (sinusoidal) ---
            float r = brightness + amplitude * (float)Math.Sin(phase + 0.0f);
            float g = brightness + amplitude * (float)Math.Sin(phase + 2.094f); // +120°
            float b = brightness + amplitude * (float)Math.Sin(phase + 4.188f); // +240°
            Vector4 color = new Vector4(r, g, b, alpha);

            // --- vertical wave offset ---
            float yOffset = (float)Math.Sin(phase * waveSpeed) * waveHeight;

            // measure character size
            string ch = text[i].ToString();
            Vector2 charSize = ImGui.CalcTextSize(ch);

            // compute position of each char manually
            Vector2 pos = new Vector2(
                startPos.X + ImGui.CalcTextSize(text.Substring(0, i)).X,
                startPos.Y + waveHeight - yOffset
            );

            drawList.AddText(pos, ImGui.ColorConvertFloat4ToU32(color), ch);
        }
        // after manual drawing, move cursor down a bit
        ImGui.Dummy(new Vector2(0, 25f));
    }

    public static bool RenderGradientSelectable(string text, bool selected, Vector4 colour1, Vector4 colour2, ImGuiSelectableFlags flags = ImGuiSelectableFlags.None, int startingCharIndex = 0, int endingCharIndex = 0)
    {
        ImDrawListPtr drawList = ImGui.GetWindowDrawList();
        Vector2 startPos = ImGui.GetCursorScreenPos();
        Vector2 textSize = ImGui.CalcTextSize(text);
        Vector2 selectableSize = new Vector2(ImGui.GetContentRegionAvail().X, textSize.Y + 2);
        
        bool pressed = ImGui.Selectable("##hiddenSelectable" + text, ref selected, flags, selectableSize);
        
        double t = ImGui.GetTime();
        const float speed = 2.0f;
        const float spread = 0.4f;


        // Draw the rainbow text manually
        for (int i = 0; i < text.Length; i++)
        {
            float phase = (float)((Math.Sin(t * speed - i * spread)) + 0.5);
            Vector4 col = Vector4.Lerp(colour1, colour2, phase);
            Vector2 charPos = new Vector2(startPos.X + ImGui.CalcTextSize(text.Substring(0, i)).X, startPos.Y);
            uint colorU32 = ImGui.ColorConvertFloat4ToU32(col);
            
            if (i >= startingCharIndex & i < endingCharIndex)
            {
                drawList.AddText(charPos, colorU32, text[i].ToString());
            }
            else
            {
                 drawList.AddText(charPos, ImGui.ColorConvertFloat4ToU32(new GuiColour(Color.White).value), text[i].ToString());
            }

        }
        return pressed;
    }


    public static bool RenderRainbowSelectable(string text, bool selected, ImGuiSelectableFlags flags = ImGuiSelectableFlags.None)
    {
        ImDrawListPtr drawList = ImGui.GetWindowDrawList();
        Vector2 startPos = ImGui.GetCursorScreenPos();
        Vector2 textSize = ImGui.CalcTextSize(text);
        Vector2 selectableSize = new Vector2(textSize.X, textSize.Y + 2);

        // Create an invisible selectable
        bool pressed = ImGui.Selectable("##hiddenSelectable" + text, ref selected, flags, selectableSize);

        // Compute time-based rainbow
        double t = ImGui.GetTime();
        const float speed = 3.0f;
        const float spread = 0.4f;
        const float brightness = 0.5f;
        const float amplitude = 0.5f;
        const float alpha = 1.0f;

        Vector4 purple = new Vector4(0.6f, 0.2f, 0.9f, 1.0f); // purple
        Vector4 blue = new Vector4(0.39f, 0.58f, 0.93f, 1.0f); // cornflower blue
        // Draw the rainbow text manually
        for (int i = 0; i < text.Length; i++)
        {
            float phase = (float)(t * speed - i * spread);

            float r = brightness + amplitude * (float)Math.Sin(phase + 0.0f);
            float g = brightness + amplitude * (float)Math.Sin(phase + 2.094f);
            float b = brightness + amplitude * (float)Math.Sin(phase + 4.188f);
            Vector4 col = new Vector4(r, g, b, alpha);

            Vector2 charPos = new Vector2(
                startPos.X + ImGui.CalcTextSize(text.Substring(0, i)).X,
                startPos.Y
            );

            uint colorU32 = ImGui.ColorConvertFloat4ToU32(col);
            drawList.AddText(charPos, colorU32, text[i].ToString());
        }

        // Optionally draw highlight/underline when hovered or selected
        if (ImGui.IsItemHovered() || selected)
        {
            uint hlColor = ImGui.ColorConvertFloat4ToU32(new Vector4(1, 1, 1, 0.15f));
            drawList.AddRectFilled(
                startPos,
                new Vector2(startPos.X + textSize.X, startPos.Y + textSize.Y),
                hlColor
            );
        }

        return pressed;
    }
}