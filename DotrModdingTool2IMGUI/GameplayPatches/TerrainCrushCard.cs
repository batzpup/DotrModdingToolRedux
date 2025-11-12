
using DotrModdingTool2IMGUI;
namespace GameplayPatches
{
    class TerrainCrushCard : Patch
    {
        static int cardEffectPatch = 0x2CC968 - DataAccess.IsoSlusRamOffset;
        static int cardParamPatch =  0x2CC96C - DataAccess.IsoSlusRamOffset;
        static int cardTypePatch =   0x2C2E88 - DataAccess.IsoSlusRamOffset;
        public override bool IsApplied()
        {
            return dataAccess.CheckIfPatchApplied(cardEffectPatch, new byte[4] { 0xfd, 0x02, 0x00, 0x00 });
        }

        protected override void Apply()
        {
            //Change Card Effect
            dataAccess.ApplyPatch(cardEffectPatch, new byte[4] { 0xfd, 0x02, 0x00, 0x00 });
            //Change Card Param
            dataAccess.ApplyPatch(cardParamPatch, new byte[4] { 0x09, 0x00, 0x02, 0x00 });

            //Change Card Type
            dataAccess.ApplyPatch(cardTypePatch, new byte[1] { 0x20 });
        }

        protected override void Remove()
        {
            //Change Card Effect
            dataAccess.ApplyPatch(cardEffectPatch, new byte[4] { 0x00, 0x00, 0x00, 0x00 });
            //Change Card Param
            dataAccess.ApplyPatch(cardParamPatch, new byte[4] { 0x00, 0x00, 0x00, 0x00 });

            //Change Card Type
            dataAccess.ApplyPatch(cardTypePatch, new byte[1] { 0x40 });
        }
    }
}
