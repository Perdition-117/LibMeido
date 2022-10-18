namespace LibMeido;

public class PartsColor {
	public ColorPart Part { get; set; }
	public bool Use { get; set; }
	public int MainHue { get; set; }
	public int MainChroma { get; set; }
	public int MainBrightness { get; set; }
	public int MainContrast { get; set; }
	public int ShadowRate { get; set; }
	public int ShadowHue { get; set; }
	public int ShadowChroma { get; set; }
	public int ShadowBrightness { get; set; }
	public int ShadowContrast { get; set; }
}

internal static partial class BinaryStreamExtensions {
	public static PartsColor ReadColor(this BinaryReader reader) {
		return new() {
			Use = reader.ReadBoolean(),
			MainHue = reader.ReadInt32(),
			MainChroma = reader.ReadInt32(),
			MainBrightness = reader.ReadInt32(),
			MainContrast = reader.ReadInt32(),
			ShadowRate = reader.ReadInt32(),
			ShadowHue = reader.ReadInt32(),
			ShadowChroma = reader.ReadInt32(),
			ShadowBrightness = reader.ReadInt32(),
			ShadowContrast = reader.ReadInt32(),
		};
	}

	public static void Write(this BinaryWriter writer, PartsColor color) {
		writer.Write(color.Use);
		writer.Write(color.MainHue);
		writer.Write(color.MainChroma);
		writer.Write(color.MainBrightness);
		writer.Write(color.MainContrast);
		writer.Write(color.ShadowRate);
		writer.Write(color.ShadowHue);
		writer.Write(color.ShadowChroma);
		writer.Write(color.ShadowBrightness);
		writer.Write(color.ShadowContrast);
	}
}
