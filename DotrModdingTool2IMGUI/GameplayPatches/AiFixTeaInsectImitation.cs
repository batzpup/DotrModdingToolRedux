
using DotrModdingTool2IMGUI;

namespace GameplayPatches
{
    public class AiFixTeaInsectImitation : Patch
    {
        static int patchLocation = 0x13F4D4 - DataAccess.IsoSlusRamOffset;
        

        public override bool IsApplied()
        {
            return dataAccess.CheckIfPatchApplied(patchLocation, new byte[4] { 0x06, 0x00, 0x34, 0xA2 });
        }

        protected override void Apply()
        {
            dataAccess.ApplyPatch(patchLocation, new byte[4] { 0x06, 0x00, 0x34, 0xA2 });

        }

        protected override void Remove()
        {
            dataAccess.ApplyPatch(patchLocation, new byte[4] { 0x07, 0x00, 0x34, 0xA2 });
        }
    }
}
