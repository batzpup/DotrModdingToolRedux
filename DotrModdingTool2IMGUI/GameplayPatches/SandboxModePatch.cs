using DotrModdingTool2IMGUI;
namespace GameplayPatches

{
    public class SandboxModePatch : Patch
    {
        static int patchLocation = 0x0021c580 - DataAccess.IsoSlusRamOffset;
        
        public override bool IsApplied()
        {
            return dataAccess.CheckIfPatchApplied(patchLocation, new byte[4] { 0x04, 0x00, 0x00, 0x10 });
        }

        protected override void Apply()
        {
           dataAccess.ApplyPatch(patchLocation, new byte[4] { 0x04, 0x00, 0x00, 0x10 });
        }

        protected override void Remove()
        {
             dataAccess.ApplyPatch(patchLocation, new byte[4] { 0x09, 0x00, 0x40, 0x10 });
        }
    }
}