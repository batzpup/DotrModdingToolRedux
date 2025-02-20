namespace DotrModdingTool2IMGUI;

public enum AttributeVisual
{
    Light,
    Dark,
    Fire,
    Earth,
    Water,
    Wind,
    Magic,
    Trap
}

public class CardAttribute
{
    public static int GetAttributeVisual(CardConstant cardConstant)
    {

        if (cardConstant.CardKind.isMonster())
        {
            return cardConstant.Attribute;
        }
        if (cardConstant.CardKind.isTrap())
        {
            return (int)AttributeVisual.Trap;
        }
        return (int)AttributeVisual.Magic;
    }

    public static readonly string[] AttributeNames = [
        "Light",
        "Dark",
        "Fire",
        "Earth",
        "Water",
        "Wind"
    ];

    public CardAttribute(byte attributeId)
    {
        Id = attributeId;
    }

    byte id;

    public byte Id
    {
        get => id;
        set
        {
            id = value;
            Name = AttributeNames[id];
        }
    }

    public string Name { get; private set; }
}