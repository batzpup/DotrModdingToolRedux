using System.Collections;
namespace DotrModdingTool2IMGUI;

public static class BitArrayExtension
	{
public static byte toByte(this BitArray input, int startIndex = 0, int bitLength = 8)
{
	BitArray newBitArray = new BitArray(bitLength);
	
	for (int i = 0, bi = startIndex; i < bitLength; i++, bi++)
	{
		newBitArray[i] = input[bi]; 
	}

	byte[] result = new byte[1];
	newBitArray.CopyTo(result, 0);

	return result[0];
}

public static byte[] ToByteArray(this BitArray bits)
  {
	byte[] ret = new byte[(bits.Length - 1) / 8 + 1];
	bits.CopyTo(ret, 0);
	return ret;
}

public static void setHalfByte(this BitArray bitArray, byte inputByte, int startIndex)
  {
	BitArray halfByteBitArray = new BitArray(new byte[] { inputByte });

	for (int i = 0, j = startIndex; j < startIndex + 4; i++, j++)
    {
		bitArray[j] = halfByteBitArray[i];
    }
  }

public static void copyRangeTo(this BitArray from, int[] range, ref BitArray to, int toStartIndex)
  {
	int fromStartIndex = range[0];
	int toEndIndex = range[1];

	for (int fi = fromStartIndex, ti = toStartIndex; fi <= toEndIndex; fi++, ti++)
    {
		to[ti] = from[fi];
    }
}
	}

	public static class ByteExtension
{
public static byte[] splitByte(this byte inputByte)
  {
	BitArray tempBitArray = new BitArray(new byte[] { inputByte });
	byte[] splitBytes = new byte[2];

	splitBytes[0] = tempBitArray.toByte(0, 4);
	splitBytes[1] = tempBitArray.toByte(4, 4);

	return splitBytes;
}
}

	public static class StringExtension
{
public static string Reverse(this string s)
{
	char[] charArray = s.ToCharArray();
	Array.Reverse(charArray);
	return new string(charArray);
}
	}