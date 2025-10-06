namespace DotrModdingTool2IMGUI;

public class ModdedStringName
{
    public string Default { get; set; } // From the original ISO
    public string Edited { get; set; } // From the edited binary

    public string Current
    {
        get { return UserSettings.UseDefaultNames ? Default : Edited; }
    }

    public ModdedStringName(string def, string edt)
    {
        Default = def;
        Edited = edt;
    }

    public override string ToString()
    {
        return Current;
    }
}