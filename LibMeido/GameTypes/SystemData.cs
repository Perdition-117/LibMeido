namespace LibMeido;

public class SystemData {
	private const string FileHeader = "CM3D2_SYSTEM2";

	public int Version { get; set; }
	public Dictionary<int, Dictionary<string, int>> ColorPresets { get; set; }
	public Dictionary<string, string> Variables { get; set; }

	public void Deserialize(BinaryReader reader) {
		var header = reader.ReadString();
		if (header != FileHeader) {
			throw new Exception($"Invalid header \"{header}\" for {nameof(SystemData)}. Expected \"{FileHeader}\".");
		}

		Version = reader.ReadInt32();

		ColorPresets = new();
		Variables = new();

		var numColorPresets = reader.ReadInt32();
		for (var i = 0; i < numColorPresets; i++) {
			var key = reader.ReadInt32();
			var numColorValues = reader.ReadInt32();
			var colorValues = new Dictionary<string, int>();
			for (var j = 0; j < numColorValues; j++) {
				var key2 = reader.ReadString();
				var value = reader.ReadInt32();
				colorValues.Add(key2, value);
			}
			ColorPresets.Add(key, colorValues);
		}

		var numVariables = reader.ReadInt32();
		for (var i = 0; i < numVariables; i++) {
			Variables.Add(reader.ReadString(), reader.ReadString());
		}
	}

	public void Serialize(BinaryWriter writer) {
		writer.Write(FileHeader);
		writer.Write(Version);
		writer.Write(ColorPresets.Count);
		foreach (var keyValuePair in ColorPresets) {
			writer.Write(keyValuePair.Key);
			writer.Write(keyValuePair.Value.Count);
			foreach (var keyValuePair2 in keyValuePair.Value) {
				writer.Write(keyValuePair2.Key);
				writer.Write(keyValuePair2.Value);
			}
		}
		writer.Write(Variables.Count);
		foreach (var keyValuePair3 in Variables) {
			writer.Write(keyValuePair3.Key);
			writer.Write(keyValuePair3.Value);
		}
	}
}
