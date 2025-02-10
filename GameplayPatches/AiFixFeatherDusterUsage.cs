﻿using GameplayPatches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameplayPatches
{
    public class FixFeatherDusterUsage : Patch
    {
        static int patchLocation = 0x11B4fc;
        

        public override bool IsApplied()
        {
            return dataAccess.CheckIfPatchApplied(patchLocation, new byte[8] {0x08, 0x80, 0xC0, 0x70, 0x01, 0x00, 0x07, 0x24 });
        }

        protected override void Apply()
        {
            dataAccess.ApplyPatch(patchLocation, new byte[136] { 0x08, 0x80, 0xC0, 0x70, 0x01, 0x00, 0x07, 0x24, 0x28, 0x16, 0x00, 0x70, 0x01, 0x00, 0x07, 0x24, 0x1E, 0x00, 0x08, 0x24, 0x12, 0x00, 0x09, 0x24, 0x28, 0x26, 0x20, 0x72, 0x28, 0x2E, 0x00, 0x72, 0x90, 0x32, 0x09, 0x0C, 0x28, 0x36, 0x40, 0x72, 0x25, 0x20, 0x40, 0x00, 0x44, 0x3A, 0x09, 0x0C, 0x00, 0x00, 0x00, 0x00, 0x03, 0x00, 0x42, 0x28, 0x0B, 0x00, 0x40, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x25, 0x20, 0x20, 0x02, 0x25, 0x28, 0x00, 0x02, 0x25, 0x30, 0x40, 0x02, 0x90, 0x32, 0x09, 0x0C, 0x01, 0x00, 0xC6, 0x38, 0x05, 0x00, 0x40, 0x14, 0x01, 0x00, 0x02, 0x24, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x08, 0x10, 0x00, 0x70, 0x00, 0x00, 0x00, 0x00, 0x30, 0x00, 0xBF, 0x7B, 0x20, 0x00, 0xB2, 0x7B, 0x10, 0x00, 0xB1, 0x7B, 0x00, 0x00, 0xB0, 0x7B, 0x08, 0x00, 0xE0, 0x03, 0x40, 0x00, 0xBD, 0x27 });

        }

        protected override void Remove()
        {
            dataAccess.ApplyPatch(patchLocation, new byte[136] { 0x28, 0x2B, 0x05, 0x0C, 0x28, 0x86, 0xC0, 0x70, 0xFF, 0x00, 0x42, 0x30, 0x04, 0x00, 0x40, 0x14, 0x01, 0x00, 0x07, 0x24, 0x16, 0x00, 0x00, 0x10, 0x28, 0x16, 0x00, 0x70, 0x01, 0x00, 0x07, 0x24, 0x1E, 0x00, 0x08, 0x24, 0x12, 0x00, 0x09, 0x24, 0x28, 0x26, 0x20, 0x72, 0x28, 0x2E, 0x00, 0x72, 0x90, 0x32, 0x09, 0x0C, 0x28, 0x36, 0x40, 0x72, 0x04, 0x00, 0x40, 0x10, 0x02, 0x00, 0x07, 0x24, 0x0B, 0x00, 0x00, 0x10, 0x28, 0x16, 0x00, 0x70, 0x02, 0x00, 0x07, 0x24, 0x1E, 0x00, 0x08, 0x24, 0x12, 0x00, 0x09, 0x24, 0x28, 0x26, 0x20, 0x72, 0x28, 0x2E, 0x00, 0x72, 0x90, 0x32, 0x09, 0x0C, 0x28, 0x36, 0x40, 0x72, 0x01, 0x00, 0x03, 0x24, 0x0A, 0x18, 0x02, 0x00, 0x28, 0x16, 0x60, 0x70, 0x30, 0x00, 0xBF, 0x7B, 0x20, 0x00, 0xB2, 0x7B, 0x10, 0x00, 0xB1, 0x7B, 0x00, 0x00, 0xB0, 0x7B, 0x08, 0x00, 0xE0, 0x03, 0x40, 0x00, 0xBD, 0x27 });
        }
    }
}
