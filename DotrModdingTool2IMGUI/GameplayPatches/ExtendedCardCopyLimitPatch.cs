namespace GameplayPatches;

public class ExtendedCardCopyLimitPatch: Patch
{
    //00215628 03  00  e5  28    slti       deckNo ,cardLimit ,0x3
    //00215628 09  00  e5  28    slti       deckNo ,cardLimit ,0x3
    static int patchLocation = 0x1e5728;
    public override bool IsApplied()
    {
        return dataAccess.CheckIfPatchApplied(patchLocation, new byte[] {0x09,0x00,0xe5,0x28 });
    }

    protected override void Apply()
    {
        dataAccess.ApplyPatch(patchLocation, new byte[] {0x09,0x00,0xe5,0x28 });
    }

    protected override void Remove()
    {
        dataAccess.ApplyPatch(patchLocation, new byte[] {0x03,0x00,0xe5,0x28});
    }
}