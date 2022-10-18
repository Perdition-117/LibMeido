using System.Drawing;
using LibMeido.MeidoStubs;
using LibMeido.UnityStubs;

namespace LibMeido;

public class Material {
	private const string FileHeader = "CM3D2_MATERIAL";

	public int Version { get; set; }
	public string FileName { get; set; }
	public string MaterialName { get; set; }
	public string ShaderName { get; set; }
	public string Text4 { get; set; }

	public Material() {	}

	public Material(string path) {
		using var reader = new BinaryReader(File.Open(path, FileMode.Open));

		Deserialize(reader);
	}

	void Deserialize(BinaryReader reader) {
		var header = reader.ReadString();
		if (header != FileHeader) {
			throw new Exception($"Invalid header \"{header}\" for {nameof(Material)}. Expected \"{FileHeader}\".");
		}

		Version = reader.ReadInt32();
		FileName = reader.ReadString();

		ReadMaterial(reader);
	}

	public void ReadMaterial(BinaryReader reader) {
		MaterialName = reader.ReadString();
		ShaderName = reader.ReadString();
		Text4 = reader.ReadString();

		while (true) {
			var materialType = reader.ReadString();
			if (materialType == "end") {
				break;
			}
			var propertyName = reader.ReadString();
			switch (materialType) {
				case "tex":
					string a2 = reader.ReadString();
					switch (a2) {
						case "null":
							//material2.SetTexture(propName, null);
							break;
						case "tex2d":
							var t = new MateTex2d();
							string textureName = reader.ReadString();
							string textureName2 = reader.ReadString();
							t.PropertyName = propertyName;
							t.TextureName = textureName;
							t.Offset = reader.ReadVector2();
							t.Scale = reader.ReadVector2();
							break;
						case "texRT":
							string text9 = reader.ReadString();
							string text10 = reader.ReadString();
							break;
					}
					break;
				case "col":
					var r = reader.ReadSingle();
					var g = reader.ReadSingle();
					var b = reader.ReadSingle();
					var a = reader.ReadSingle();
					var value4 = Color.FromArgb((int)a, (int)r, (int)g, (int)b);
					break;
				case "vec":
					var value5 = reader.ReadVector4();
					break;
				case "f":
					var value6 = reader.ReadSingle();
					break;
				default:
					Console.WriteLine("マテリアルが読み込めません。不正なマテリアルプロパティ型です " + materialType);
					break;
			}
		}
	}
}
