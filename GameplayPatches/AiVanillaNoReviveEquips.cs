﻿using GameplayPatches;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameplayPatches
{
    public class AiVanillaNoReviveEquips : Patch
    {
        static int MFL_SK_Part1 = 0x13705C;
        static int MFL_SK_Part2 = 0x1370F4;
        static int TaTuto_ControlFade = 0x24F990;
        static int DMK_Part1 = 0x13CB54;
        static int DMK_Part2 = 0x13CBF8;


        public override bool IsApplied()
        {
            return dataAccess.CheckIfPatchApplied(MFL_SK_Part1, new byte[8] { 0x24, 0xFE, 0x09, 0x08, 0x00, 0x00, 0x00, 0x00 });
        }

        protected override void Apply()
        {
            // Actual fix for all affected characters (MFL_SK & DMK)
            dataAccess.ApplyPatch(TaTuto_ControlFade, new byte[144] { 0x06, 0x00, 0x40, 0x50, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x1E, 0x24, 0x03, 0x00, 0x5E, 0x50, 0x00, 0x00, 0x00, 0x00, 0xD9, 0x9B, 0x05, 0x08, 0x00, 0x00, 0x00, 0x00, 0xED, 0x9B, 0x05, 0x08, 0x01, 0x00, 0x10, 0x26, 0x06, 0x00, 0x40, 0x50, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x1E, 0x24, 0x03, 0x00, 0x5E, 0x50, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x9B, 0x05, 0x08, 0x00, 0x00, 0x00, 0x00, 0x15, 0x9C, 0x05, 0x08, 0x01, 0x00, 0x10, 0x26, 0x06, 0x00, 0x40, 0x50, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x1E, 0x24, 0x03, 0x00, 0x5E, 0x50, 0x00, 0x00, 0x00, 0x00, 0x97, 0xB2, 0x05, 0x08, 0x00, 0x00, 0x00, 0x00, 0xAB, 0xB2, 0x05, 0x08, 0x01, 0x00, 0x31, 0x26, 0x06, 0x00, 0x40, 0x50, 0x00, 0x00, 0x00, 0x00, 0x02, 0x00, 0x1E, 0x24, 0x03, 0x00, 0x5E, 0x50, 0x00, 0x00, 0x00, 0x00, 0xC0, 0xB2, 0x05, 0x08, 0x00, 0x00, 0x00, 0x00, 0xD7, 0xB2, 0x05, 0x08, 0x01, 0x00, 0x31, 0x26 });
            //Jump patches
            dataAccess.ApplyPatch(MFL_SK_Part1, new byte[8] { 0x24, 0xFE, 0x09, 0x08, 0x00, 0x00, 0x00, 0x00 });
            dataAccess.ApplyPatch(MFL_SK_Part2, new byte[8] { 0x2D, 0xFE, 0x09, 0x08, 0x00, 0x00, 0x00, 0x00 });
            dataAccess.ApplyPatch(DMK_Part1, new byte[8] { 0x36, 0xFE, 0x09, 0x08, 0x00, 0x00, 0x00, 0x00 });
            dataAccess.ApplyPatch(DMK_Part2, new byte[8] { 0x3F, 0xFE, 0x09, 0x08, 0x00, 0x00, 0x00, 0x00 });


        }

        protected override void Remove()
        {
            //Actual fix is revert is part of Restore nopped function in Fast Intro removal

            //Jump fixes
            dataAccess.ApplyPatch(MFL_SK_Part1, new byte[8] { 0x15, 0x00, 0x40, 0x50, 0x01, 0x00, 0x10, 0x26 });
            dataAccess.ApplyPatch(MFL_SK_Part2, new byte[8] { 0x17, 0x00, 0x40, 0x50, 0x01, 0x00, 0x10, 0x26 });
            dataAccess.ApplyPatch(DMK_Part1, new byte[8] { 0x15, 0x00, 0x40, 0x50, 0x01, 0x00, 0x31, 0x26 });
            dataAccess.ApplyPatch(DMK_Part2, new byte[8] { 0x18, 0x00, 0x40, 0x50, 0x01, 0x00, 0x31, 0x26 });
        }
    }
}
