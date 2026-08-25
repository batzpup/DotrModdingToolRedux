using ImGuiNET;
namespace DotrModdingTool2IMGUI;

public static class ImGuiExt
{
    public static bool InputUShort(string label, ref ushort value, int min = ushort.MinValue, int max = ushort.MaxValue, int step = 0)
    {
        int temp = value;

        bool changed = ImGui.InputInt(label, ref temp,step);

        if (changed)
        {
            temp = Math.Clamp(temp, min, max);
            value = (ushort)temp;
        }

        return changed;
    }

    public static bool InputByte(string label, ref byte value, int min = byte.MinValue, int max = byte.MaxValue, int step = 0)
    {
        int temp = value;

        bool changed = ImGui.InputInt(label, ref temp, step);

        if (changed)
        {
            temp = Math.Clamp(temp, min, max);
            value = (byte)temp;
        }

        return changed;
    }
}