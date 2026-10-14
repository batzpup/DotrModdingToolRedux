namespace DotrModdingTool2IMGUI;

public static class UpdateStartingDeck
{
    static int patchLocation = 0x269E00;
    const int StarterDeckCount = 17;
    const int EntrySizeBytes = 12;
    static DataAccess dataAccess = DataAccess.Instance;

    // The vanilla table was not generated with the formula below, so an entry is only
    // regenerated when its computed value differs from the one computed at load time.
    static byte[][] loadedEntryBytes = new byte[StarterDeckCount][];
    static byte[][] loadedComputedBytes = new byte[StarterDeckCount][];

    static readonly StartingDeckData.StarterDeckDataEnums.Attribute[] attributeEnumValues = Enum.GetValues<StartingDeckData.StarterDeckDataEnums.Attribute>();
    static readonly StartingDeckData.StarterDeckDataEnums.Kind[] kindEnumValues = Enum.GetValues<StartingDeckData.StarterDeckDataEnums.Kind>();

    public static void SnapshotLoadedData(List<Deck> decks)
    {
        byte[] isoBytes = dataAccess.ReadBytes(patchLocation, StarterDeckCount * EntrySizeBytes);
        for (int i = 0; i < StarterDeckCount; i++)
        {
            loadedEntryBytes[i] = isoBytes.AsSpan(i * EntrySizeBytes, EntrySizeBytes).ToArray();
            loadedComputedBytes[i] = BuildEntry(decks[i]);
        }
    }

    public static void CreateNewStartingDeckData(List<Deck> decks)
    {
        byte[][] patchBytes = new byte[StarterDeckCount][];

        for (int i = 0; i < StarterDeckCount; i++)
        {
            byte[] computed = BuildEntry(decks[i]);
            bool unchanged = loadedComputedBytes[i] != null && computed.SequenceEqual(loadedComputedBytes[i]);
            patchBytes[i] = unchanged ? loadedEntryBytes[i] : computed;
        }

        byte[] combinedArray = CombineArrays(patchBytes);
        dataAccess.ApplyPatch(patchLocation, combinedArray);
    }

    static byte[] BuildEntry(Deck deck)
    {
        StartingDeckData startingDeckData = new StartingDeckData();
        var attributeCounts = attributeEnumValues.ToDictionary(attr => attr, attr => (byte)0);
        var kindCounts = kindEnumValues.ToDictionary(kind => kind, kind => (byte)0);
        float totalSummonPower = 0;
        startingDeckData.LeaderID = deck.DeckLeader.Number;
        byte numberOfMonsters = 0;
        for (int j = 0; j < deck.CardList.Count; j++)
        {
            DeckCard card = deck.CardList[j];
            if (card.CardConstant.CardColor == CardColourType.NormalMonster || card.CardConstant.CardColor == CardColourType.EffectMonster)
            {
                numberOfMonsters++;
                string kindString = card.CardConstant.CardKind.Name.Replace(' ', '_').Replace("-", "_");
                if (Enum.TryParse(card.Attribute, out StartingDeckData.StarterDeckDataEnums.Attribute attribute))
                {
                    totalSummonPower += card.Level;
                    attributeCounts[attribute]++;
                }
                if (Enum.TryParse(kindString, out StartingDeckData.StarterDeckDataEnums.Kind kind))
                {
                    kindCounts[kind]++;
                }
            }
        }

        SetStartingDeckDataAttributes(startingDeckData, attributeCounts);
        SetStartingDeckDataKinds(startingDeckData, kindCounts);

        startingDeckData.AverageSummonLevel = (ushort)((totalSummonPower / numberOfMonsters) * 100);
        return startingDeckData.ToByteArray();
    }

    private static void SetStartingDeckDataAttributes(StartingDeckData data, Dictionary<StartingDeckData.StarterDeckDataEnums.Attribute, byte> counts)
    {
        var topAttributes = counts.OrderByDescending(pair => pair.Value)
            .Take(4)
            .Select(pair => pair.Value > 0 ? pair.Key : StartingDeckData.StarterDeckDataEnums.Attribute.None)
            .ToArray();

        data.Attributes =
            ((byte)topAttributes[0] * (8 * 8 * 8) + (byte)topAttributes[1] * (8 * 8) + (byte)topAttributes[2] * 8 + (byte)topAttributes[3]);
    }

    static void SetStartingDeckDataKinds(StartingDeckData data, Dictionary<StartingDeckData.StarterDeckDataEnums.Kind, byte> counts)
    {
        var topKinds = counts.OrderByDescending(pair => pair.Value)
            .Take(4)
            .Select(pair => pair.Value > 0 ? pair.Key : StartingDeckData.StarterDeckDataEnums.Kind.None)
            .ToArray();

        data.Kinds = ((byte)topKinds[0] * (32 * 32 * 32) + (byte)topKinds[1] * (32 * 32) + (byte)topKinds[2] * 32 + (byte)topKinds[3]);
    }

    static byte[] CombineArrays(byte[][] arrays)
    {
        using (var memoryStream = new MemoryStream())
        {
            foreach (byte[] subArray in arrays)
            {
                if (subArray != null)
                {
                    memoryStream.Write(subArray, 0, subArray.Length);
                }
            }
            return memoryStream.ToArray();
        }
    }
}

internal class StartingDeckData
{
    public ushort LeaderID;
    public ushort AverageSummonLevel;
    public int Attributes;
    public int Kinds;


    public byte[] ToByteArray()
    {
        using (var memoryStream = new MemoryStream())
        using (var writer = new BinaryWriter(memoryStream))
        {
            writer.Write(LeaderID);
            writer.Write(AverageSummonLevel);
            writer.Write(Attributes);
            writer.Write(Kinds);

            return memoryStream.ToArray();
        }
    }

    public static class StarterDeckDataEnums
    {
        public enum Attribute
        {
            None,
            Light,
            Dark,
            Fire,
            Earth,
            Water,
            Wind
        }

        public enum Kind
        {
            None,
            Dragon,
            Spellcaster,
            Zombie,
            Warrior,
            Beast_Warrior,
            Beast,
            Winged_Beast,
            Fiend,
            Fairy,
            Insect,
            Dinosaur,
            Reptile,
            Fish,
            Sea_Serpent,
            Machine,
            Thunder,
            Aqua,
            Pyro,
            Rock,
            Plant,
            Immortal
        }
    }
}