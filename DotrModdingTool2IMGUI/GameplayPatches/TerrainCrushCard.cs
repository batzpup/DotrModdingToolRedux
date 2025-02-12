

namespace GameplayPatches
{
    internal class TerrainCrushCard : Patch
    {
        static int cardEffectPatch = 0x29ca68;
        static int cardParamPatch = 0x29ca6c;
        static int cardTypePatch = 0x292f88;
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
