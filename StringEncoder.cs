namespace DotrModdingTool2IMGUI;

public static class StringEncoder
{
    //IS BROKEN DO NOT USE
     static readonly Dictionary<int, string> knownChars = new Dictionary<int, string> {
        { 0x00, "\n" }, { 0x01, "PLAYER_NAME_CHAR" }, { 0x02, "," }, { 0x03, "\u25CF" },
        { 0x1E, " " }, { 0x1F, "\uFF3B" }, { 0x20, "\uFF3D" }, { 0x3B, "\uFF08" },
        { 0x3C, "\uFF09" }, { 0x46, "\uFF11" }, { 0x47, "\uFF01" }, { 0x48, "\uFF02" },
        { 0x49, "\uFF03" }, { 0x4A, "\uFF04" }, { 0x4B, "\uFF05" }, { 0x4C, "\uFF06" },
        { 0x4D, "\uFF07" }, { 0x4E, "\uFF1D" }, { 0x4F, "\uFF3E" }, { 0x50, "\uFF0D" },
        { 0x51, "\uFFE5" }, { 0x52, "\uFF0C" }, { 0x53, "\uFF0E" }, { 0x54, "\uFF0F" },
        { 0x55, "\uFF3F" }, { 0x70, " " }, { 0x71, "[" }, { 0x72, "]" }, { 0x8D, "(" },
        { 0x8E, ")" }, { 0x98, "0" }, { 0x99, "!" }, { 0x9A, "\"" }, { 0x9B, "#" },
        { 0x9C, "$" }, { 0x9D, "%" }, { 0x9E, "&" }, { 0x9F, "'" }, { 0xA0, "=" },
        { 0xA1, "^" }, { 0xA2, "-" }, { 0xA3, "\u00A5" }, { 0xA4, "." }, { 0xA5, "/" },
        { 0xA6, "_" }, { 0x14F, "\u221E" }, { 0x151, "?" }, { 0x152, ":" },
        { 0x153, "\u00B7\u00B7\u00B7" }, { 0x169, "\u03B1" }, { 0x16A, "<" }, { 0x16B, ">" },
        { 0x16C, "3." }, { 0x16D, "\u25A1" }, { 0x16E, "\u25B3" }, { 0x16F, "\u00D7" }, { 0x170, ";" }
    };
    
    static List<int> EncodeStrings(List<string> inputList)
    {
        List<int> encodedBytes = new List<int>();

        foreach (var input in inputList)
        {
            List<int> encodedString = EncodeString(input);
            encodedBytes.AddRange(encodedString); 
        }
        return encodedBytes;
    }
    
    static string DecodeString(List<int> encodedBytes)
    {
        List<string> decodedChars = new List<string>();

        foreach (var encodedByte in encodedBytes)
        {
            if (encodedByte == 0xFFFD)
            {
                decodedChars.Add("?"); // Unknown character, represented as '?'
            }
            else if (knownChars.ContainsKey(encodedByte))
            {
                decodedChars.Add(knownChars[encodedByte]);
            }
            else
            {
                decodedChars.Add("?"); // For any unexpected byte, use a placeholder
            }
        }

        return string.Join(string.Empty, decodedChars);
    }

    static List<int> EncodeString(string input)
    {
        List<int> encodedBytes = new List<int>();
        foreach (var ch in input)
        {
            bool found = false;
            foreach (var kvp in knownChars)
            {
                if (kvp.Value == ch.ToString())
                {
                    encodedBytes.Add(kvp.Key);
                    found = true;
                    break;
                }
            }
            if (!found)
            {
                encodedBytes.Add(0xFFFD);
            }
        }
        return encodedBytes;
    }

    public static void Run()
    {
        for (int i = 0; i < 26; i++)
        {
            knownChars[0x56 + i] = ((char)('A' + i)).ToString();
            knownChars[0x73 + i] = ((char)('a' + i)).ToString();
            knownChars[0x04 + i] = ((char)(0xFF21 + i)).ToString();
            knownChars[0x21 + i] = ((char)(0xFF41 + i)).ToString();
        }
        for (int i = 0; i < 9; i++)
        {
            knownChars[0x8F + i] = (i + 1).ToString();
            knownChars[0x3D + i] = ((char)(0xFF11 + i)).ToString();
        }
        List<string> inputStrings = StringDecoder.StringTable.Values.ToList();
        Console.WriteLine("Encoded String List:");
        List<int> encoded = EncodeStrings(inputStrings);
        Console.WriteLine(string.Join(", ", encoded));
        
        string decodedString = DecodeString(encoded);
        Console.WriteLine("Decoded String: ");
        Console.WriteLine(decodedString);
    }
}