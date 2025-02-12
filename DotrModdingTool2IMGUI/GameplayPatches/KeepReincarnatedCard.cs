namespace GameplayPatches {
public class KeepReincarnatedCard : Patch {
    static int patchLocation = 0x189f1c;

    public override bool IsApplied() {
        return dataAccess.CheckIfPatchApplied(patchLocation, new byte[4] { 0x00, 0x00, 0x00, 0x00 });
    }

    protected override void Apply() {
        dataAccess.NopInstructions(patchLocation, 1);
    }

    protected override void Remove() {
        dataAccess.ApplyPatch(patchLocation, new byte[4] { 0x24, 0x59, 0x08, 0x0c });
    }
}
}
