﻿using GameplayPatches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameplayPatches
{
    public class AiDMKFixRevive : Patch
    {
        static int patchLocation = 0x13CA9C;
        

        public override bool IsApplied()
        {
            return dataAccess.CheckIfPatchApplied(patchLocation, new byte[8] { 0xFF, 0x00, 0x25, 0x32, 0x08, 0x98, 0x40, 0x70 });
        }

        protected override void Apply()
        {
            dataAccess.ApplyPatch(patchLocation, new byte[80] { 0xFF, 0x00, 0x25, 0x32, 0x08, 0x98, 0x40, 0x70, 0x80, 0x7B, 0x07, 0x0C, 0x08, 0x20, 0x40, 0x72, 0x08, 0x80, 0x40, 0x70, 0x38, 0xBF, 0x09, 0x0C, 0x08, 0x20, 0x00, 0x72, 0x43, 0x11, 0x02, 0x00, 0x72, 0x00, 0x40, 0x14, 0xFF, 0xFF, 0x02, 0x24, 0xFF, 0xFF, 0x62, 0x32, 0xC4, 0x09, 0x42, 0x28, 0x6D, 0x00, 0x40, 0x14, 0x00, 0x00, 0x00, 0x00, 0xFF, 0xFF, 0x22, 0x32, 0x00, 0x94, 0x12, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x67, 0x00, 0x00, 0x10 });

        }

        protected override void Remove()
        {
            dataAccess.ApplyPatch(patchLocation, new byte[80] { 0x0F, 0x00, 0x25, 0x32, 0x80, 0x7B, 0x07, 0x0C, 0x28, 0x26, 0x40, 0x72, 0x28, 0x86, 0x40, 0x70, 0x38, 0xBF, 0x09, 0x0C, 0x28, 0x26, 0x00, 0x72, 0x43, 0x11, 0x02, 0x00, 0x73, 0x00, 0x40, 0x14, 0xFF, 0xFF, 0x02, 0x24, 0x80, 0x00, 0xA4, 0x27, 0x5C, 0x73, 0x07, 0x0C, 0x28, 0x2E, 0x00, 0x72, 0x80, 0x00, 0xA4, 0x27, 0xBC, 0x80, 0x07, 0x0C, 0x28, 0x2E, 0x00, 0x70, 0xC4, 0x09, 0x42, 0x28, 0x69, 0x00, 0x40, 0x14, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x22, 0x32, 0x67, 0x00, 0x00, 0x10 });
        }
    }
}
