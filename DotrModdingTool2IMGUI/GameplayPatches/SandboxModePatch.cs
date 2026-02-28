using DotrModdingTool2IMGUI;
namespace GameplayPatches

{
    public class SandboxModePatch : Patch
    {
        static int patchLocation = 0x0021c548 - DataAccess.IsoSlusRamOffset;
        static int patchLocation2 = 0x0021c550 - DataAccess.IsoSlusRamOffset;
        static int Test1 = 0x0021c6ec - DataAccess.IsoSlusRamOffset;
        static int Test2 = 0x0021c6f0 - DataAccess.IsoSlusRamOffset;
        static int Test3 = 0x0021c6f8 - DataAccess.IsoSlusRamOffset;
        static int Test4Arena = 0x0021c708 - DataAccess.IsoSlusRamOffset;

        //static int mapFlags1 = 0x0021d424 - DataAccess.IsoSlusRamOffset;
        //static int mapFlags2 = 0x0021d2d4 - DataAccess.IsoSlusRamOffset;
        //static int mapFlags3 = 0x0021d198 - DataAccess.IsoSlusRamOffset;
        //static int mapFlags4 = 0x0021d0e4 - DataAccess.IsoSlusRamOffset;
        //static int mapFlags5 = 0x0021d024 - DataAccess.IsoSlusRamOffset;
        //static int mapFlags6 = 0x0021cf64 - DataAccess.IsoSlusRamOffset;
        //static int mapFlags7 = 0x0021cea4 - DataAccess.IsoSlusRamOffset;
        //static int mapFlags8 = 0x0021c708 - DataAccess.IsoSlusRamOffset;
        //static int mapFlags9 = 0x0021cd84 - DataAccess.IsoSlusRamOffset;
        //static int mapFlags10 = 0x0021c9b4 - DataAccess.IsoSlusRamOffset;
        //static int mapFlags11 = 0x0021ca5c - DataAccess.IsoSlusRamOffset;


        public override bool IsApplied()
        {
            return dataAccess.CheckIfPatchApplied(patchLocation, new byte[4] { 0x01, 0x00, 0x15, 0x24 });
        }

        protected override void Apply()
        {
           // dataAccess.ApplyPatch(patchLocation, new byte[4] { 0x01, 0x00, 0x15, 0x24 });
           // dataAccess.ApplyPatch(patchLocation2, new byte[4] { 0x03, 0x00, 0x16, 0x24 });

           // dataAccess.ApplyPatch(Test1, new byte[4] { 0x01, 0x00, 0x35, 0xa2 });
           // dataAccess.ApplyPatch(Test2, new byte[4] { 0x02, 0x00, 0x35, 0xa2 });
           // dataAccess.ApplyPatch(Test3, new byte[4] { 0x8c, 0x01, 0x35, 0xa0 });
           // dataAccess.ApplyPatch(Test4Arena, new byte[4] { 0x03, 0x00, 0x56, 0xa0 });


        }

        protected override void Remove()
        {
            // dataAccess.ApplyPatch(patchLocation, new byte[4] { 0x00, 0x00, 0x00, 0x00 });
            // dataAccess.ApplyPatch(patchLocation2, new byte[4] { 0x00, 0x00, 0x00, 0x00 });

            // dataAccess.ApplyPatch(Test1, new byte[4] { 0x01, 0x00, 0x20, 0xa2 });
            // dataAccess.ApplyPatch(Test2, new byte[4] { 0x02, 0x00, 0x20, 0xa2 });
            // dataAccess.ApplyPatch(Test3, new byte[4] { 0x8c, 0x01, 0x20, 0xa0 });
            // dataAccess.ApplyPatch(Test4Arena, new byte[4] { 0x03, 0x00, 0x40, 0xa0 });

        }
    }
}