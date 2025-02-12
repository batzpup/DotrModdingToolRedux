﻿

namespace GameplayPatches
{
    /// <summary>
    /// TOO MANY DAMN IMPLICATIONS DOING THIS, Intern who did the AI needs this for AI to not break, but breaks how things are loaded and what teams are thing in your hand
    /// </summary>
    public class AiHandTeamFix : Patch
    {
        static int patchLocation = 0x130648;


        public override bool IsApplied()
        {
            return dataAccess.CheckIfPatchApplied(patchLocation, new byte[4] { 0x43, 0x11, 0x02, 0x00 });
        }

        protected override void Apply()
        {
            

            //Fix Yugi HandPlay
            dataAccess.ApplyPatch(patchLocation, new byte[648] { 0x44, 0x00, 0xb6, 0xa2, 0x43, 0x11, 0x02, 0x00, 0xFF, 0x00, 0x42, 0x30, 0x6A, 0x00, 0x40, 0x14, 0x28, 0x96, 0x00, 0x70, 0x67, 0x00, 0x80, 0x06, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x02, 0x24, 0xC0, 0x00, 0xA2, 0xAF, 0xFF, 0xFF, 0x02, 0x34, 0xB0, 0x00, 0xA2, 0xAF, 0x03, 0x22, 0x14, 0x00, 0x24, 0x84, 0x07, 0x0C, 0xFF, 0x00, 0x85, 0x32, 0x28, 0x9E, 0x00, 0x70, 0x03, 0xF2, 0x02, 0x00, 0xFF, 0x00, 0x57, 0x30, 0x28, 0x26, 0xC0, 0x72, 0x04, 0x39, 0x09, 0x0C, 0x28, 0x2E, 0x60, 0x72, 0x28, 0x86, 0x40, 0x70, 0x4E, 0x00, 0x02, 0x06, 0x01, 0x00, 0x73, 0x26, 0xFF, 0x00, 0x12, 0x32, 0x03, 0x8A, 0x10, 0x00, 0x28, 0x26, 0x40, 0x72, 0xAC, 0x88, 0x07, 0x0C, 0x28, 0x2E, 0x20, 0x72, 0x08, 0x00, 0x03, 0x24, 0x45, 0x00, 0x43, 0x10, 0x00, 0x00, 0x00, 0x00, 0x28, 0x26, 0x40, 0x72, 0xAC, 0x88, 0x07, 0x0C, 0x28, 0x2E, 0x20, 0x72, 0x09, 0x00, 0x03, 0x24, 0x0A, 0x00, 0x43, 0x14, 0x28, 0x26, 0x40, 0x72, 0x28, 0x26, 0xA0, 0x72, 0x28, 0x2E, 0x40, 0x72, 0x90, 0x3F, 0x09, 0x0C, 0x28, 0x36, 0x20, 0x72, 0x01, 0x00, 0x03, 0x24, 0x38, 0x00, 0x43, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x28, 0x26, 0x40, 0x72, 0xB8, 0x8D, 0x07, 0x0C, 0x28, 0x2E, 0x20, 0x72, 0x32, 0x00, 0x40, 0x14, 0x00, 0x00, 0x00, 0x00, 0x60, 0x89, 0x05, 0x0C, 0x28, 0x26, 0xC0, 0x72, 0x28, 0x26, 0xA0, 0x72, 0x28, 0x2E, 0x80, 0x72, 0x28, 0x36, 0x40, 0x70, 0x28, 0x3E, 0xE0, 0x72, 0x28, 0x46, 0xC0, 0x73, 0x44, 0xE1, 0x09, 0x0C, 0x28, 0x4E, 0x00, 0x70, 0x13, 0x00, 0x40, 0x18, 0xD0, 0x00, 0xA5, 0x27, 0x60, 0x89, 0x05, 0x0C, 0x28, 0x26, 0xC0, 0x72, 0xFF, 0xFF, 0x43, 0x30, 0x03, 0x00, 0x02, 0x24, 0x00, 0x00, 0xA2, 0xFF, 0xD0, 0x00, 0xA5, 0x27, 0x28, 0x36, 0x40, 0x72, 0x28, 0x3E, 0x20, 0x72, 0x08, 0x00, 0xA3, 0xFF, 0x28, 0x26, 0xA0, 0x72, 0x28, 0x46, 0xE0, 0x72, 0x28, 0x4E, 0xC0, 0x73, 0x28, 0x56, 0x80, 0x72, 0x4C, 0xE8, 0x09, 0x0C, 0x28, 0x5E, 0x00, 0x70, 0x0B, 0x00, 0x00, 0x10, 0xD0, 0x00, 0xA4, 0x27, 0xD0, 0x00, 0xA5, 0x27, 0x03, 0x00, 0x0B, 0x24, 0x28, 0x36, 0x40, 0x72, 0x28, 0x3E, 0x20, 0x72, 0xFF, 0xFF, 0x8A, 0x32, 0x28, 0x26, 0xA0, 0x72, 0x28, 0x46, 0xE0, 0x72, 0xB0, 0xE4, 0x09, 0x0C, 0x28, 0x4E, 0xC0, 0x73, 0xD0, 0x00, 0xA4, 0x27, 0xF0, 0xEB, 0x09, 0x0C, 0x28, 0x8E, 0x40, 0x70, 0x08, 0x00, 0x40, 0x18, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0xA2, 0x8F, 0x2A, 0x08, 0x22, 0x02, 0x04, 0x00, 0x20, 0x10, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0xB1, 0xAF, 0xB0, 0x00, 0xB0, 0xAF, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x73, 0x26, 0x08, 0x00, 0x62, 0x2A, 0xAC, 0xFF, 0x40, 0x14, 0x28, 0x26, 0xC0, 0x72, 0xB0, 0x00, 0xA2, 0x8F, 0xFF, 0xFF, 0x03, 0x34, 0x03, 0x00, 0x43, 0x10, 0x00, 0x00, 0x00, 0x00, 0x2A, 0x00, 0x00, 0x10, 0xA0, 0x00, 0xBF, 0x7B, 0x28, 0x96, 0x00, 0x70, 0x28, 0x26, 0xC0, 0x72, 0x04, 0x39, 0x09, 0x0C, 0x28, 0x2E, 0x40, 0x72, 0x28, 0x9E, 0x40, 0x70, 0x1E, 0x00, 0x62, 0x06, 0x01, 0x00, 0x52, 0x26, 0xFF, 0x00, 0x71, 0x32, 0x03, 0x82, 0x13, 0x00, 0x28, 0x26, 0x20, 0x72, 0xAC, 0x88, 0x07, 0x0C, 0x28, 0x2E, 0x00, 0x72, 0x08, 0x00, 0x03, 0x24, 0x15, 0x00, 0x43, 0x10, 0x00, 0x00, 0x00, 0x00, 0x28, 0x26, 0x20, 0x72, 0xAC, 0x88, 0x07, 0x0C, 0x28, 0x2E, 0x00, 0x72, 0x09, 0x00, 0x03, 0x24, 0x09, 0x00, 0x43, 0x14, 0x28, 0x26, 0x20, 0x72, 0x28, 0x26, 0xA0, 0x72, 0x28, 0x2E, 0x20, 0x72, 0x90, 0x3F, 0x09, 0x0C, 0x28, 0x36, 0x00, 0x72, 0x01, 0x00, 0x03, 0x24, 0x08, 0x00, 0x43, 0x10, 0x00, 0x00, 0x00, 0x00, 0x28, 0x26, 0x20, 0x72, 0xE4, 0x8D, 0x07, 0x0C, 0x28, 0x2E, 0x00, 0x72, 0x03, 0x00, 0x41, 0x04, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x10, 0x28, 0x16, 0x60, 0x72, 0x01, 0x00, 0x52, 0x26, 0x08, 0x00, 0x42, 0x2A, 0xDC, 0xFF, 0x40, 0x14, 0x28, 0x26, 0xC0, 0x72, 0xFF, 0xFF, 0x02, 0x24, 0xA0, 0x00, 0xBF, 0x7B, 0x90, 0x00, 0xBE, 0x7B, 0x80, 0x00, 0xB7, 0x7B, 0x70, 0x00, 0xB6, 0x7B, 0x60, 0x00, 0xB5, 0x7B, 0x50, 0x00, 0xB4, 0x7B, 0x40, 0x00, 0xB3, 0x7B, 0x30, 0x00, 0xB2, 0x7B, 0x20, 0x00, 0xB1, 0x7B, 0x10, 0x00, 0xB0, 0x7B, 0x08, 0x00, 0xE0, 0x03, 0xE0, 0x00, 0xBD, 0x27, 0x00, 0x00, 0x00, 0x00});
        }

        protected override void Remove()
        {
            //Restore Yugi HandPlay
            dataAccess.ApplyPatch(patchLocation, new byte[648] { 0x43, 0x11, 0x02, 0x00, 0xFF, 0x00, 0x42, 0x30, 0x6A, 0x00, 0x40, 0x14, 0x28, 0x96, 0x00, 0x70, 0x67, 0x00, 0x80, 0x06, 0x00, 0x00, 0x00, 0x00, 0xFF, 0x00, 0x02, 0x24, 0xC0, 0x00, 0xA2, 0xAF, 0xFF, 0xFF, 0x02, 0x34, 0xB0, 0x00, 0xA2, 0xAF, 0x03, 0x22, 0x14, 0x00, 0x24, 0x84, 0x07, 0x0C, 0xFF, 0x00, 0x85, 0x32, 0x28, 0x9E, 0x00, 0x70, 0x03, 0xF2, 0x02, 0x00, 0xFF, 0x00, 0x57, 0x30, 0x28, 0x26, 0xC0, 0x72, 0x04, 0x39, 0x09, 0x0C, 0x28, 0x2E, 0x60, 0x72, 0x28, 0x86, 0x40, 0x70, 0x4E, 0x00, 0x02, 0x06, 0x01, 0x00, 0x73, 0x26, 0xFF, 0x00, 0x12, 0x32, 0x03, 0x8A, 0x10, 0x00, 0x28, 0x26, 0x40, 0x72, 0xAC, 0x88, 0x07, 0x0C, 0x28, 0x2E, 0x20, 0x72, 0x08, 0x00, 0x03, 0x24, 0x45, 0x00, 0x43, 0x10, 0x00, 0x00, 0x00, 0x00, 0x28, 0x26, 0x40, 0x72, 0xAC, 0x88, 0x07, 0x0C, 0x28, 0x2E, 0x20, 0x72, 0x09, 0x00, 0x03, 0x24, 0x0A, 0x00, 0x43, 0x14, 0x28, 0x26, 0x40, 0x72, 0x28, 0x26, 0xA0, 0x72, 0x28, 0x2E, 0x40, 0x72, 0x90, 0x3F, 0x09, 0x0C, 0x28, 0x36, 0x20, 0x72, 0x01, 0x00, 0x03, 0x24, 0x38, 0x00, 0x43, 0x10, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x28, 0x26, 0x40, 0x72, 0xB8, 0x8D, 0x07, 0x0C, 0x28, 0x2E, 0x20, 0x72, 0x32, 0x00, 0x40, 0x14, 0x00, 0x00, 0x00, 0x00, 0x60, 0x89, 0x05, 0x0C, 0x28, 0x26, 0xC0, 0x72, 0x28, 0x26, 0xA0, 0x72, 0x28, 0x2E, 0x80, 0x72, 0x28, 0x36, 0x40, 0x70, 0x28, 0x3E, 0xE0, 0x72, 0x28, 0x46, 0xC0, 0x73, 0x44, 0xE1, 0x09, 0x0C, 0x28, 0x4E, 0x00, 0x70, 0x13, 0x00, 0x40, 0x18, 0xD0, 0x00, 0xA5, 0x27, 0x60, 0x89, 0x05, 0x0C, 0x28, 0x26, 0xC0, 0x72, 0xFF, 0xFF, 0x43, 0x30, 0x03, 0x00, 0x02, 0x24, 0x00, 0x00, 0xA2, 0xFF, 0xD0, 0x00, 0xA5, 0x27, 0x28, 0x36, 0x40, 0x72, 0x28, 0x3E, 0x20, 0x72, 0x08, 0x00, 0xA3, 0xFF, 0x28, 0x26, 0xA0, 0x72, 0x28, 0x46, 0xE0, 0x72, 0x28, 0x4E, 0xC0, 0x73, 0x28, 0x56, 0x80, 0x72, 0x4C, 0xE8, 0x09, 0x0C, 0x28, 0x5E, 0x00, 0x70, 0x0B, 0x00, 0x00, 0x10, 0xD0, 0x00, 0xA4, 0x27, 0xD0, 0x00, 0xA5, 0x27, 0x03, 0x00, 0x0B, 0x24, 0x28, 0x36, 0x40, 0x72, 0x28, 0x3E, 0x20, 0x72, 0xFF, 0xFF, 0x8A, 0x32, 0x28, 0x26, 0xA0, 0x72, 0x28, 0x46, 0xE0, 0x72, 0xB0, 0xE4, 0x09, 0x0C, 0x28, 0x4E, 0xC0, 0x73, 0xD0, 0x00, 0xA4, 0x27, 0xF0, 0xEB, 0x09, 0x0C, 0x28, 0x8E, 0x40, 0x70, 0x08, 0x00, 0x40, 0x18, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0xA2, 0x8F, 0x2A, 0x08, 0x22, 0x02, 0x04, 0x00, 0x20, 0x10, 0x00, 0x00, 0x00, 0x00, 0xC0, 0x00, 0xB1, 0xAF, 0xB0, 0x00, 0xB0, 0xAF, 0x00, 0x00, 0x00, 0x00, 0x01, 0x00, 0x73, 0x26, 0x08, 0x00, 0x62, 0x2A, 0xAC, 0xFF, 0x40, 0x14, 0x28, 0x26, 0xC0, 0x72, 0xB0, 0x00, 0xA2, 0x8F, 0xFF, 0xFF, 0x03, 0x34, 0x03, 0x00, 0x43, 0x10, 0x00, 0x00, 0x00, 0x00, 0x2A, 0x00, 0x00, 0x10, 0xA0, 0x00, 0xBF, 0x7B, 0x28, 0x96, 0x00, 0x70, 0x28, 0x26, 0xC0, 0x72, 0x04, 0x39, 0x09, 0x0C, 0x28, 0x2E, 0x40, 0x72, 0x28, 0x9E, 0x40, 0x70, 0x1E, 0x00, 0x62, 0x06, 0x01, 0x00, 0x52, 0x26, 0xFF, 0x00, 0x71, 0x32, 0x03, 0x82, 0x13, 0x00, 0x28, 0x26, 0x20, 0x72, 0xAC, 0x88, 0x07, 0x0C, 0x28, 0x2E, 0x00, 0x72, 0x08, 0x00, 0x03, 0x24, 0x15, 0x00, 0x43, 0x10, 0x00, 0x00, 0x00, 0x00, 0x28, 0x26, 0x20, 0x72, 0xAC, 0x88, 0x07, 0x0C, 0x28, 0x2E, 0x00, 0x72, 0x09, 0x00, 0x03, 0x24, 0x09, 0x00, 0x43, 0x14, 0x28, 0x26, 0x20, 0x72, 0x28, 0x26, 0xA0, 0x72, 0x28, 0x2E, 0x20, 0x72, 0x90, 0x3F, 0x09, 0x0C, 0x28, 0x36, 0x00, 0x72, 0x01, 0x00, 0x03, 0x24, 0x08, 0x00, 0x43, 0x10, 0x00, 0x00, 0x00, 0x00, 0x28, 0x26, 0x20, 0x72, 0xE4, 0x8D, 0x07, 0x0C, 0x28, 0x2E, 0x00, 0x72, 0x03, 0x00, 0x41, 0x04, 0x00, 0x00, 0x00, 0x00, 0x06, 0x00, 0x00, 0x10, 0x28, 0x16, 0x60, 0x72, 0x01, 0x00, 0x52, 0x26, 0x08, 0x00, 0x42, 0x2A, 0xDC, 0xFF, 0x40, 0x14, 0x28, 0x26, 0xC0, 0x72, 0xFF, 0xFF, 0x02, 0x24, 0xA0, 0x00, 0xBF, 0x7B, 0x90, 0x00, 0xBE, 0x7B, 0x80, 0x00, 0xB7, 0x7B, 0x70, 0x00, 0xB6, 0x7B, 0x60, 0x00, 0xB5, 0x7B, 0x50, 0x00, 0xB4, 0x7B, 0x40, 0x00, 0xB3, 0x7B, 0x30, 0x00, 0xB2, 0x7B, 0x20, 0x00, 0xB1, 0x7B, 0x10, 0x00, 0xB0, 0x7B, 0x08, 0x00, 0xE0, 0x03, 0xE0, 0x00, 0xBD, 0x27, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00 });
            
        }
    }
}
