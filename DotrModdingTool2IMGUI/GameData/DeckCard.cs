namespace DotrModdingTool2IMGUI;

public class DeckCard
{
    private CardConstant cardConstant;
    private DeckLeaderRank rank;
    public byte[] Bytes;

    public DeckCard(byte[] cardBytes)
    {
        Bytes = cardBytes;
        ushort value = BitConverter.ToUInt16(cardBytes, 0);
        rank =  (DeckLeaderRank)(value >> 12);
        ushort cardNumber = (ushort)(((cardBytes[1] & 0x0F) << 8) + cardBytes[0]);
        CardConstant = CardConstant.List[cardNumber];
    }

    public DeckCard(CardConstant cardConstant, DeckLeaderRank deckLeaderRank)
    {
        this.cardConstant = cardConstant;
        rank = deckLeaderRank;

        Bytes = calculateBytes(CardConstant, Rank);
    }

    public DeckLeaderRank Rank
    {
        get { return rank; }

        set
        {
            rank = value;
            Bytes = calculateBytes(CardConstant, rank);
        }
    }

    public CardConstant CardConstant
    {
        get { return cardConstant; }

        set
        {
            cardConstant = value;
            Bytes = calculateBytes(CardConstant, Rank);
        }
    }

    public ModdedStringName Name
    {
        get { return CardConstant.Name; }
    }

    public ushort Number
    {
        get { return CardConstant.Index; }
    }

    public string? Type
    {
        get { return CardConstant.Type; }
    }

    public string Attribute
    {
        get { return CardConstant.AttributeName; }
    }

    public ushort Attack
    {
        get { return CardConstant.Attack; }
    }

    public ushort Defense
    {
        get { return CardConstant.Defense; }
    }

    public byte DeckCost
    {
        get { return CardConstant.DeckCost; }
    }

    public byte Level
    {
        get { return CardConstant.Level; }
    }

    public override string ToString()
    {
        return CardConstant.Name.Current;
    }

    private static byte[] calculateBytes(CardConstant cardConstant, DeckLeaderRank Rank)
    {
        ushort newValue = (ushort)((cardConstant.Index & 0xFFF) | (((ushort)Rank & 0xF) << 12));
        return BitConverter.GetBytes(newValue);
    }
}