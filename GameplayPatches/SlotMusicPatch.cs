namespace GameplayPatches;

public class SlotMusicPatch : Patch
{
    public static int SlotTrackPtr = 0x17ae34;

    public override bool IsApplied()
    {
        return !dataAccess.CheckIfPatchApplied(SlotTrackPtr, new byte[4] { 0x21, 0x00, 0x04, 0x24 });
    }

    public void Apply(byte trackNumber)
    {
        dataAccess.ApplyPatch(SlotTrackPtr, new byte[4] { trackNumber, 0x00, 0x04, 0x24 });
    }

    protected override void Remove()
    {
        dataAccess.ApplyPatch(SlotTrackPtr, new byte[4] { 0x21, 0x00, 0x04, 0x24 });
    }
}