namespace LibMeido;

public class MaidBody {
	private const string FileHeader = "CM3D2_MAID_BODY";

	public int Version { get; set; }

	internal MaidBody(BinaryReader reader) {
		var header = reader.ReadString();
		if (header != FileHeader) {
			throw new Exception($"Invalid header \"{header}\" for {nameof(MaidBody)}. Expected \"{FileHeader}\".");
		}

		Version = reader.ReadInt32();
	}

	public void Serialize(BinaryWriter writer) {
		writer.Write(FileHeader);
		writer.Write(Version);
	}
}
