namespace LibMeido;

public class PriorityMaterial {
	private const string FileHeader = "CM3D2_PMATERIAL";

	public int Version { get; set; }
	public int NameHash { get; set; }
	public string Name { get; set; }
	public float Value { get; set; }

	void Deserialize(BinaryReader reader) {
		var header = reader.ReadString();
		if (header != FileHeader) {
			throw new Exception($"Invalid header \"{header}\" for {nameof(PriorityMaterial)}. Expected \"{FileHeader}\".");
		}

		Version = reader.ReadInt32();
		NameHash = reader.ReadInt32();
		Name = reader.ReadString();
		Value = reader.ReadSingle();
	}
}
