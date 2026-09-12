using DotrModdingTool2IMGUI;
namespace GameplayPatches {
    // Holding Cross while confirming an attack inverts the saved Monster Battle
    // setting (show battle <-> abbreviate) for that battle only; the saved menu
    // option is never modified. Checks both controller ports, so either local
    // player can trigger it. Does not distinguish which player's saved setting
    // should apply in a 2-human game - it always reads player 1's copy.
    public class InvertMonsterBattleWithButtonPress : Patch {
        // Monster Battle presentation wrapper; replaced with a jump into CaveLocation.
        static int WrapperLocation = 0x001DE2A0 - DataAccess.IsoSlusRamOffset;
        // Free alignment padding (13 nop words) between two unrelated functions.
        static int CaveLocation = 0x0017B48C - DataAccess.IsoSlusRamOffset;

        static byte[] WrapperOriginal = { 0x01, 0x00, 0x05, 0x24, 0x40, 0x53, 0x08, 0x08, 0x28, 0x26, 0x00, 0x70 };
        static byte[] WrapperPatched = { 0x23, 0xed, 0x05, 0x08, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 };

        static byte[] CaveOriginal = new byte[52];

        // lui   at, 0x0035
        // lbu   v1, -0x934(at)   ; saveData[1].flags
        // lui   at, 0x0032
        // lhu   t0, 0x5BC0(at)   ; port 0 input word
        // lhu   t1, 0x5C1C(at)   ; port 1 input word
        // or    t0, t0, t1       ; combine before masking
        // andi  v1, v1, 0x0008
        // andi  t0, t0, 0x0040
        // sltu  v1, zero, v1
        // sltu  t0, zero, t0
        // xor   v0, v1, t0
        // jr    ra
        // nop
        static byte[] CavePatched = {
            0x35, 0x00, 0x01, 0x3c, 0xcc, 0xf6, 0x23, 0x90, 0x32, 0x00, 0x01, 0x3c, 0xc0, 0x5b, 0x28, 0x94,
            0x1c, 0x5c, 0x29, 0x94, 0x25, 0x40, 0x09, 0x01, 0x08, 0x00, 0x63, 0x30, 0x40, 0x00, 0x08, 0x31,
            0x2b, 0x18, 0x03, 0x00, 0x2b, 0x40, 0x08, 0x00, 0x26, 0x10, 0x68, 0x00, 0x08, 0x00, 0xe0, 0x03,
            0x00, 0x00, 0x00, 0x00
        };

        public override bool IsApplied() {
            return dataAccess.CheckIfPatchApplied(WrapperLocation, WrapperPatched);
        }

        protected override void Apply() {
            dataAccess.ApplyPatch(CaveLocation, CavePatched);
            dataAccess.ApplyPatch(WrapperLocation, WrapperPatched);
        }

        protected override void Remove() {
            dataAccess.ApplyPatch(WrapperLocation, WrapperOriginal);
            dataAccess.ApplyPatch(CaveLocation, CaveOriginal);
        }
    }
}
