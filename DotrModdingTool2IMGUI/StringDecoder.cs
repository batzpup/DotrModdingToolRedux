namespace DotrModdingTool2IMGUI;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.Json;

public class StringDecoder
{
    Dictionary<int, char> knownChars;

    public StringDecoder()
    {
        knownChars = new Dictionary<int, char> {
            { 0x00, '\n' }, { 0x01, '\uFFF2' }, //PNAME_CHAR
            { 0x02, ',' }, { 0x03, '\u25CF' },
            { 0x1E, '~' }, { 0x1F, '\uFF3B' }, { 0x20, '\uFF3D' }, { 0x3B, '\uFF08' },
            { 0x3C, '\uFF09' }, { 0x46, '\uFF11' }, { 0x47, '\uFF01' }, { 0x48, '\uFF02' },
            { 0x49, '\uFF03' }, { 0x4A, '\uFF04' }, { 0x4B, '\uFF05' }, { 0x4C, '\uFF06' },
            { 0x4D, '\uFF07' }, { 0x4E, '\uFF1D' }, { 0x4F, '\uFF3E' }, { 0x50, '\uFF0D' },
            { 0x51, '\uFFE5' }, { 0x52, '\uFF0C' }, { 0x53, '\uFF0E' }, { 0x54, '\uFF0F' },
            { 0x55, '\uFF3F' }, { 0x70, ' ' }, { 0x71, '[' }, { 0x72, ']' }, { 0x8D, '(' },
            { 0x8E, ')' }, { 0x98, '0' }, { 0x99, '!' }, { 0x9A, '"' }, { 0x9B, '#' },
            { 0x9C, '$' }, { 0x9D, '%' }, { 0x9E, '&' }, { 0x9F, '\'' }, { 0xA0, '=' },
            { 0xA1, '^' }, { 0xA2, '-' }, { 0xA3, '\u00A5' }, { 0xA4, '.' }, { 0xA5, '/' },
            { 0xA6, '_' }, { 0x14F, '\u221E' }, { 0x151, '?' }, { 0x152, ':' },
            { 0x153, '\u00B7' }, { 0x169, '\u03B1' }, { 0x16A, '<' }, { 0x16B, '>' },
            { 0x16C, '\uFFF3' } /* III character for Richard III */, { 0x16D, '\u25A1' }, { 0x16E, '\u25B3' }, { 0x16F, '\u00D7' }, { 0x170, ';' }
        };

        for (int i = 0; i < 26; i++)
        {
            knownChars[0x56 + i] = ((char)('A' + i));
            knownChars[0x73 + i] = ((char)('a' + i));
            knownChars[0x04 + i] = ((char)(0xFF21 + i));
            knownChars[0x21 + i] = ((char)(0xFF41 + i));
        }
        for (int i = 0; i < 9; i++)
        {
            knownChars[0x8F + i] = (i + 1).ToString()[0];
            knownChars[0x3D + i] = ((char)(0xFF11 + i));
        }

        for (int i = 0; i < 194; i++)
        {
            if (i == 168 | (i >= 170 & i <= 172))
            {
            }
            else
                knownChars[0xA7 + i] = ((char)(0xD0A7 + i));
        }
    }

    List<int> ReadOffsetTable(BinaryReader reader)
    {
        reader.BaseStream.Seek(DataAccess.OffsetTable, SeekOrigin.Begin);
        List<int> offsets = new List<int>(3073);
        for (int i = 0; i < 3073; i++)
        {
            offsets.Add(reader.ReadInt32());
        }
        return offsets;
    }

    List<ushort> ReadStringBlob(BinaryReader reader)
    {
        reader.BaseStream.Seek(0x2A4AD4, SeekOrigin.Begin);
        List<ushort> blob = new List<ushort>(74252);
        for (int i = 0; i < 74252; i++)
        {
            blob.Add(reader.ReadUInt16());
        }
        return blob;
    }

    List<List<int>> RecursiveRead(List<int> offsets, List<ushort> blob, int blobIndex, int length)
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

    List<List<int>> ReadString(List<int> offsets, List<ushort> blob, int index)
    {
        int blobIndex = offsets[index];
        List<List<int>> lines = new List<List<int>> { new List<int>() };
        while (true)
        {
            if ((blob[blobIndex] & 0x4000) != 0)
            {
                int subLength = blob[blobIndex] & 0x3F;
                int pointerindex = blob[blobIndex + 1] & 0x3FFF;
                int pointerStart = offsets[pointerindex];
                int pointeroffset = (blob[blobIndex] >> 6) & 0x7F;
                pointerStart += pointeroffset;
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

    public void Run()
    {
        var fs = DataAccess.fileStream;
        using (var reader = new BinaryReader(fs, Encoding.UTF8, true))
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


            chars.RemoveWhere(c => knownChars.ContainsKey(c));

            for (int i = 0; i < 3073; i++)
            {
                if (new HashSet<int>(stringCharSets[i]).IsSubsetOf(knownChars.Keys)) continue;
            }

            StringEditor.StringTable = new Dictionary<int, string>();
            for (int i = 0; i < strings.Count; i++)
            {
                var realChars = new List<char>();
                foreach (var line in strings[i])
                {
                    foreach (var charValue in line)
                    {
                        if (knownChars.TryGetValue(charValue, out var c))
                        {
                            realChars.Add(c);
                        }

                        else
                            realChars.Add('\uFFFD');
                    }
                    //Non character used for 
                    realChars.Add('\n');
                }
                //transforms the custom codes to their string representations
                string tempStr = string.Join("", realChars);
                tempStr = tempStr.Remove(tempStr.Length-1)
                    .Replace("\uFFF2", "PNAME_CHAR")
                    .Replace("\uFFF3", "III");
                StringEditor.StringTable.Add(i,tempStr);
            }
            if (StringEditor.ShouldDumpText)
            {
                StringEditor.ExportStringsToJSON("strings.json");
            }


        }
    }

   
}