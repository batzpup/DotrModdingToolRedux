namespace DotrModdingTool2IMGUI;

public class CardKind
{
    public static Dictionary<byte, string> Kinds = new Dictionary<byte, string>() {
        { 0, "Dragon" },
        { 1, "Spellcaster" },
        { 2, "Zombie" },
        { 3, "Warrior" },
        { 4, "Beast-Warrior" },
        { 5, "Beast" },
        { 6, "Winged-Beast" },
        { 7, "Fiend" },
        { 8, "Fairy" },
        { 9, "Insect" },
        { 10, "Dinosaur" },
        { 11, "Reptile" },
        { 12, "Fish" },
        { 13, "Sea Serpent" },
        { 14, "Machine" },
        { 15, "Thunder" },
        { 16, "Aqua" },
        { 17, "Pyro" },
        { 18, "Rock" },
        { 19, "Plant" },
        { 20, "Immortal" },
        { 32, "Magic" },
        { 64, "Power Up" },
        { 96, "Trap (Limited Range)" },
        { 128, "Trap (Full Range)" },
        { 160, "Ritual" }
    };

    public enum CardKindEnum : byte
    {
        Dragon = 0,
        Spellcaster = 1,
        Zombie = 2,
        Warrior = 3,
        BeastWarrior = 4,
        Beast = 5,
        WingedBeast = 6,
        Fiend = 7,
        Fairy = 8,
        Insect = 9,
        Dinosaur = 10,
        Reptile = 11,
        Fish = 12,
        SeaSerpent = 13,
        Machine = 14,
        Thunder = 15,
        Aqua = 16,
        Pyro = 17,
        Rock = 18,
        Plant = 19,
        Immortal = 20,
        Magic = 32,
        PowerUp = 64,
        TrapLimitedRange = 96,
        TrapFullRange = 128,
        Ritual = 160
    }

    public CardKind(byte cardKindId)
    {
        this.Id = cardKindId;

    }

    public bool isMonster()
    {
        return this.Id / 32 == 0;
    }

    public bool isMagic()
    {
        return this.Id / 32 == 1;
    }

    public bool isPowerUp()
    {
        return this.Id / 32 == 2;
    }

    public bool isTrap()
    {
        return this.Id / 32 == 3 || this.Id / 32 == 4;
    }

    public bool isRitual()
    {
        return this.Id / 32 == 5;
    }


    byte id;

    public byte Id
    {
        get => id;
        set
        {
            id = value;
            if (!Kinds.ContainsKey(id))
            {
                Kinds.Add(id, $"Custom: {id}");
            }
            Name = Kinds[Id];

        }
    }

    public string? Name { get; set; }
}
