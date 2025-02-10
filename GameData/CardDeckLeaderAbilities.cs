namespace DotrModdingTool2IMGUI;

public static class CardDeckLeaderAbilities
{
    public static List<DeckLeaderAbilityInstance> MonsterAbilities = new List<DeckLeaderAbilityInstance>();


    public static byte[] Bytes
    {
        get { return MonsterAbilities.SelectMany(a => a.Abilities.SelectMany(b => b.Bytes)).ToArray(); }
    }
}