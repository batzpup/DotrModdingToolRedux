namespace GameplayPatches {
public class AIFasterTurnPassing : Patch {
    static int patchLocation = 0x246400;
    static int TaTutoSetDeck35 = 0x148420;

    public override bool IsApplied() {
        return dataAccess.CheckIfPatchApplied(patchLocation, new byte[8] { 0xc8, 0xe0, 0x05, 0x08, 0x00, 0x00, 0x00, 0x00 });
    }

    protected override void Apply() {
        dataAccess.ApplyPatch(patchLocation, new byte[8] { 0xc8, 0xe0, 0x05, 0x08, 0x00, 0x00, 0x00, 0x00 });

        // Overwrites nopped bytes.
        dataAccess.ApplyPatch(TaTutoSetDeck35, new byte[156] { 0x02, 0x00, 0x03, 0x92, 0x06, 0x00, 0x0b, 0x24, 0x21, 0x00, 0x6b, 0x14, 0x00, 0x00, 0x00, 0x00, 0xff, 0x00, 0x4c, 0x30, 0x04, 0x00, 0x0d, 0x96, 0xff, 0x00, 0xb8, 0x31, 0x23, 0x70, 0x98, 0x01, 0x1b, 0x00, 0xc0, 0x15, 0x00, 0x00, 0x00, 0x00, 0x03, 0x62, 0x02, 0x00, 0x03, 0xc2, 0x0d, 0x00, 0x22, 0x70, 0x98, 0x01, 0x16, 0x00, 0xc0, 0x15, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x0c, 0x00, 0x0d, 0x8e, 0x01, 0x00, 0x09, 0x24, 0x0f, 0x00, 0xa9, 0x11, 0x02, 0x00, 0x09, 0x24, 0x10, 0x00, 0x0d, 0x92, 0x0c, 0x00, 0xa9, 0x15, 0x00, 0x00, 0x00, 0x00, 0x50, 0x7d, 0x07, 0x0c, 0x00, 0x00, 0x00, 0x00, 0x57, 0x00, 0x4d, 0x80, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0xad, 0x35, 0x57, 0x00, 0x4d, 0xa0, 0x60, 0x01, 0x28, 0x26, 0x20, 0x00, 0x09, 0x24, 0x00, 0x00, 0x09, 0xa5, 0xc1, 0xda, 0x09, 0x08, 0x00, 0x00, 0x00, 0x00, 0xff, 0xff, 0x8a, 0x32, 0xff, 0x00, 0x83, 0x32, 0xc2, 0xd8, 0x09, 0x08 });
    }

    protected override void Remove() {
        // Restoring Nopped bytes is done elsewhere
        dataAccess.ApplyPatch(patchLocation, new byte[8] { 0xff, 0xff, 0x8a, 0x32, 0xff, 0x00, 0x83, 0x32 });
    }
}
}
