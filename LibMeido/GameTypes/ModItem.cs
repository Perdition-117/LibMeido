namespace LibMeido;

public class ModItem : BaseItem {
	private const string FileHeader = "CM3D2_MOD";

	// parsed properties
	public string BaseItem { get; set; }
	public string MpnName { get; set; }
	public string ColorSet { get; set; }
	public Dictionary<string, byte[]> Textures { get; } = new();

	public byte[] IconTexture { get; set; }

	public bool IsMod;
	public int FileNameHash;

	public ModItem(Stream stream, string fileName) {
		using var reader = new BinaryReader(stream);
		Deserialize(reader);

		IsMod = true;
		FileName = Path.GetFileName(fileName);
		FileNameHash = FileName.ToLower().GetHashCode();

		if (Enum.TryParse<Mpn>(CategoryName, out var mpn2)) {
			Mpn = mpn2;
		} else {
			Console.Write("There is no category " + CategoryName);
			Mpn = Mpn.null_mpn;
		}

		var mpn = Mpn.null_mpn;
		if (mpn != Mpn.null_mpn) {
			ColorSetMpn = mpn;
			if (!string.IsNullOrEmpty(ColorSet)) {
				MenuNameInColorSet = ColorSet;
			}
		}

		if (!string.IsNullOrEmpty(IconFileName)) {
			IconTexture = Textures[IconFileName];
		}

		Priority = 999f;
	}

	public ModItem(string fileName) : this(new FileStream(fileName, FileMode.Open), fileName) { }

	internal void Deserialize(BinaryReader reader) {
		var header = reader.ReadString();
		if (header != FileHeader) {
			throw new Exception($"Invalid header \"{header}\" for {nameof(ModItem)}. Expected \"{FileHeader}\".");
		}

		Version = reader.ReadInt32();
		IconFileName = reader.ReadString();
		BaseItem = reader.ReadString();
		Name = reader.ReadString();
		CategoryName = reader.ReadString();
		Description = reader.ReadString();
		MpnName = reader.ReadString();

		if (!Enum.TryParse<Mpn>(MpnName, out var mpn)) {
			throw new Exception($"Invalid category {MpnName}");
		} else if (mpn != Mpn.null_mpn) {
			ColorSet = reader.ReadString();
		}

		var properties = reader.ReadString();

		using var stringReader = new StringReader(properties);
		while (true) {
			var line = stringReader.ReadLine();
			if (line == null) {
				break;
			}
			var values = new List<string>(line.Split(new char[] { '\t', ' ' }, StringSplitOptions.RemoveEmptyEntries));
			var key = values[0];
			values.RemoveAt(0);
			Properties.Add(new(key, values));
		}

		var numTextures = reader.ReadInt32();
		for (var i = 0; i < numTextures; i++) {
			var textureName = reader.ReadString();
			var textureLength = reader.ReadInt32();
			var textureData = reader.ReadBytes(textureLength);
			Textures.Add(textureName, textureData);
		}
	}

	internal void Serialize(BinaryWriter writer) {
		writer.Write(FileHeader);
		writer.Write(Version);
		writer.Write(IconFileName);
		writer.Write(BaseItem);
		writer.Write(Name);
		writer.Write(CategoryName);
		writer.Write(Description);
		writer.Write(MpnName);

		if (Enum.TryParse<Mpn>(MpnName, out var mpn) && mpn != Mpn.null_mpn) {
			writer.Write(ColorSet);
		}

		var stringWriter = new StringWriter();
		foreach (var property in Properties) {
			stringWriter.WriteLine(property);
		}
		writer.Write(stringWriter.ToString());

		writer.Write(Textures.Count);
		foreach (var texture in Textures) {
			writer.Write(texture.Key);
			writer.Write(texture.Value.Length);
			writer.Write(texture.Value);
		}
	}
}
