using System.Drawing;
using System.Numerics;
using ImGuiNET;
namespace DotrModdingTool2IMGUI;

public struct GuiColour
{
    public Vector4 value;

    public GuiColour(int r, int g, int b, int a = 255)
    {
        r = Math.Clamp(r, 0, 255);
        g = Math.Clamp(g, 0, 255);
        b = Math.Clamp(b, 0, 255);
        a = Math.Clamp(a, 0, 255);

        value = new Vector4(r / 255.0f, g / 255.0f, b / 255.0f, a / 255.0f);

    }

    public GuiColour(float r, float g, float b, float a = 1.0f)
    {
        r = Math.Clamp(r, 0.0f, 1.0f);
        g = Math.Clamp(g, 0.0f, 1.0f);
        b = Math.Clamp(b, 0.0f, 1.0f);
        a = Math.Clamp(a, 0.0f, 1.0f);
        value = new Vector4(r, g, b, a);
    }

    public GuiColour(Color color)
    {
      value = new Vector4(
        Math.Clamp(color.R / 255f, 0f, 1f), 
        Math.Clamp(color.G / 255f, 0f, 1f), 
        Math.Clamp(color.B / 255f, 0f, 1f), 
        Math.Clamp(color.A / 255f, 0f, 1f));
    }

    public GuiColour(Vector4 vec)
    {
        vec.X = Math.Clamp(vec.X, 0.0f, 1.0f);
        vec.Y = Math.Clamp(vec.Y, 0.0f, 1.0f);
        vec.Z = Math.Clamp(vec.Z, 0.0f, 1.0f);
        vec.W = Math.Clamp(vec.W, 0.0f, 1.0f);
        value = vec;
    }
}