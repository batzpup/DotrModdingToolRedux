using DotrModdingTool2IMGUI;
namespace GameplayPatches {
public class EmulatorCameraFix : Patch {
    static int EmulatorCameraFix1 =    0x1D9328 - DataAccess.IsoSlusRamOffset;
    static int EmulatorCameraFix2 =    0x1D2B78 - DataAccess.IsoSlusRamOffset;
    static int EmulatorCameraFix3 =    0x1D2B80 - DataAccess.IsoSlusRamOffset;
    static int EmulatorCameraSpyFix1 = 0x255DDC - DataAccess.IsoSlusRamOffset;
    static int EmulatorCameraSpyFix2 = 0x255DFC - DataAccess.IsoSlusRamOffset;

    public override bool IsApplied() {
        return dataAccess.CheckIfPatchApplied(EmulatorCameraFix1, new byte[4] { 0x00, 0x38, 0x02, 0x24 });
    }

    protected override void Apply() {
        dataAccess.ApplyPatch(EmulatorCameraFix1, new byte[4] { 0x00, 0x38, 0x02, 0x24 });
        dataAccess.ApplyPatch(EmulatorCameraFix2, new byte[4] { 0x00, 0x38, 0x41, 0x28 });
        dataAccess.ApplyPatch(EmulatorCameraFix3, new byte[4] { 0x00, 0x38, 0x03, 0x24 });
        dataAccess.ApplyPatch(EmulatorCameraSpyFix1, new byte[4] { 0x00, 0x38, 0x03, 0x24 });
        dataAccess.ApplyPatch(EmulatorCameraSpyFix2, new byte[4] { 0x00, 0x38, 0x03, 0x24 });
    }

    protected override void Remove() {
        dataAccess.ApplyPatch(EmulatorCameraFix1, new byte[4] { 0x00, 0x40, 0x02, 0x24 });
        dataAccess.ApplyPatch(EmulatorCameraFix2, new byte[4] { 0x00, 0x40, 0x41, 0x28 });
        dataAccess.ApplyPatch(EmulatorCameraFix3, new byte[4] { 0x00, 0x40, 0x03, 0x24 });
        dataAccess.ApplyPatch(EmulatorCameraSpyFix1, new byte[4] { 0x00, 0x40, 0x03, 0x24 });
        dataAccess.ApplyPatch(EmulatorCameraSpyFix2, new byte[4] { 0x00, 0x40, 0x03, 0x24 });
    }
}
}
