namespace GameplayPatches {
public class RemoveRNGFromSlots : Patch {
    static int patchLocation = 0x2212b4;

    public override bool IsApplied() {
        return dataAccess.CheckIfPatchApplied(patchLocation, new byte[4] { 0x01, 0x00, 0x60, 0xa2 });
    }

    protected override void Apply() {
        dataAccess.ApplyPatch(patchLocation, new byte[4] { 0x01, 0x00, 0x60, 0xa2 });
    }

    protected override void Remove() {
        dataAccess.ApplyPatch(patchLocation, new byte[4] { 0x01, 0x00, 0x62, 0xa2 });
    }
}
}
