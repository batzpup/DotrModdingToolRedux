namespace GameplayPatches;

class ChangeStartingLp : Patch
{
    public static int patchLocationRed = 0x17E1DC;
    public static int patchLocationWhite = 0x17E280;

    public override bool IsApplied()
    {
        return !dataAccess.CheckIfPatchApplied(patchLocationRed, new byte[4] { 0xa0, 0x0f, 0x02, 0x24 });
    }

    public void Apply(uint lp, int side)
    {
        byte[] value = BitConverter.GetBytes(lp);
        if (side == 0)
        {
            dataAccess.ApplyPatch(patchLocationRed, new byte[4] { value[0], value[1], 0x02, 0x24 });
        }
        else
        {
            dataAccess.ApplyPatch(patchLocationWhite, new byte[4] { value[0], value[1], 0x03, 0x24 });
        }

    }


    protected override void Remove()
    {

        dataAccess.ApplyPatch(patchLocationRed, new byte[4] { 0xa0, 0x0f, 0x02, 0x24 });
        dataAccess.ApplyPatch(patchLocationWhite, new byte[4] { 0xa0, 0x0f, 0x03, 0x24 });
    }

    public void ApplyOrRemove(bool apply, uint value, int side)
    {
        if (apply)
        {
            Apply(value, side);
        }
        else
        {
            Remove();
        }
    }
}