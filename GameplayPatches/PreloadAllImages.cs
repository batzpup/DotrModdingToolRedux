using System;

namespace GameplayPatches
{
    //DO NOT USE DOESNT WORK SHITS BROKEN, also not how the game loads cards images sad
    public class PreloadAllImages : Patch
    {
        static int AI_Tut_05 = 0x14AB70;
        static int relocatePicPack = 0x1d26cc;

        public override bool IsApplied()
        {
            return dataAccess.CheckIfPatchApplied(relocatePicPack, new byte[8] { 0x18, 0x00, 0x03, 0x3c, 0x70, 0xaa, 0x7e, 0x24 }   );
        }

        protected override void Apply()
        {
            ushort[] cardIds = new ushort[854];
            for (ushort i = 0; i < cardIds.Length; i++)
            {
                cardIds[i] = i;
            }
            byte[] byteArray = new byte[cardIds.Length * 2];
            for (int i = 0; i < cardIds.Length; i++)
            {
                byte[] bytes = BitConverter.GetBytes(cardIds[i]);

                // Copy the bytes to the result array
                Array.Copy(bytes, 0, byteArray, i * 2, 2);
            }
            dataAccess.ApplyPatch(AI_Tut_05,byteArray);
            dataAccess.ApplyPatch(relocatePicPack, new byte[8] { 0x18, 0x00, 0x03, 0x3c, 0x70, 0xaa, 0x7e, 0x24 });

        }

        protected override void Remove()
        {
            dataAccess.ApplyPatch(relocatePicPack, new byte[8] { 0x2d, 0x00, 0x03, 0x3c, 0xfc, 0xe9, 0x7e, 0x24 });
        }
    }


}
