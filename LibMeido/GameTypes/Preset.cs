namespace LibMeido;

public class Preset {
	private const string FileHeader = "CM3D2_PRESET";

	public int Version { get; set; }
	public PresetType Type { get; set; }
	public byte[] Texture { get; set; }
	public MaidPropList PropList { get; set; }
	public MultiColor ColorParts { get; set; }
	public MaidBody MaidBody { get; set; }

	public string FileName { get; set; }

	public MultiColor PartsColor;

	public Preset(string fileName) : this(File.Open(fileName, FileMode.Open), fileName) { }

	public Preset(Stream stream, string fileName) {
		using var reader = new BinaryReader(stream);

		Deserialize(reader);

		FileName = System.IO.Path.GetFileName(fileName);

		if (Version >= 2) {
			PartsColor = ColorParts;
		}

		if (Version >= 200) {
			//MaidBody = new(binaryReader);
		}
	}

	internal void Deserialize(BinaryReader binaryReader) {
		var header = binaryReader.ReadString();
		if (header != FileHeader) {
			throw new Exception($"Invalid header \"{header}\" for {nameof(Preset)}. Expected \"{FileHeader}\".");
		}

		Version = binaryReader.ReadInt32();
		Type = (PresetType)binaryReader.ReadInt32();

		var imageSize = binaryReader.ReadInt32();
		if (imageSize != 0) {
			Texture = binaryReader.ReadBytes(imageSize);
		}

		PropList = new(binaryReader);

		if (Version >= 2) {
			ColorParts = new(binaryReader);
		}

		if (Version >= 200) {
			MaidBody = new(binaryReader);
		}
	}

	internal void Serialize(BinaryWriter binaryWriter) {
		binaryWriter.Write(FileHeader);
		binaryWriter.Write(Version);
		binaryWriter.Write((int)Type);

		if (Texture != null) {
			binaryWriter.Write(Texture.Length);
			binaryWriter.Write(Texture);
		} else {
			binaryWriter.Write(0);
		}

		PropList.Serialize(binaryWriter);

		if (Version >= 2) {
			ColorParts.Serialize(binaryWriter);
		}

		if (Version >= 200) {
			MaidBody.Serialize(binaryWriter);
		}
	}

	public void Save(string path) {
		using var fileStream = new FileStream(path, FileMode.CreateNew);
		var binaryWriter = new BinaryWriter(fileStream);
		Serialize(binaryWriter);
	}
}
