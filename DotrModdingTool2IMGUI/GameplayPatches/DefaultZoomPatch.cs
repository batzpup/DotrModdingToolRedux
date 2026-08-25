using DotrModdingTool2IMGUI;
namespace GameplayPatches;

public class DefaultZoomPatch : Patch
{
    public static int Zoom0Location = 0x001ae0b0 - DataAccess.IsoSlusRamOffset;
    static int Zoom1Location = 0x001ae154 - DataAccess.IsoSlusRamOffset;
    static int customResourceZoom1Location = 0x001ae130 - DataAccess.IsoSlusRamOffset;

    public override bool IsApplied()
    {
        return !dataAccess.CheckIfPatchApplied(Zoom0Location, new byte[4] { 0xC8, 0x00, 0x02, 0x24 });
    }

    public void ApplyOrRemove(bool apply, int value)
    {
        if (apply)
        {
            Apply((ushort)value);
        }
        else
        {
            Remove();
        }
    }

    protected void Apply(ushort zoom)
    {
        byte[] value = BitConverter.GetBytes(zoom);
        dataAccess.ApplyPatch(Zoom0Location, new byte[4] { value[0], value[1], 0x02, 0x24 });
        if (GameplayPatchesWindow.Instance.bCustomResources)
        {
            dataAccess.ApplyPatch(customResourceZoom1Location, new byte[4] { value[0], value[1], 0x03, 0x24 });
        }
        else
        {
            dataAccess.ApplyPatch(Zoom1Location, new byte[4] { value[0], value[1], 0x03, 0x24 });
        }


    }

    protected override void Remove()
    {
        dataAccess.ApplyPatch(Zoom0Location, new byte[4] { 0xC8, 0x00, 0x02, 0x24 });
        if (new CustomDuelResources().IsApplied())
        {
            dataAccess.ApplyPatch(customResourceZoom1Location, new byte[4] { 0xC8, 0x00, 0x03, 0x24 });
        }
        else
        {
            dataAccess.ApplyPatch(Zoom1Location, new byte[4] { 0xC8, 0x00, 0x03, 0x24 });
        }


    }
}