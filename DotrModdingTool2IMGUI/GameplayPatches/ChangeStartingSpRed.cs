namespace GameplayPatches;

class ChangeStartingSpRed : Patch
{
    public static int patchLocationRed = 0x17E1CC;

    public override bool IsApplied()
    {
        return !dataAccess.CheckIfPatchApplied(patchLocationRed, new byte[4] { 0x04, 0x00, 0x02, 0x24 });
    }

    public void Apply(uint lp)
    {
        byte[] value = BitConverter.GetBytes(lp);
        dataAccess.ApplyPatch(patchLocationRed, new byte[4] { value[0], value[1], 0x02, 0x24 });
    }

    protected override void Remove()
    {
        dataAccess.ApplyPatch(patchLocationRed, new byte[4] { 0x04, 0x00, 0x02, 0x24 });
    }

    public void ApplyOrRemove(bool apply, uint value)
    {
        if (apply)
        {
            Apply(value);
        }
        else
        {
            Remove();
        }
    }
}

class ChangeStartingSpWhite : Patch
{
    public static int patchLocationWhite = 0x17E270;

    public override bool IsApplied()
    {
        return !dataAccess.CheckIfPatchApplied(patchLocationWhite, new byte[4] { 0x04, 0x00, 0x03, 0x24 });
    }

    public void Apply(uint lp)
    {
        byte[] value = BitConverter.GetBytes(lp);
        dataAccess.ApplyPatch(patchLocationWhite, new byte[4] { value[0], value[1], 0x03, 0x24 });
    }


    protected override void Remove()
    {
        dataAccess.ApplyPatch(patchLocationWhite, new byte[4] { 0x04, 0x00, 0x03, 0x24 });
    }

    public void ApplyOrRemove(bool apply, uint value)
    {
        if (apply)
        {
            Apply(value);
        }
        else
        {
            Remove();
        }
    }
}