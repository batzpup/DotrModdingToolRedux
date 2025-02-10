

using System;
/// WORKS
namespace GameplayPatches
{
    class ChangeLpRecovery : Patch
    {
        static int patchLocation = 0x24268C;
        public override bool IsApplied()
        {
            return !dataAccess.CheckIfPatchApplied(patchLocation, new byte[4] { 0x32, 0x00, 0x04, 0x24 });
        }

        public void Apply(Decimal numLpRecoveryValue)
        {
            byte[] value = BitConverter.GetBytes((uint)numLpRecoveryValue);
            dataAccess.ApplyPatch(patchLocation, new byte[4] { value[0], value[1], 0x04, 0x24 });
        }
          

        protected override void Remove()
        {
            dataAccess.ApplyPatch(patchLocation, new byte[4] { 0x32, 0x00, 0x04, 0x24 });
        }
        public void ApplyOrRemove(bool apply, decimal numLpRecoveryValue)
        {
            if (apply)
            {
                Apply(numLpRecoveryValue);
             }
            else
            {
                Remove();
            }
        }
    }
}
