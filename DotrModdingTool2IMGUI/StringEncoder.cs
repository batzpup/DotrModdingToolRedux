namespace DotrModdingTool2IMGUI;

//Original code by  LordMewtwo73 https://github.com/LordMewtwo73/YGO-DOTR-Text-Editor, cleaned up and adapted by batzpup
public class StringEncoder
{
    //Contains compressed strings   
    List<string> modStrings;
    List<Pointer2> PointerList = new List<Pointer2>();
    Dictionary<char, string> ReplacedStrings = new Dictionary<char, string>();
    Dictionary<char, Pointer2> PointerDictionary = new Dictionary<char, Pointer2>();
    List<int> Offsets = new List<int>();

    string nameString = new string(StringEditor.PNamePlaceholder, 12);

    //assumes strings before 30 cant be edited which is valid

    public void CompressStrings(List<string> strings, ref int progress)
    {

        modStrings = strings;
        for (int i = 0; i < modStrings.Count; i++)
        {
            modStrings[i] = modStrings[i]
                .Replace("PNAME_CHAR", StringEditor.PNamePlaceholder.ToString());
            //.Replace("III", "\uFFF3")
        }
        for (int i = 0; i < modStrings.Count; i++)
        {
            if (i >= 30)
            {
                if (modStrings[i].Contains(new string(StringEditor.PNamePlaceholder, 12)))
                {
                    // Mark the player name block temporarily with a unique placeholder
                    string tempToken = "\uFFFC";
                    modStrings[i] = modStrings[i].Replace(new string(StringEditor.PNamePlaceholder, 12), tempToken);

                    // Compress the rest of the string
                    modStrings[i] = ReplacePiecesOfWords(modStrings[i], i);

                    // Restore the 12× PNAME_CHAR block
                    modStrings[i] = modStrings[i].Replace(tempToken, new string(StringEditor.PNamePlaceholder, 12));
                }
                else
                {
                    modStrings[i] = ReplacePiecesOfWords(modStrings[i], i);
                }
            }
        }
    }

    string ReplacePiecesOfWords(string instring, int index)
    {
        for (int i = 30; i < index; i++)
        {
            Substring comparison = LargestStringCompare(i, instring, modStrings[i]);
            if (comparison.SubString.Length >= 3)
            {
                instring = CreatePointer(comparison, index, i, instring);
                i--;
            }
        }
        return instring;
    }

    string CreatePointer(Substring substring, int index, int pindex, string instring)
    {
        char replaceChar = (char)(0xE000 + PointerList.Count);
        ReplacedStrings.Add(replaceChar, substring.SubString);
        ReplaceAll(substring.SubString, index, replaceChar);
        instring = instring.Replace(substring.SubString, (replaceChar).ToString());
        if (substring.SubString == nameString)
        {
            Console.WriteLine("Foundname string");
            return instring;
        }

        int length = 0;
        int offset = 0;
        for (int i = 0; i < substring.SubString.Length; i++)
        {
            if (substring.SubString[i] >= '\uE000' && substring.SubString[i] < '\uFF00')
                length += 2;
            else
                length++;

        }

        for (int i = 0; i < modStrings[substring.Index].Length; i++)
        {
            if (i < substring.Offset)
            {
                if (modStrings[substring.Index][i] >= '\uE000' && modStrings[substring.Index][i] < '\uFF00')
                    offset += 2;
                else
                    offset++;
            }
        }

        Pointer2 newPointer = new Pointer2(index, pindex, offset, length, substring.SubString);
        PointerDictionary.Add(replaceChar, newPointer);
        PointerList.Add(newPointer);
        return instring;
    }

    void ReplaceAll(string replacethis, int starthere, char replacechar)
    {
        for (int i = starthere; i < modStrings.Count; i++)
        {
            modStrings[i] = modStrings[i].Replace(replacethis, (replacechar).ToString());
        }
    }

    Substring LargestStringCompare(int index, string findthis, string inthis)
    {
        List<int[]> foundchars = new List<int[]>();
        foreach (char c in findthis)
        {
            if (c == '\n')
            {
                foundchars.Add(new int[0] { });
            }
            else
            {
                List<int> foundIndexes = new List<int>();
                for (int i = inthis.IndexOf(c); i > -1; i = inthis.IndexOf(c, i + 1))
                {
                    // for loop end when i=-1 ('a' not found)
                    foundIndexes.Add(i);
                }
                foundchars.Add(foundIndexes.ToArray());
            }
        }

        int longestLength = 0;
        int longestStart = -1;

        for (int i = 0; i < foundchars.Count; i++)
        {
            for (int j = 0; j < foundchars[i].Length; j++)
            {
                int currCount = 1;
                int currIndex = i;
                int currValue = foundchars[i][j];
                int trueOffset = 0;
                for (int k = 0; k < foundchars[i][j]; k++)
                {
                    if (inthis[k] >= '\uE000' && inthis[k] < '\uFF00')
                        trueOffset += 2;
                    else
                        trueOffset++;
                }
                // offset is saved with 7 bits, so cannot be greater than 127
                if (trueOffset > 127)
                    continue;
                ;
                while (true)
                {
                    // length is saved with 6 bits, so cannot be greater than 63
                    if (trueOffset >= 62)
                        break;

                    if (currIndex + 1 < foundchars.Count)
                    {
                        if (NextInt(foundchars[currIndex + 1], currValue))
                        {
                            currCount++;
                            currIndex++;
                            currValue++;
                        }
                        else
                            break;
                    }
                    else
                        break;
                }

                if (currCount > 1 && currCount > longestLength)
                {
                    longestLength = currCount;
                    longestStart = foundchars[i][j];
                }
            }
        }

        if (longestLength == 0)
            return new Substring(0, "", 0, 0);
        else
            return new Substring(index, inthis.Substring(longestStart, longestLength), longestStart, longestLength);

    }

    bool NextInt(int[] checkthese, int followingthis)
    {
        return (checkthese.Contains(followingthis + 1));
    }


    Dictionary<char, byte[]> CharByteDictionary;

    void CreateCharByteDictionary()
    {
        DefaultCharByteDictionary();

        List<char> keychars = new List<char>(PointerDictionary.Keys);
        for (int i = 0; i < keychars.Count; i++)
        {

            Pointer2 point = PointerDictionary[keychars[i]];
            int len = point.Length;
            int off = point.Offset;
            int index = point.PointerIndex;

            // 2 bytes: ABCD EFGH IJKL MNOP
            // offset is D->J
            // length is K->P
            // B high = pointer
            // next two bytes -> C->P is index of pointer
            int b12 = (off << 6) | len; // lower 14 bits: offset + length
            b12 &= 0x1FFF; // just to be safe, clear upper 3 bits
            b12 |= 0x4000; // pointer flag
            int b34 = index;

            byte[] pbytes = new byte[4];
            pbytes[0] = (byte)((b34 >> 8) & 0xFF); // index high
            pbytes[1] = (byte)(b34 & 0xFF); // index low
            pbytes[2] = (byte)((b12 >> 8) & 0xFF); // pointer info high
            pbytes[3] = (byte)(b12 & 0xFF); // pointer info low
            CharByteDictionary.Add(keychars[i], pbytes);
        }
    }

    void DefaultCharByteDictionary()
    {
        CharByteDictionary = new Dictionary<char, byte[]> {
            //Clashes with PLAYER_NAME_CHAR is it meant to be here?
            { '\n', new byte[2] { 0x00, 0x00 } }, { StringEditor.PNamePlaceholder, new byte[2] { 0x00, 0x01 } }, /*{ '@', new byte[2] { 0x00, 0x01 } },*/ { ',', new byte[2] { 0x00, 0x02 } }, { '\u25CF', new byte[2] { 0x00, 0x03 } },
            { '~', new byte[2] { 0x00, 0x1E } }, { '\uFF3B', new byte[2] { 0x00, 0x1F } }, { '\uFF3D', new byte[2] { 0x00, 0x20 } }, { '\uFF08', new byte[2] { 0x00, 0x3B } },
            { '\uFF09', new byte[2] { 0x00, 0x3C } }, { '\uFF10', new byte[2] { 0x00, 0x46 } }, { '\uFF01', new byte[2] { 0x00, 0x47 } }, { '\uFF02', new byte[2] { 0x00, 0x48 } },
            { '\uFF03', new byte[2] { 0x00, 0x49 } }, { '\uFF04', new byte[2] { 0x00, 0x4A } }, { '\uFF05', new byte[2] { 0x00, 0x4B } }, { '\uFF06', new byte[2] { 0x00, 0x4C } },
            { '\uFF07', new byte[2] { 0x00, 0x4D } }, { '\uFF1D', new byte[2] { 0x00, 0x4E } }, { '\uFF3E', new byte[2] { 0x00, 0x4F } }, { '\uFF0D', new byte[2] { 0x00, 0x50 } },
            { '\uFFE5', new byte[2] { 0x00, 0x51 } }, { '\uFF0C', new byte[2] { 0x00, 0x52 } }, { '\uFF0E', new byte[2] { 0x00, 0x53 } }, { '\uFF0F', new byte[2] { 0x00, 0x54 } },
            { '\uFF3F', new byte[2] { 0x00, 0x55 } }, { ' ', new byte[2] { 0x00, 0x70 } }, { '[', new byte[2] { 0x00, 0x71 } }, { ']', new byte[2] { 0x00, 0x72 } }, { '(', new byte[2] { 0x00, 0x8D } },
            { ')', new byte[2] { 0x00, 0x8E } }, { '0', new byte[2] { 0x00, 0x98 } }, { '!', new byte[2] { 0x00, 0x99 } }, { '"', new byte[2] { 0x00, 0x9A } }, { '#', new byte[2] { 0x00, 0x9B } },
            { '$', new byte[2] { 0x00, 0x9C } }, { '%', new byte[2] { 0x00, 0x9D } }, { '&', new byte[2] { 0x00, 0x9E } }, { '\'', new byte[2] { 0x00, 0x9F } }, { '=', new byte[2] { 0x00, 0xA0 } },
            { '^', new byte[2] { 0x00, 0xA1 } }, { '-', new byte[2] { 0x00, 0xA2 } }, { '\u00A5', new byte[2] { 0x00, 0xA3 } }, { '.', new byte[2] { 0x00, 0xA4 } }, { '/', new byte[2] { 0x00, 0xA5 } },
            { '_', new byte[2] { 0x00, 0xA6 } }, { '\u221E', new byte[2] { 0x01, 0x4F } }, { '?', new byte[2] { 0x01, 0x51 } }, { ':', new byte[2] { 0x01, 0x52 } },
            { '\u00B7', new byte[2] { 0x01, 0x53 } }, { '\u03B1', new byte[2] { 0x01, 0x69 } }, { '<', new byte[2] { 0x01, 0x6A } }, { '>', new byte[2] { 0x01, 0x6B } },
            { '\uFFF3', new byte[2] { 0x01, 0x6C } }, { '\u25A1', new byte[2] { 0x01, 0x6D } }, { '\u25B3', new byte[2] { 0x01, 0x6E } }, { '\u00D7', new byte[2] { 0x01, 0x6F } }, { ';', new byte[2] { 0x01, 0x70 } },
            { '\uFFF1', new byte[2] { 0x00, 0x00 } }
        };

        for (int i = 0; i < 26; i++)
        {
            CharByteDictionary.Add((char)('A' + i), new byte[2] { 0x00, (byte)(0x56 + i) });
            CharByteDictionary.Add((char)('a' + i), new byte[2] { 0x00, (byte)(0x73 + i) });
            CharByteDictionary.Add((char)(0xFF21 + i), new byte[2] { 0x00, (byte)(0x04 + i) });
            CharByteDictionary.Add((char)(0xFF41 + i), new byte[2] { 0x00, (byte)(0x21 + i) });
        }

        for (int i = 0; i < 9; i++)
        {
            CharByteDictionary.Add((char)('1' + i), new byte[2] { 0x00, (byte)(0x8F + i) });
            CharByteDictionary.Add((char)(0xFF11 + i), new byte[2] { 0x00, (byte)(0x3D + i) });
        }

        //What are these
        for (int i = 0; i < 194; i++)
        {
            if (i == 168 | (i >= 170 && i <= 172))
            {
            }
            else
            {
                // Shouldl this be + i?
                CharByteDictionary.Add((char)(0xD0A7 + i), new byte[2] { 0x00, (byte)(0xA7 + 1) });
            }

        }

    }


    //Still causes (pointer corruption) issues for some text in the tutorial, maybe some pointer issues but no longer crashes and fixes player name usage
    // also corrupts string 2226 to  is ior two... instead of  duel or two...
    
    public void ExportToBytes()
    {
        Offsets = new List<int>();
        StringEditor.StringBytes = new List<byte>();
        // Start of offset for index 30
        int currentoffset = StringEditor.FirstEnglishOffset;
        CreateCharByteDictionary();
        for (int index = 30; index < modStrings.Count; index++)
        {
            Offsets.Add(currentoffset);
            for (int i = 0; i < modStrings[index].Length; i++)
            {
                if (modStrings[index][i] == StringEditor.PNamePlaceholder)
                {
                    bool isPlayerName = true;
                    for (int j = 0; j < 12; j++)
                    {
                        int checkIndex = i + j;
                        if (checkIndex >= modStrings[index].Length ||
                            modStrings[index][checkIndex] != StringEditor.PNamePlaceholder)
                        {
                            isPlayerName = false;
                            break;
                        }
                    }
                    if (isPlayerName)
                    {
                        bool isEndOfString = (i + 12) == modStrings[index].Length;

                        byte[] pnameBytes = isEndOfString
                            ? new byte[4] { 0x8C, 0x40, 0x02, 0xC0 }
                            : new byte[4] { 0x8C, 0x40, 0x02, 0x40 };

                        foreach (byte b in pnameBytes)
                        {
                            StringEditor.StringBytes.Add(b);
                        }
                        currentoffset += (pnameBytes.Length / 2);
                        i += 11;
                        continue;
                    }
                }

                if (!CharByteDictionary.TryGetValue(modStrings[index][i], out var charBytes))
                {
                    Console.WriteLine($" Unknown char U+{(int)modStrings[index][i]:X4} at string {index}");
                    continue;
                }

                byte[] characterBytes = new byte[charBytes.Length];
                Array.Copy(charBytes, characterBytes, charBytes.Length);

                bool isPointer = modStrings[index][i] >= '\uE000' && modStrings[index][i] < '\uF000';
                bool isEnglish = characterBytes.Length == 2 && !isPointer;

                if (i == modStrings[index].Length - 1)
                {
                    characterBytes[0] |= 0x80;
                }
                if (isEnglish)
                {
                    if (!characterBytes.SequenceEqual(CharByteDictionary[StringEditor.PNamePlaceholder]))
                        characterBytes[0] |= 0x20;
                    else
                        characterBytes[0] |= 0x00;
                }

                Array.Reverse(characterBytes);
                foreach (byte b in characterBytes) StringEditor.StringBytes.Add(b);

                currentoffset += (characterBytes.Length / 2);
            }
        }

        ExportOffsetsToBytes();
    }


    void ExportOffsetsToBytes()
    {
        StringEditor.OffsetBytes = new List<byte>();

        for (int i = 0; i < Offsets.Count; i++)
        {
            byte[] savethis = new byte[4];
            int offset = Offsets[i];

            for (int j = 0; j < 4; j++)
            {
                savethis[j] = (byte)(offset / (int)(Math.Pow(256, 3 - j)));
                offset = offset % (int)(Math.Pow(256, 3 - j));
            }

            Array.Reverse(savethis);

            foreach (byte b in savethis)
                StringEditor.OffsetBytes.Add(b);
        }

    }


    public class Substring
    {
        public int Index;
        public string SubString;
        public int Offset;
        public int Length;

        public Substring(int index, string str, int offset, int length)
        {
            Index = index;
            SubString = str;
            Offset = offset;
            Length = length;
        }
    }

    public class Pointer2
    {
        public int Index;
        public int PointerIndex;
        public int Offset;
        public int Length;
        public string SubString;

        public Pointer2(int index, int pindex, int offset, int length, string substring)
        {
            Index = index;
            PointerIndex = pindex;
            Offset = offset;
            Length = length;
            SubString = substring;
        }
    }
}