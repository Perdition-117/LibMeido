namespace LibMeido;

public class MultiColor {
	private const string FileHeader = "CM3D2_MULTI_COL";

	private int Version { get; set; }
	private OrderedDictionary<ColorPart, PartsColor> Colors { get; } = new();

	public PartsColor[] PartsColor;

	internal MultiColor(BinaryReader reader) {
		Deserialize(reader);

		PartsColor = new PartsColor[13];
		for (var i = 0; i < 13; i++) {
			PartsColor[i] = DefaultPartColors[i];
			PartsColor[i].Part = (ColorPart)i;
		}

		foreach (var color in Colors) {
			var Name = color.Key;
			var ColorV = color.Value;
			ColorV.Part = Name;
			PartsColor[(int)Name] = ColorV;
		}
	}

	internal void Deserialize(BinaryReader reader) {
		var header = reader.ReadString();
		if (header != FileHeader) {
			throw new Exception($"Invalid header \"{header}\" for {nameof(MultiColor)}. Expected \"{FileHeader}\".");
		}

		Version = reader.ReadInt32();
		var numColors = reader.ReadInt32();
		if (Version <= 1200 && ((Version < 200) ? (numColors != 7) : (numColors != 9))) {
			throw new Exception("The infinite number of colors is different. Saved data is corrupted.");
		}

		if (Version <= 1200) {
			for (var i = 0; i < numColors; i++) {
				var colorPart = (ColorPart)i;
				Colors.Add((ColorPart)i, reader.ReadColor());
			}
		} else {
			while (true) {
				var value = reader.ReadString();
				if (Enum.TryParse<ColorPart>(value, true, out var colorPart) && colorPart == ColorPart.MAX) {
					break;
				}
				Colors.Add(colorPart, reader.ReadColor());
			}
		}
	}

	internal void Serialize(BinaryWriter writer) {
		writer.Write(FileHeader);
		writer.Write(Version);

		writer.Write(Colors.Count);

		//if (version <= 1200 && ((version < 200) ? (numColors != 7) : (numColors != 9))) {
		//	throw new Exception("The infinite number of colors is different. Saved data is corrupted.");
		//}

		if (Version <= 1200) {
			foreach (var color in Colors) {
				writer.Write(color.Value);
			}
		} else {
			foreach (var color in Colors) {
				writer.Write(color.Value.Part.ToString());
				writer.Write(color.Value);
			}
			writer.Write(ColorPart.MAX.ToString());
		}
	}

	private static readonly PartsColor[] DefaultPartColors = new PartsColor[] {
		new() {
			MainHue = 6,
			MainChroma = 117,
			MainBrightness = 179,
			MainContrast = 94,
		},
		new() {
			MainHue = 6,
			MainChroma = 117,
			MainBrightness = 179,
			MainContrast = 94,
		},
		new() {
			MainHue = 69,
			MainChroma = 186,
			MainBrightness = 270,
			MainContrast = 94,
		},
		new() {
			MainHue = 0,
			MainChroma = 0,
			MainBrightness = 183,
			MainContrast = 0,
		},
		new() {
			MainHue = 0,
			MainChroma = 0,
			MainBrightness = 0,
			MainContrast = 0,
		},
		new() {
			MainHue = 18,
			MainChroma = 149,
			MainBrightness = 247,
			MainContrast = 100,
		},
		new() {
			MainHue = 18,
			MainChroma = 149,
			MainBrightness = 247,
			MainContrast = 100,
		},
		new() {
			MainHue = 69,
			MainChroma = 0,
			MainBrightness = 67,
			MainContrast = 100,
		},
		new() {
			MainHue = 18,
			MainChroma = 100,
			MainBrightness = 138,
			MainContrast = 85,
		},
		new() {
			MainHue = 18,
			MainChroma = 36,
			MainBrightness = 434,
			MainContrast = 100,
			ShadowRate = 255,
			ShadowHue = 18,
			ShadowChroma = 79,
			ShadowBrightness = 321,
			ShadowContrast = 0,
		},
		new() {
			MainHue = 0,
			MainChroma = 56,
			MainBrightness = 185,
			MainContrast = 47,
		},
		new() {
			MainHue = 0,
			MainChroma = 56,
			MainBrightness = 185,
			MainContrast = 47,
		},
		new() {
			MainHue = 18,
			MainChroma = 60,
			MainBrightness = 200,
			MainContrast = 128,
		},
	};
}
