using System.Collections;
namespace DotrModdingTool2IMGUI;

public class TreasureCards
{
    static TreasureCards instance;

    public List<TreasureCard> Treasures = new List<TreasureCard> { };
    public List<TreasureCard> DefaultTreasures = new List<TreasureCard> { };

    static byte[] defaultTreasureBytes = new byte[] {
        0x00, 0x00, 0xE7, 0x03, 0x01, 0x33, 0xAA, 0x02, 0x02, 0x36, 0xD3, 0x02, 0x03, 0x06, 0xDA, 0x02, 0x04, 0x23, 0xD2, 0x02, 0x05, 0x33, 0xA0, 0x02,
        0x06, 0x00, 0xD1, 0x02, 0x07, 0x15, 0xD8, 0x02, 0x08, 0x06,
        0x28, 0x02, 0x09, 0x03, 0xA5, 0x02, 0x0A, 0x13, 0xD0, 0x02, 0x0B, 0x22, 0xA8, 0x02, 0x0C, 0x32, 0x49, 0x03, 0x0D, 0x20, 0xD9, 0x02, 0x0E, 0x21,
        0xA6, 0x02, 0x0F, 0x20, 0xA9, 0x02, 0x10, 0x33, 0xA1, 0x02, 0x11, 0x33, 0xF4, 0x01, 0x12, 0x60, 0xA3, 0x02, 0x13, 0x36, 0xA4, 0x02, 0x14, 0x33,
        0xCE, 0x02, 0x15, 0x33, 0xDD, 0x02
    };

    public static TreasureCards Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new TreasureCards();
            }
            return instance;
        }
    }

    public TreasureCards()
    {
        for (int i = 0, bi = 0; i < DataAccess.TreasureCardCount; i++, bi += DataAccess.TreasureCardByteSize)
        {
            TreasureCard defaultTreasureCards = new TreasureCard(new byte[]
                { defaultTreasureBytes[bi], defaultTreasureBytes[bi + 1], defaultTreasureBytes[bi + 2], defaultTreasureBytes[bi + 3] });
            this.DefaultTreasures.Add(defaultTreasureCards);
        }
    }

    public void InitTreasureData(byte[] treasureData)
    {

        Treasures.Clear();
        //Resets default treasures
        for (int i = 0, bi = 0; i < DataAccess.TreasureCardCount; i++, bi += DataAccess.TreasureCardByteSize)
        {
            TreasureCard defaultTreasureCards = new TreasureCard(new byte[]
                { defaultTreasureBytes[bi], defaultTreasureBytes[bi + 1], defaultTreasureBytes[bi + 2], defaultTreasureBytes[bi + 3] });
            this.DefaultTreasures.Add(defaultTreasureCards);
        }
        
        for (int i = 0, bi = 0; i < DataAccess.TreasureCardCount; i++, bi += DataAccess.TreasureCardByteSize)
        {
            TreasureCard treasureCard = new TreasureCard(new byte[]
                { treasureData[bi], treasureData[bi + 1], treasureData[bi + 2], treasureData[bi + 3] });
            this.Treasures.Add(treasureCard);

        }
    }

    public void ResetTreasureData()
    {
        Treasures.Clear();
        Treasures = new List<TreasureCard>(DefaultTreasures);
    }
}

public class TreasureCard
{
    byte[] treasureData;
    byte duelIndex;
    public BitArray gridLocationBitArray;
    //string enemyName;
    ushort cardIndex;
    //string cardName;

    public byte Row { get; set; }
    public byte Column { get; set; }

    public ushort CardIndex
    {
        get { return this.cardIndex; }

        set
        {
            this.cardIndex = value;
           // this.cardName = Card.GetNameByIndex(this.cardIndex);
        }
    }

    public ModdedStringName CardName
    {
        get { return Card.GetNameByIndex(this.cardIndex); }
    }

    public byte EnemyIndex
    {
        get { return this.duelIndex; }

        set
        {
            this.duelIndex = value;
          //  this.enemyName = Enemy.GetEnemyNameByIndex(this.duelIndex);
        }
    }

    public ModdedStringName EnemyName
    {
        get { return Enemy.GetEnemyNameByIndex(this.duelIndex); }
    }

    public TreasureCard(byte[] treasureData)
    {
        this.treasureData = treasureData;
        duelIndex = treasureData[0];
        gridLocationBitArray = new BitArray(new byte[] { treasureData[1] });
        CardIndex = BitConverter.ToUInt16(new byte[] { treasureData[2], treasureData[3] }, 0);
        EnemyIndex = this.duelIndex;
        Row = gridLocationBitArray.toByte(4, 4);
        Column = gridLocationBitArray.toByte(0, 4);
    }

    public byte[] Bytes
    {
        get
        {
            byte[] bytes = new byte[4];
            // Pack Row and Column into a single byte with Row in the upper 4 bits and Column in the lower 4 bits
            byte gridLocation = (byte)((Row & 0xF) << 4 | (Column & 0xF));

            bytes[0] = this.duelIndex;
            bytes[1] = gridLocation;
            BitConverter.GetBytes(this.cardIndex).CopyTo(bytes, 2);

            return bytes;
        }
    }
}