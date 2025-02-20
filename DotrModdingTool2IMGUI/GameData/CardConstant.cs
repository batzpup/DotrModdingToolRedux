using System.Collections;
using System.Text;
namespace DotrModdingTool2IMGUI;

public class CardConstant
{
    public static List<CardConstant> List = new List<CardConstant> { };
    public static Dictionary<string, CardConstant> CardLookup;

    public static void LoadFromBytes(byte[][] bytes)
    {
        List.Clear();

        for (ushort i = 0; i < Card.TotalCardCount; i++)
        {
            List.Add(new CardConstant(i, bytes[i]));
        }
        CardLookup = List.ToDictionary(c => c.Name);
    }

    public static List<CardConstant> Monsters
    {
        get
        {
            CardColourType[] monsterTypes = { CardColourType.NormalMonster, CardColourType.EffectMonster };
            return List.FindAll(constant => monsterTypes.Contains(constant.CardColor)).ToList<CardConstant>();
        }
    }

    public static List<CardConstant> NonEquips
    {
        get { return List.FindAll(cardConstant => cardConstant._cardKind.Id != 64); }
    }

    public static byte[] AllBytes
    {
        get { return List.SelectMany(c => c.Bytes).ToArray(); }
    }

    CardColourType _cardColourType;
    public CardColourType CardColor => _cardColourType;

    const int maxAttackDefense = 8191;

    byte kind;
    CardKind _cardKind;
    byte kindOfs;
    BitArray levelAttribute;
    byte level;
    CardAttribute attribute;
    ushort effectId;
    ushort xaxId;
    BitArray apWithFlags;
    ushort attack;
    bool hasAlternateArt;
    bool isSlotRare;
    bool appearsInSlotReels;
    BitArray dpWithFlags;
    ushort defense;
    bool hasImage;
    bool passwordWorks;
    bool appearsInReincarnation;
    byte[] passwordArray;
    string password;

    public string Password
    {
        get => password.ToUpper();
        set
        {
            password = value.ToLower();
            passwordArray = EncryptPassword(EncodePassword(password));
        }
    }


    public CardConstant(ushort cardIndex, byte[] bytes)
    {
        Index = cardIndex;
        Name = Card.GetNameByIndex(cardIndex);
        kind = bytes[0];
        _cardKind = new CardKind(this.kind);
        kindOfs = bytes[1];
        levelAttribute = new BitArray(new byte[] { bytes[2] });
        level = bytes[2].splitByte()[1];
        attribute = new CardAttribute(bytes[2].splitByte()[0]);
        DeckCost = bytes[3];
        effectId = BitConverter.ToUInt16(new byte[] { bytes[4], bytes[5] }, 0);
        xaxId = BitConverter.ToUInt16(new byte[] { bytes[6], bytes[7] }, 0);
        apWithFlags = new BitArray(new byte[] { bytes[8], bytes[9] });
        attack = CardConstant.GetAttackOrDefense(new byte[] { bytes[8], bytes[9] });
        hasImage = apWithFlags[apWithFlags.Length - 3];
        passwordWorks = apWithFlags[apWithFlags.Length - 2];
        appearsInReincarnation = apWithFlags[apWithFlags.Length - 1];
        dpWithFlags = new BitArray(new byte[] { bytes[10], bytes[11] });
        defense = CardConstant.GetAttackOrDefense(new byte[] { bytes[10], bytes[11] });
        appearsInSlotReels = dpWithFlags[dpWithFlags.Length - 3];
        isSlotRare = dpWithFlags[dpWithFlags.Length - 2];
        hasAlternateArt = dpWithFlags[dpWithFlags.Length - 1];
        passwordArray = new byte[] { bytes[12], bytes[13], bytes[14], bytes[15], bytes[16], bytes[17], bytes[18], bytes[19] };
        password = DecodePassword(DecryptPassword(passwordArray));
        setCardColor();
    }

    string DecodePassword(byte[] decryptedBytes)
    {
        StringBuilder stringBuilder = new StringBuilder();
        for (int i = 0; i < decryptedBytes.Length; i++)
        {
            stringBuilder.Append((char)decryptedBytes[i]);
        }
        return stringBuilder.ToString().ToUpper();
    }

    //Bounds checking is done in the input field
    byte[] EncodePassword(string password)
    {
        byte[] encodedBytes = new byte[password.Length];
        for (int i = 0; i < password.Length; i++)
        {
            encodedBytes[i] = (byte)password[i];
        }
        return encodedBytes;
    }

    byte[] DecryptPassword(byte[] encryptedBytes)
    {
        byte[] decryptedBytes = new byte[encryptedBytes.Length];
        string key = "HANNIBAL";
        for (int i = 0; i < encryptedBytes.Length; i++)
        {
            decryptedBytes[i] = (byte)((encryptedBytes[i] + key[(i + Index) % 8]) % 256);
        }
        return decryptedBytes;
    }

    byte[] EncryptPassword(byte[] plainBytes)
    {
        byte[] encryptedBytes = new byte[plainBytes.Length];
        string key = "HANNIBAL";
        for (int i = 0; i < plainBytes.Length; i++)
        {
            encryptedBytes[i] = (byte)((plainBytes[i] - key[(i + Index) % 8] + 256) % 256);
        }
        return encryptedBytes;
    }

    public void setCardColor()
    {
        if (EffectId == 65535)
        {
            _cardColourType = CardColourType.NormalMonster;
        }
        else if (CardKind.Id < 32)
        {
            _cardColourType = CardColourType.EffectMonster;
        }
        else if (CardKind.Id == 96 || this.CardKind.Id == 128)
        {
            _cardColourType = CardColourType.Trap;
        }
        else if (CardKind.Id == 160)
        {
            _cardColourType = CardColourType.Ritual;
        }
        else
        {
            _cardColourType = CardColourType.Magic;
        }
    }

    public static ushort GetAttackOrDefense(byte[] bytes)
    {
        BitArray bitArray = new BitArray(bytes);
        bitArray[bitArray.Length - 1] = false;
        bitArray[bitArray.Length - 2] = false;
        bitArray[bitArray.Length - 3] = false;
        byte[] newBytes = new byte[2];
        bitArray.CopyTo(newBytes, 0);
        return BitConverter.ToUInt16(newBytes, 0);
    }

    private byte[] calculateByteSequence()
    {
        byte[] newByteArray = new byte[20];
        newByteArray[0] = this.kind;
        newByteArray[1] = this.kindOfs;
        newByteArray[2] = this.levelAttribute.toByte();
        newByteArray[3] = this.DeckCost;
        newByteArray[4] = BitConverter.GetBytes(this.effectId)[0];
        newByteArray[5] = BitConverter.GetBytes(this.effectId)[1];
        newByteArray[6] = BitConverter.GetBytes(this.xaxId)[0];
        newByteArray[7] = BitConverter.GetBytes(this.xaxId)[1];
        byte[] apWithFlagsByteArray = new byte[2];
        this.apWithFlags.CopyTo(apWithFlagsByteArray, 0);
        newByteArray[8] = apWithFlagsByteArray[0];
        newByteArray[9] = apWithFlagsByteArray[1];
        byte[] dpWithFlagsByteArray = new byte[2];
        this.dpWithFlags.CopyTo(dpWithFlagsByteArray, 0);
        newByteArray[10] = dpWithFlagsByteArray[0];
        newByteArray[11] = dpWithFlagsByteArray[1];
        this.passwordArray.CopyTo(newByteArray, 12);

        return newByteArray;
    }

    public byte[] Bytes
    {
        get { return this.calculateByteSequence(); }
    }

    public ushort Index { get; }

    public string Name { get; }

    public byte DeckCost { get; set; }

    public byte Kind
    {
        get => kind;
        set
        {
            kind = value;
            _cardKind = new CardKind(kind);
        }
    }

    public CardKind CardKind
    {
        get { return this._cardKind; }
    }

    public string? Type
    {
        get { return this._cardKind.Name; }
    }


    public ushort EffectId
    {
        get { return this.effectId; }
        set
        {
            if (value >= 0 && value <= 255)
            {
                effectId = value;
            }
            else
            {
                effectId = 65535;
            }
            ;
        }
    }

    public byte Level
    {
        get { return this.level; }

        set
        {
            this.level = value;
            this.levelAttribute.setHalfByte(value, 4);
        }
    }

    public byte Attribute
    {
        get { return this.attribute.Id; }

        set
        {
            this.attribute.Id = value;
            this.levelAttribute.setHalfByte(value, 0);
        }
    }

    public ushort XaxId
    {
        get { return this.xaxId; }
    }

    public string AttributeName
    {
        get
        {
            if (_cardKind.isMonster())
            {
                return attribute.Name;
            }

            return "";
        }
    }


    public ushort Attack
    {
        get { return this.attack; }
        set
        {
            this.attack = this.roundAttackDefense(value);
            byte[] attackBytes = BitConverter.GetBytes(this.attack);
            BitArray attackBitArray = new BitArray(attackBytes);
            attackBitArray.copyRangeTo(new int[] { 0, 12 }, ref this.apWithFlags, 0);
        }
    }

    public ushort Defense
    {
        get { return this.defense; }
        set
        {
            this.defense = this.roundAttackDefense(value);
            byte[] defenseBytes = BitConverter.GetBytes(this.defense);
            BitArray defenseBitArray = new BitArray(defenseBytes);
            defenseBitArray.copyRangeTo(new int[] { 0, 12 }, ref this.dpWithFlags, 0);
        }
    }

    private ushort roundAttackDefense(ushort value)
    {
        if (value < 0)
        {
            value = 0;
        }
        else if (value > CardConstant.maxAttackDefense)
        {
            value = CardConstant.maxAttackDefense;
        }

        return value;
    }

    public bool IsSlotRare
    {
        get { return this.isSlotRare; }

        set
        {
            this.isSlotRare = value;
            this.dpWithFlags[dpWithFlags.Length - 2] = this.isSlotRare;
        }
    }

    public bool AppearsInSlotReels
    {
        get { return this.appearsInSlotReels; }

        set
        {
            this.appearsInSlotReels = value;
            this.dpWithFlags[this.dpWithFlags.Length - 3] = this.appearsInSlotReels;
        }
    }

    public bool AppearsInReincarnation
    {
        get { return this.appearsInReincarnation; }

        set
        {
            this.appearsInReincarnation = value;
            this.apWithFlags[this.apWithFlags.Length - 1] = this.appearsInReincarnation;
        }
    }

    public bool PasswordWorks
    {
        get { return this.passwordWorks; }

        set
        {
            this.passwordWorks = value;
            this.apWithFlags[apWithFlags.Length - 2] = this.passwordWorks;
        }
    }

    public bool HasImage
    {
        get { return this.hasImage; }
    }

    public bool HasAlternateArt
    {
        get { return this.hasAlternateArt; }
    }

    public override string ToString()
    {
        return Name;
    }
}