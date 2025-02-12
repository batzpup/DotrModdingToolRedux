namespace DotrModdingTool2IMGUI;

public class Deck
{
    public int Index { get; }
    public DeckCard DeckLeader { get; set; } = null;
    public List<DeckCard> CardList { get; set; }

    public Deck()
    {
        CardList = new List<DeckCard> { };
    }

    public Deck(int index, byte[][] bytes)
    {
        Index = index;
        DeckLeader = new DeckCard(bytes[0]);
        CardList = new List<DeckCard> { };

        for (int cardIndex = 1; cardIndex <= 40; cardIndex++)
        {
            DeckCard deckCard = new DeckCard(bytes[cardIndex]);
            CardList.Add(deckCard);
        }
    }

    public int DeckCost
    {
        get { return CardList.Sum(deckCard => deckCard.CardConstant.DeckCost); }
    }

    public override string ToString()
    {
        return $"{NamePrefix(Index)} - {DeckLeader.CardConstant.Name}";
    }

    public static List<Deck> LoadDeckListFromBytes(byte[][][] bytes)
    {
        List<Deck> deckList = new List<Deck> { };

        for (int deckIndex = 0; deckIndex < bytes.Length; deckIndex++)
        {
            Deck deck = new Deck(deckIndex, bytes[deckIndex]);
            deckList.Add(deck);
        }

        return deckList;
    }

    public void Save(DataAccess dataAccess)
    {
        if (DeckLeader == null)
        {
            throw new InvalidOperationException("Deck must have a deck leader.");
        }

        if (CardList.Count != 40)
        {
            throw new InvalidOperationException("Deck must have 40 cards.");
        }

        CardList.Sort((c1, c2) => c1.Number - c2.Number);
        dataAccess.SaveDeck(Index, Bytes);
    }

    public byte[] Bytes
    {
        get
        {
            byte[] byteArray = new byte[DataAccess.DeckCardCount * DataAccess.DeckCardByteCount];
            DeckLeader.Bytes.CopyTo(byteArray, 0);

            for (int cardIndex = 0; cardIndex < CardList.Count; cardIndex++)
            {
                byte[] deckCardBytes = CardList[cardIndex].Bytes;
                deckCardBytes.CopyTo(byteArray, (1 + cardIndex) * DataAccess.DeckCardByteCount);
            }

            return byteArray;
        }
    }

    public static string NamePrefix(int deckIndex)
    {
        if (deckIndex < 17)
        {
            return $"Starter Deck {deckIndex}";
        }

        if (deckIndex < 27)
        {
            return $"Tutorial Deck {deckIndex-17}";
        }

        return CharacterNameDictionary[deckIndex];
    }

    public static Dictionary<int, string> CharacterNameDictionary;

    static Deck()
    {
        CharacterNameDictionary = new Dictionary<int, string> {
            { 27, "Seto" },
            { 28, "Weevil Underwood" },
            { 29, "Rex Raptor" },
            { 30, "Keith" },
            { 31, "Ishtar" },
            { 32, "Necromancer" },
            { 33, "Darkness-ruler" },
            { 34, "Labyrinth-ruler" },
            { 35, "Pegasus Crawford" },
            { 36, "Richard Slysheen of York" },
            { 37, "Tea" },
            { 38, "T. Tristan Grey" },
            { 39, "Margaret Mai Beaufort" },
            { 40, "Mako" },
            { 41, "Joey" },
            { 42, "J. Shadi Morton" },
            { 43, "Jasper Dice Tudor" },
            { 44, "Bakura" },
            { 45, "Yugi" },
            { 46, "Manawyddan fab Llyr (vs White Rose)" },
            { 47, "Manawyddan fab Llyr (vs Red Rose)" },
            { 48, "Deck Master I" },
            { 49, "Deck Master T" },
            { 50, "Deck Master S" }
        };
    }
}