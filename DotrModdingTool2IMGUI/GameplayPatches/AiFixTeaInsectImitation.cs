using GameplayPatches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameplayPatches
{
    public class AiFixTeaInsectImitation : Patch
    {
        static int patchLocation = 0x10F5D4;
        

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
