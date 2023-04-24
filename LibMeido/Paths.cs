namespace LibMeido;

public class Paths {
	private const string FileHeader = "CM3D2_PATHS";

	public static IEnumerable<string> ReadPathFile(string filePath) {
		using var fileStream = File.OpenRead(filePath);
		using var binaryReader = new BinaryReader(fileStream);

		var header = binaryReader.ReadString();
		if (header != FileHeader) {
			throw new Exception($"Invalid header \"{header}\" for paths file. Expected \"{FileHeader}\".");
		}

		var version = binaryReader.ReadInt32();

		var numPaths = binaryReader.ReadInt32();
		for (var i = 0; i < numPaths; i++) {
			var text = binaryReader.ReadString();
			yield return text;
		}

		yield break;
	}
}
