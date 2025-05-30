using DotrModdingTool2IMGUI;
namespace GameplayPatches
{
    public class DcRuleChanges : Patch
    {
        static int postGamePatchLocation = 0x209f40;
        static int TaTutoDbgInit34 = 0x145f60;
        static int allGamePatchLocation = 0x209f44;

        public DcRules GetRule()
        {
            if (dataAccess.CheckIfPatchApplied(postGamePatchLocation, new byte[8] { 0x2a, 0x08, 0x43, 0x00, 0x08, 0x00, 0x20, 0x10 }))
            {
                return DcRules.Normal;
            }
            if (dataAccess.CheckIfPatchApplied(postGamePatchLocation, new byte[8] { 0x98, 0xd7, 0x05, 0x08, 0x00, 0x00, 0x00, 0x00 }))
            {
                return DcRules.NoCheckPostGame;
            }
            if (dataAccess.CheckIfPatchApplied(postGamePatchLocation, new byte[] { 0x99, 0xe7, 0x08, 0x08 }))
            {
                return DcRules.NoCheckAll;
            }
            return DcRules.Unknown;
        }

        void ApplyVanilla()
        {
            dataAccess.ApplyPatch(postGamePatchLocation, new byte[8] { 0x2a, 0x08, 0x43, 0x00, 0x08, 0x00, 0x20, 0x10 });
        }

        void ApplyPostGame()
        {
            dataAccess.ApplyPatch(postGamePatchLocation, new byte[8] { 0x98, 0xd7, 0x05, 0x08, 0x00, 0x00, 0x00, 0x00 });
            dataAccess.ApplyPatch(TaTutoDbgInit34,
                new byte[60] {
                    0x01, 0x00, 0x0c, 0x82, 0x03, 0x00, 0x80, 0x15, 0x00, 0x00, 0x00, 0x00, 0x04, 0x00, 0x00, 0x10, 0x08, 0x08, 0x00, 0x70, 0x02, 0x00,
                    0x0c, 0x82, 0x06, 0x00, 0x80, 0x15, 0x00, 0x00, 0x00, 0x00, 0x2a, 0x08, 0x43, 0x00, 0x03, 0x00, 0x20, 0x10, 0x00, 0x00, 0x00, 0x00,
                    0x93, 0xe7, 0x08, 0x08, 0x00, 0x00, 0x00, 0x00, 0x99, 0xe7, 0x08, 0x08, 0x00, 0x00, 0x00, 0x00
                });
        }

        void ApplyAllGame()
        {
            dataAccess.ApplyPatch(allGamePatchLocation, new byte[] { 0x99, 0xe7, 0x08, 0x08 });
        }

        public void Apply(DcRules rules)
        {
            ApplyVanilla();
            switch (rules)
            {
                case DcRules.NoCheckPostGame:
                    ApplyPostGame();
                    break;
                case DcRules.NoCheckAll:
                    ApplyAllGame();
                    break;
            }

        }

        protected override void Remove()
        {
            ApplyVanilla();
        }
    }
}