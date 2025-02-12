namespace DotrModdingTool2IMGUI;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

public static class StringDecoder
{
    public static Dictionary<int, string> StringTable;
    public static bool ShouldDumpText = true;

    public const int CustomDuelistNameStart = 141;
    public const int CustomDuelistNameEnd = 144;
    public const int DuelistNameOffsetStart = 196;
    public const int DuelistNameOffsetEnd = 217;
    public const int DuelArenaNamesStart = 218;
    public const int DuelArenaNamesEnd = 242;
    public const int CardNamesOffset = 320;
    public const int CardEffectTextOffset = 1174;

    static List<int> ReadOffsetTable(BinaryReader reader)
    {
        reader.BaseStream.Seek(0x2A1AD0, SeekOrigin.Begin);
        List<int> offsets = new List<int>(3073);
        for (int i = 0; i < 3073; i++)
        {
            offsets.Add(reader.ReadInt32());
        }
        return offsets;
    }

    private static List<ushort> ReadStringBlob(BinaryReader reader)
    {
        reader.BaseStream.Seek(0x2A4AD4, SeekOrigin.Begin);
        List<ushort> blob = new List<ushort>(74252);
        for (int i = 0; i < 74252; i++)
        {
            blob.Add(reader.ReadUInt16());
        }
        return blob;
    }

    private static List<List<int>> RecursiveRead(List<int> offsets, List<ushort> blob, int blobIndex, int length)
    {
        List<List<int>> lines = new List<List<int>> { new List<int>() };

        while (length > 0)
        {
            if ((blob[blobIndex] & 0x4000) != 0)
            {
                int subLength = blob[blobIndex] & 0x3F;
                int pointerStart = offsets[blob[blobIndex + 1] & 0x3FFF];
                pointerStart += (blob[blobIndex] >> 6) & 0x7F;
                var subStr = RecursiveRead(offsets, blob, pointerStart, subLength);
                if (subStr.Count > 0)
                {
                    lines[lines.Count - 1].AddRange(subStr[0]);
                    lines.AddRange(subStr.GetRange(1, subStr.Count - 1));
                }
                blobIndex += 2;
                length -= 2;
            }
            else if ((blob[blobIndex] & 0x1FFF) != 0)
            {
                lines[lines.Count - 1].Add(blob[blobIndex] & 0x1FFF);
                blobIndex++;
                length--;
            }
            else
            {
                lines.Add(new List<int>());
                blobIndex++;
                length--;
            }
            if ((blob[blobIndex - 1] & 0x8000) != 0)
                return lines;
        }
        return lines;
    }

    private static List<List<int>> ReadString(List<int> offsets, List<ushort> blob, int index)
    {
        int blobIndex = offsets[index];
        List<List<int>> lines = new List<List<int>> { new List<int>() };

        while (true)
        {
            if ((blob[blobIndex] & 0x4000) != 0)
            {
                int subLength = blob[blobIndex] & 0x3F;
                int pointerStart = offsets[blob[blobIndex + 1] & 0x3FFF];
                pointerStart += (blob[blobIndex] >> 6) & 0x7F;
                var subStr = RecursiveRead(offsets, blob, pointerStart, subLength);
                if (subStr.Count > 0)
                {
                    lines[lines.Count - 1].AddRange(subStr[0]);
                    lines.AddRange(subStr.GetRange(1, subStr.Count - 1));
                }
                blobIndex += 2;
            }
            else if ((blob[blobIndex] & 0x1FFF) != 0)
            {
                lines[lines.Count - 1].Add(blob[blobIndex] & 0x1FFF);
                blobIndex++;
            }
            else
            {
                lines.Add(new List<int>());
                blobIndex++;
            }

            if ((blob[blobIndex - 1] & 0x8000) != 0)
                return lines;
        }
    }

    public static void Run()
    {
        var fs = DataAccess.fileStream;
        using (var reader = new BinaryReader(fs,Encoding.UTF8,true))
        {
            var offsets = ReadOffsetTable(reader);
            var blob = ReadStringBlob(reader);

            var strings = new List<List<List<int>>>();
            for (int i = 0; i < 3073; i++)
            {
                strings.Add(ReadString(offsets, blob, i));
            }

            var stringCharSets = new List<HashSet<int>>();
            foreach (var str in strings)
            {
                var charSet = new HashSet<int>();
                foreach (var line in str)
                {
                    charSet.UnionWith(line);
                }
                stringCharSets.Add(charSet);
            }

            var chars = new HashSet<int>();
            foreach (var charSet in stringCharSets)
            {
                chars.UnionWith(charSet);
            }

            Dictionary<int, string> knownChars = new Dictionary<int, string> {
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
            chars.RemoveWhere(c => knownChars.ContainsKey(c));
            var badStrings = new List<int>();
            for (int i = 0; i < 3073; i++)
            {
                if (new HashSet<int>(stringCharSets[i]).IsSubsetOf(knownChars.Keys)) continue;
                badStrings.Add(i);
            }

            StringTable = new Dictionary<int, string>();
            for (int i = 0; i < strings.Count; i++)
            {
                var realChars = new List<string>();
                foreach (var line in strings[i])
                {
                    foreach (var charValue in line)
                    {
                        if (knownChars.ContainsKey(charValue))
                            realChars.Add(knownChars[charValue]);
                        else
                            realChars.Add("\uFFFD");
                    }
                    realChars.Add("\n");
                }
                realChars.Remove(realChars[^1]);
                StringTable.Add(i, string.Join("", realChars));
            }

            if (ShouldDumpText)
            {
                var options = new JsonSerializerOptions {
                    WriteIndented = true,
                    Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping

                };
                string jsonOutput = JsonSerializer.Serialize(StringTable, options);
                File.WriteAllText("strings.json", jsonOutput, Encoding.UTF8);
            }

        }
    }
}