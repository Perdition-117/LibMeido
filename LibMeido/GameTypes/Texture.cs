namespace LibMeido;

public class Texture {
	private const string FileHeader = "CM3D2_TEX";

	public int Version { get; set; }
	public string Path { get; set; }
	public List<SubTexture> Textures { get; set; }
	public int Width { get; set; }
	public int Height { get; set; }
	public int TextureFormat { get; set; }
	public byte[] ImageBuffer { get; set; }

	public Texture(string path, byte[] data) {
		Version = 1000;
		Path = path;
		ImageBuffer = data;
	}

	public Texture(Stream stream) {
		Load(stream);
	}

	public Texture(byte[] buffer) {
		Load(buffer);
	}

	public void Write(BinaryWriter binaryWriter) {
		Serialize(binaryWriter);
	}

	public byte[] Load(string fileName) {
		var buffer = File.ReadAllBytes(fileName);
		return Load(buffer);
	}

	public byte[] Load(byte[] buffer) {
		using var memoryStream = new MemoryStream(buffer);
		return Load(memoryStream);
	}

	public byte[] Load(Stream stream) {
		using var binaryReader = new BinaryReader(stream, Encoding.UTF8);
		Deserialize(binaryReader);
		return ImageBuffer;
	}

	void Deserialize(BinaryReader binaryReader) {
		var header = binaryReader.ReadString();
		if (header != FileHeader) {
			throw new Exception($"Invalid header \"{header}\" for {nameof(Texture)}. Expected \"{FileHeader}\".");
		}

		Version = binaryReader.ReadInt32();
		Path = binaryReader.ReadString();
		if (Version >= 1010) {
			if (Version >= 1011) {
				Textures = new();
				var numTextures = binaryReader.ReadInt32();
				for (var i = 0; i < numTextures; i++) {
					Textures.Add(new() {
						X = binaryReader.ReadSingle(),
						Y = binaryReader.ReadSingle(),
						Width = binaryReader.ReadSingle(),
						Height = binaryReader.ReadSingle(),
					});
				}
			}
			Width = binaryReader.ReadInt32();
			Height = binaryReader.ReadInt32();
			TextureFormat = binaryReader.ReadInt32();
		}

		var imageSize = binaryReader.ReadInt32();
		ImageBuffer = binaryReader.ReadBytes(imageSize);
	}

	void Serialize(BinaryWriter binaryWriter) {
		binaryWriter.Write(FileHeader);
		binaryWriter.Write(Version);
		binaryWriter.Write(Path);

		if (Version >= 1010) {
			if (Version >= 1011) {
				foreach (var texture in Textures) {
					binaryWriter.Write(texture.X);
					binaryWriter.Write(texture.Y);
					binaryWriter.Write(texture.Width);
					binaryWriter.Write(texture.Height);
				}
			}
			binaryWriter.Write(Width);
			binaryWriter.Write(Height);
			binaryWriter.Write(TextureFormat);
		}

		binaryWriter.Write(ImageBuffer.Length);
		binaryWriter.Write(ImageBuffer);
	}
}

public class SubTexture {
	public float X { get; set; }
	public float Y { get; set; }
	public float Width { get; set; }
	public float Height { get; set; }
}
