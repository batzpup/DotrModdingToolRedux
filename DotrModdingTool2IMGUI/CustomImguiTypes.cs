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


    public static Vector4 RGBtoHSV(Vector4 rgb)
    {
        float max = Math.Max(rgb.X, Math.Max(rgb.Y, rgb.Z));
        float min = Math.Min(rgb.X, Math.Min(rgb.Y, rgb.Z));
        float delta = max - min;
        float h = 0f;
        if (delta > 0f)
        {
            if (max == rgb.X)
                h = (rgb.Y - rgb.Z) / delta;
            else if (max == rgb.Y)
                h = (rgb.Z - rgb.X) / delta + 2f;
            else
                h = (rgb.X - rgb.Y) / delta + 4f;
            h /= 6f;
            if (h < 0f)
                h += 1f;
        }
        float s = max > 0f ? delta / max : 0f;
        return new Vector4(h, s, max, rgb.W);
    }
}
