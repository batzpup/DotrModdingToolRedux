namespace DotrModdingTool2IMGUI;

public class Ai
{
	public static List<Ai> All { get; }
	public int Id { get; }
	public byte[] Bytes { get; }
	public string Name { get; }

	static Ai()
	{
		All = new List<Ai> {
			new Ai(0, "00117C90", "Seto"),
			new Ai(1, "00110F00", "Weevil Underwood"),
			new Ai(2, "0011C430", "Rex Raptor"),
			new Ai(3, "00121630", "Keith"),
			new Ai(4, "00126AB0", "Ishtar"),
			new Ai(5, "0012B9A0", "Necromancer"),
			new Ai(6, "00130E90", "Darkness-ruler"),
			new Ai(7, "00135060", "Labyrinth-ruler"),
			new Ai(8, "001396C0", "Pegasus Crawford"),
			new Ai(9, "0013DAD0", "Richard Slysheen of York"),
			new Ai(10, "001422E0", "Tea"),
			new Ai(11, "00146790", "T. Tristan Grey"),
			new Ai(12, "0014B730", "Margaret Mai Beaufort"),
			new Ai(13, "0014FB70", "Mako"),
			new Ai(14, "00154040", "Joey"),
			new Ai(15, "00154210", "J. Shadi Morton"),
			new Ai(16, "00158AB0", "Jasper Dice Tudor"),
			new Ai(17, "0015D180", "Bakura"),
			new Ai(18, "00162F60", "Yugi"),
			new Ai(19, "001676E0", "Manawyddan fab Llyr (vs. White Rose)"),
			new Ai(20, "0016B1F0", "Manawyddan fab Llyr (vs.Red Rose)"),
			new Ai(21, "00172370", "Deck Master K"),
			new Ai(-1, "00000000", "Bad Data")
		};
	}

	public Ai(byte[] bytes)
	{
		Ai matchedAi = Ai.GetAiByPointerValue(bytes);
		this.Id = matchedAi == null ? Ai.All.Last().Id : matchedAi.Id;
		this.Bytes = bytes;
		this.Name = Ai.GetAiName(bytes);
	}

	public Ai(int id, string hexPointerAddress, string name)
	{
		this.Id = id;
		byte[] bytes = Enumerable.Range(0, hexPointerAddress.Length)
			.Where(x => x % 2 == 0)
			.Select(x => Convert.ToByte(hexPointerAddress.Substring(x, 2), 16))
			.ToArray();
		this.Bytes = bytes;
		this.Name = name;
	}

	public static Ai GetAiByPointerValue(byte[] aiAddressPointer)
  {
		byte[] byteArray = (byte[])aiAddressPointer.Clone();
		Array.Reverse(byteArray);

		Ai matchedAi = Ai.All.Find(x => x.Bytes.SequenceEqual(byteArray));

		return matchedAi;
	}

	public static string GetAiName(byte[] aiAddressPointer)
	{
		Ai matchedAi = Ai.GetAiByPointerValue(aiAddressPointer);

		return matchedAi == null ? "???" : matchedAi.Name;
	}

	override public string ToString()
  {
		return this.Name;
  }
}