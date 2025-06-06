﻿namespace GameplayPatches;

class CustomMusicPatch : Patch
{
    
    static int AddCustomMusicPtr = 0x17ac58;
    static int TaTuto_DrawTrapArea = 0x24f800;
    
    public override bool IsApplied()
    {
        return dataAccess.CheckIfPatchApplied(AddCustomMusicPtr, new byte[8] { 0xc0, 0xfd, 0x09, 0x08, 0x15, 0x00, 0x03, 0x24  });
    }

    protected void Apply(Dictionary<int, int> duelistMusic)
    {
            byte[] bytes = new byte[288] {
                0x3c, 0x01, 0xf8, 0x8c, 0x00, 0x00, 0x03, 0x24, 0x43, 0x00, 0x03, 0x13, 0x07, 0x00, 0x06, 0x24, 0x01, 0x00, 0x03, 0x24, 0x40, 0x00,
                0x03, 0x13, 0x10, 0x00, 0x06, 0x24, 0x02, 0x00, 0x03, 0x24, 0x3d, 0x00, 0x03, 0x13, 0x08, 0x00, 0x06, 0x24, 0x03, 0x00, 0x03, 0x24,
                0x3a, 0x00, 0x03, 0x13, 0x08, 0x00, 0x06, 0x24, 0x04, 0x00, 0x03, 0x24, 0x37, 0x00, 0x03, 0x13, 0x08, 0x00, 0x06, 0x24, 0x05, 0x00,
                0x03, 0x24, 0x34, 0x00, 0x03, 0x13, 0x08, 0x00, 0x06, 0x24, 0x06, 0x00, 0x03, 0x24, 0x31, 0x00, 0x03, 0x13, 0x08, 0x00, 0x06, 0x24,
                0x07, 0x00, 0x03, 0x24, 0x2e, 0x00, 0x03, 0x13, 0x08, 0x00, 0x06, 0x24, 0x08, 0x00, 0x03, 0x24, 0x2b, 0x00, 0x03, 0x13, 0x08, 0x00,
                0x06, 0x24, 0x09, 0x00, 0x03, 0x24, 0x28, 0x00, 0x03, 0x13, 0x08, 0x00, 0x06, 0x24, 0x0a, 0x00, 0x03, 0x24, 0x25, 0x00, 0x03, 0x13,
                0x08, 0x00, 0x06, 0x24, 0x0b, 0x00, 0x03, 0x24, 0x22, 0x00, 0x03, 0x13, 0x09, 0x00, 0x06, 0x24, 0x0c, 0x00, 0x03, 0x24, 0x1f, 0x00,
                0x03, 0x13, 0x09, 0x00, 0x06, 0x24, 0x0d, 0x00, 0x03, 0x24, 0x1c, 0x00, 0x03, 0x13, 0x09, 0x00, 0x06, 0x24, 0x0e, 0x00, 0x03, 0x24,
                0x19, 0x00, 0x03, 0x13, 0x23, 0x00, 0x06, 0x24, 0x0f, 0x00, 0x03, 0x24, 0x16, 0x00, 0x03, 0x13, 0x09, 0x00, 0x06, 0x24, 0x10, 0x00,
                0x03, 0x24, 0x13, 0x00, 0x03, 0x13, 0x09, 0x00, 0x06, 0x24, 0x11, 0x00, 0x03, 0x24, 0x10, 0x00, 0x03, 0x13, 0x09, 0x00, 0x06, 0x24,
                0x12, 0x00, 0x03, 0x24, 0x0d, 0x00, 0x03, 0x13, 0x09, 0x00, 0x06, 0x24, 0x13, 0x00, 0x03, 0x24, 0x0a, 0x00, 0x03, 0x13, 0x0b, 0x00,
                0x06, 0x24, 0x14, 0x00, 0x03, 0x24, 0x07, 0x00, 0x03, 0x13, 0x0c, 0x00, 0x06, 0x24, 0x15, 0x00, 0x03, 0x24, 0x04, 0x00, 0x03, 0x13,
                0x2c, 0x00, 0x06, 0x24, 0x28, 0x00, 0x06, 0x24, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00, 0xab, 0x06, 0x08, 0x00, 0x00,
                0x00, 0x00
            };
            int j = 0;
            for (int i = 12; j < 22; i += 12)
            {
                int trackNumber = 0;
                trackNumber = duelistMusic[j];
                if (trackNumber == 39 || trackNumber == 6)
                {
                    trackNumber = 7;
                }
                var newBytes = BitConverter.GetBytes(trackNumber);
                j++;
                bytes[i] = newBytes[0];
            }
            //patch jump
            dataAccess.ApplyPatch(AddCustomMusicPtr, new byte[8] { 0xc0, 0xfd, 0x09, 0x08, 0x15, 0x00, 0x03, 0x24 });
            dataAccess.ApplyPatch(TaTuto_DrawTrapArea, bytes);

            //Add music restart fix for combat

            dataAccess.ApplyPatch(0x1a0208, new byte[4] { 0x01, 0x00, 0x04, 0x24 });
            dataAccess.ApplyPatch(0x1a0d04, new byte[4] { 0x01, 0x00, 0x04, 0x24 });
            dataAccess.ApplyPatch(0x1a0ee0, new byte[4] { 0x01, 0x00, 0x04, 0x24 });
            dataAccess.ApplyPatch(0x1a0f80, new byte[4] { 0x01, 0x00, 0x04, 0x24 });
            
    }


    protected override void Remove()
    {
        dataAccess.ApplyPatch(AddCustomMusicPtr, new byte[8] { 0x3c, 0x01, 0xe6, 0x8c, 0x15, 0x00, 0x03, 0x24 });
        dataAccess.ApplyPatch(0x1a0208, new byte[4] { 0x84, 0x0c, 0x24, 0x96 });
        dataAccess.ApplyPatch(0x1a0d04, new byte[4] { 0x84, 0x0c, 0x24, 0x96 });
        dataAccess.ApplyPatch(0x1a0ee0, new byte[4] { 0x84, 0x0c, 0x24, 0x96 });
        dataAccess.ApplyPatch(0x1a0f80, new byte[4] { 0x84, 0x0c, 0x24, 0x96 });
    }

    public void ApplyOrRemove(bool apply, Dictionary<int, int> DuelistMusic)
    {
        if (apply)
        {
            Apply(DuelistMusic);
        }
        else
        {
            Remove();
        }
    }
}