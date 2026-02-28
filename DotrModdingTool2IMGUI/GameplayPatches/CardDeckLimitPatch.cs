using DotrModdingTool2IMGUI;
namespace GameplayPatches;

public class CardDeckLimitPatch : Patch
{
    //00215628 03  00  e5  28    slti       deckNo ,cardLimit ,0x3
    //00215628 09  00  e5  28    slti       deckNo ,cardLimit ,0x3
    public static int patchLocation = 0x215628 - DataAccess.IsoSlusRamOffset;

    public override bool IsApplied()
    {
        return !dataAccess.CheckIfPatchApplied(patchLocation, new byte[] { 0x03, 0x00, 0xe5, 0x28 });
    }

      public void ApplyOrRemove(bool apply, int value)
    {
        if (apply)
        {
            Apply(value);
        }
        else
        {
            Remove();
        }
    }

    protected void Apply(int maxCard)
    {
        dataAccess.ApplyPatch(patchLocation, new byte[] { (byte)maxCard, 0x00, 0xe5, 0x28 });
    }

    protected override void Remove()
    {
        dataAccess.ApplyPatch(patchLocation, new byte[] { 0x03, 0x00, 0xe5, 0x28 });
    }
}